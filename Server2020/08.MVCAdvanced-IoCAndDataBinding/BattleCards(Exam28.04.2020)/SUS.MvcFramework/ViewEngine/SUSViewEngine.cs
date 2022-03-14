using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SUS.MvcFramework.ViewEngine
{
    public class SUSViewEngine : IViewEngine
    {
        public string GetHtml(string templateCode, object viewModel, string user)
        {
            string cSharpCode = GenerateCSharpCode(templateCode, viewModel);
            IView executableObject = GenerateExecutableCode(cSharpCode, viewModel);
            string html = executableObject.ExecuteTemplate(viewModel, user);
            return html;
        }

        private string GenerateCSharpCode(string templateCode, object viewModel)
        {
            string typeOfModel = "object";

            if (viewModel != null)
            {
                if (viewModel.GetType().IsGenericType)
                {
                    var modelName = viewModel.GetType().FullName;
                    var genericArguments = viewModel.GetType().GenericTypeArguments;
                    typeOfModel = modelName.Substring(0, modelName.IndexOf('`')) +
                        '<' +
                        string.Join(",", genericArguments.Select(a => a.FullName)) +
                        '>';
                }
                else
                {
                    typeOfModel = viewModel.GetType().FullName;
                }
            }

            string methodBody = GetMethodBody(templateCode);
            string cSharpCode = @"
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using SUS.MvcFramework.ViewEngine;

namespace ViewNamespace
{
    public class ViewClass : IView
    {
       public string ExecuteTemplate(object viewModel, string user)
       {
            var User = user;
            var Model = viewModel as " + typeOfModel + @";
            var html = new StringBuilder();
" + GetMethodBody(templateCode) + @"
return html.ToString();
       }
    }
}";
            return cSharpCode;
        }

        private string GetMethodBody(string templateCode)
        {
            Regex cSharpCodeRegex = new Regex(@"[^\""\s&\'\<]+");
            var supportedOperators = new List<string> { "for", "foreach", "while", "if", "else" };
            StringBuilder cSharpCode = new StringBuilder();
            StringReader sr = new StringReader(templateCode);

            string line;

            while ((line = sr.ReadLine()) != null)
            {
                if (supportedOperators.Any(x => line.TrimStart().StartsWith("@" + x)))
                {
                    var atSignLocation = line.IndexOf("@");
                    line = line.Remove(atSignLocation, 1);
                    cSharpCode.AppendLine(line);
                }
                else if (line.TrimStart().StartsWith("{") || line.TrimStart().StartsWith("}"))
                {
                    cSharpCode.AppendLine(line);
                }
                else
                {
                    cSharpCode.Append($"html.AppendLine(@\"");
                    while (line.Contains("@"))
                    {
                        var atSignLocation = line.IndexOf("@");
                        var htmlBeforeAtSign = line.Substring(0, atSignLocation);
                        cSharpCode.Append(htmlBeforeAtSign.Replace("\"", "\"\"") + "\" + ");
                        var lineAfterAtSign = line.Substring(atSignLocation + 1);
                        var code = cSharpCodeRegex.Match(lineAfterAtSign).Value;
                        cSharpCode.Append(code + " + @\"");
                        line = lineAfterAtSign.Substring(code.Length);
                    }

                    cSharpCode.AppendLine(line.Replace("\"", "\"\"") + "\");");
                }
            }

            return cSharpCode.ToString();
        }

        private IView GenerateExecutableCode(string cSharpCode, object viewModel)
        {
            var compileResult = CSharpCompilation.Create("ViewAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference
                               .CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(MetadataReference
                               .CreateFromFile(typeof(IView).Assembly.Location));

            if (viewModel != null)
            {
                if (viewModel.GetType().IsGenericType)
                {
                    var genericArguments = viewModel.GetType().GenericTypeArguments;

                    foreach (var genericArgument in genericArguments)
                    {
                        compileResult = compileResult
                            .AddReferences(MetadataReference.CreateFromFile(genericArgument.Assembly.Location));
                    }
                }

                compileResult = compileResult.AddReferences(MetadataReference.CreateFromFile(viewModel.GetType().Assembly.Location));
            }

            //add libraries:
            var libraries = Assembly.Load(new AssemblyName("netstandard"))
                                  .GetReferencedAssemblies();

            foreach (var lib in libraries)
            {
                compileResult = compileResult.AddReferences(MetadataReference
                                                   .CreateFromFile(Assembly.Load(lib).Location));
            }

            compileResult = compileResult.AddSyntaxTrees(SyntaxFactory.ParseSyntaxTree(cSharpCode));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                EmitResult result = compileResult.Emit(memoryStream);

                if (!result.Success)
                {
                    return new ErrorView(result.Diagnostics
                                               .Where(e => e.Severity == DiagnosticSeverity.Error)
                                               .Select(e => e.GetMessage()), cSharpCode);
                }

                try
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var byteAssembly = memoryStream.ToArray();
                    var assembly = Assembly.Load(byteAssembly);
                    var viewType = assembly.GetType("ViewNamespace.ViewClass");
                    var instance = Activator.CreateInstance(viewType);

                    return instance as IView;
                }
                catch (Exception ex)
                {
                    return new ErrorView(new List<string> { ex.ToString() }, cSharpCode);
                }
            }
        }
    }
}
