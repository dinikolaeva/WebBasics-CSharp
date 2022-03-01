using SUS.HTTP;
using System.Runtime.CompilerServices;
using System.Text;

namespace SUS.MvcFramework
{
    public abstract class Controller
    {
        public HttpResponse View([CallerMemberName] string viewPath = null)
        {
            var layout = System.IO.File.ReadAllText("Views/Shared/_Layout.cshtml");

            var viewContent = System.IO.File
                                        .ReadAllText("Views/" +
                                        this.GetType().Name
                                        .Replace("Controller", string.Empty) +
                                        "/" + viewPath +
                                        ".cshtml");

            var responseHtml = layout.Replace("@RenderBody()", viewContent);

            var responseBodyBites = Encoding.UTF8.GetBytes(responseHtml);
            var response = new HttpResponse("text/html", responseBodyBites);

            return response;
        }

        public HttpResponse File(string pathFile, string contentType)
        {
            var fileBytes = System.IO.File.ReadAllBytes(pathFile);
            var response = new HttpResponse(contentType, fileBytes);
            return response;
        }

        public HttpResponse Redirect(string url)
        {
            var response = new HttpResponse(HttpStatusCode.Found);
            response.Headers.Add(new Header("Location", url));
            return response;
        }
    }
}