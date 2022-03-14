using SUS.HTTP;
using SUS.MvcFramework.ViewEngine;
using System.Runtime.CompilerServices;
using System.Text;

namespace SUS.MvcFramework
{
    public abstract class Controller
    {
        private const string UserIdSessionName = "UserId";
        private SUSViewEngine viewEngine;

        public Controller()
        {
            this.viewEngine = new SUSViewEngine();
        }

        public HttpRequest Request { get; set; }
        protected HttpResponse View(object viewModel = null, [CallerMemberName] string viewPath = null)
        {
            var viewContent = System.IO.File
                                        .ReadAllText("Views/" +
                                        this.GetType().Name
                                        .Replace("Controller", string.Empty) +
                                        "/" + viewPath +
                                        ".cshtml");

            viewContent = this.viewEngine.GetHtml(viewContent, viewModel, this.GetUserId());

            string responseHtml = this.PutViewInLayout(viewContent, viewModel);

            var responseBodyBites = Encoding.UTF8.GetBytes(responseHtml);
            var response = new HttpResponse("text/html", responseBodyBites);

            return response;
        }

        protected HttpResponse File(string pathFile, string contentType)
        {
            var fileBytes = System.IO.File.ReadAllBytes(pathFile);
            var response = new HttpResponse(contentType, fileBytes);
            return response;
        }

        protected HttpResponse Redirect(string url)
        {
            var response = new HttpResponse(HttpStatusCode.Found);
            response.Headers.Add(new Header("Location", url));
            return response;
        }

        protected HttpResponse Error(string errorText)
        {
            var viewContent = $"<div class=\"alert alert-danger\" role=\"alert\">{errorText}</div>";
            string responseHtml = this.PutViewInLayout(viewContent);
            var responseBodyBites = Encoding.UTF8.GetBytes(responseHtml);
            var response = new HttpResponse("text/html", responseBodyBites, HttpStatusCode.ServerError);
            return response;
        }

        protected void SignIn(string userId)
        {
            this.Request.Session[UserIdSessionName] = userId;
        }

        protected void SignOut()
        {
            this.Request.Session[UserIdSessionName] = null;
        }

        protected bool IsUserSignedIn() => this.Request.Session.ContainsKey(UserIdSessionName) && this.Request.Session[UserIdSessionName] != null;

        protected string GetUserId() => this.Request.Session.ContainsKey(UserIdSessionName) ? this.Request.Session[UserIdSessionName] : null;

        private string PutViewInLayout(string viewContent, object viewModel = null)
        {
            var layout = System.IO.File.ReadAllText("Views/Shared/_Layout.cshtml");
            layout = layout.Replace("@RenderBody()", "___VIEW_GOES_HERE___");
            layout = this.viewEngine.GetHtml(layout, viewModel, this.GetUserId());

            var responseHtml = layout.Replace("___VIEW_GOES_HERE___", viewContent);

            return responseHtml;
        }
    }
}