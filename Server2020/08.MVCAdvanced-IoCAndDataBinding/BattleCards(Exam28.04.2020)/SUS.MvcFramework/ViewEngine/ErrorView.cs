using System.Text;

namespace SUS.MvcFramework.ViewEngine
{
    public class ErrorView : IView
    {
        private readonly IEnumerable<string> errors;
        private readonly string cSharpCode;

        public ErrorView(IEnumerable<string> errors, string cSharpCode)
        {
            this.errors = errors;
            this.cSharpCode = cSharpCode;
        }
        public string ExecuteTemplate(object viewModel, string user)
        {
            var html = new StringBuilder();
            html.AppendLine($"<h1>View compile {this.errors.Count()} errors:</h1><ul>");

            foreach (var error in this.errors)
            {
                html.AppendLine($"<li>{error}</li>");
            }

            html.AppendLine($"</ul><pre>{cSharpCode}</pre>");
            return html.ToString();
        }
    }
}
