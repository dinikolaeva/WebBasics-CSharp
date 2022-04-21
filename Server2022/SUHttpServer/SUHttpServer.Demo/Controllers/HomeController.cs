using SUHttpServer.Demo.Models;
using SUHttpServer.Server.Controllers;
using SUHttpServer.Server.HTTP;
using System.Text;
using System.Web;

namespace SUHttpServer.Demo.Controllers
{
    public class HomeController : Controller
    {
        private const string FileName = "content.txt";
        private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
        {
            var downloads = new List<Task<string>>();

            foreach (var url in urls)
            {
                downloads.Add(DownloadWebSiteContent(url));
            }

            var responses = await Task.WhenAll(downloads);

            var responsesString = string.Join(Environment.NewLine + new string('-', 100), responses);

            await System.IO.File.WriteAllTextAsync(fileName, responsesString);
        }

        internal Response Login()
        {
            throw new NotImplementedException();
        }

        private static async Task<string> DownloadWebSiteContent(string url)
        {
            var httpClient = new HttpClient();

            using (httpClient)
            {
                var response = await httpClient.GetAsync(url);

                var html = await response.Content.ReadAsStringAsync();

                return html.Substring(0, 2000);
            }
        }

        public HomeController(Request request)
            : base(request)
        {
        }

        public Response Index() => Text("Hello from the server!");
        public Response Redirect() => Redirect("https://softuni.org/");
        public Response Html() => View();
        public Response HtmlFormPost()
        {
            var name = this.Request.Form["Name"];
            var age = this.Request.Form["Age"];

            var model = new FormViewModel()
            {
                Name = name,
                Age = int.Parse(age)
            };

            return View(model);
        }

        public Response Content() => View();

        public Response DownloadContent()
        {
            DownloadSitesAsTextFile(HomeController.FileName, new string[] { "https://judge.softuni.org/", "https://softuni.org/" })
                .Wait();

            return File(HomeController.FileName);
        }

        public Response Cookies()
        {
            if (this.Request.Cookies.Any(c => c.Name != SUHttpServer.Server.HTTP.Session.SessionCookieName))
            {
                var cookieText = new StringBuilder();

                cookieText.AppendLine("<h1>Cookies</h1>");
                cookieText.Append("<table border='1'><tr><th>Name</th><th>Value</th></tr>");

                foreach (var cookie in this.Request.Cookies)
                {
                    cookieText.Append("<tr>");
                    cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Name)}</td>");
                    cookieText.Append($"<td>{HttpUtility.HtmlEncode(cookie.Value)}</td>");
                    cookieText.Append("</tr>");
                }
                cookieText.Append("</table>");

                return Html(cookieText.ToString());
            }

            var cookies = new CookieCollection();
            cookies.Add("My-Cookie", "My-Cookie");
            cookies.Add("My-Second-Cookie", "My-Second-Value");

            return Html("<h1>Cookies set!</h1>", cookies);
        }

        public Response Session()
        {
            string currentDateKey = "CurrentDate";
            var sessionExist = this.Request.Session.ContainsKey(currentDateKey);

            if (sessionExist)
            {
                var currentDate = this.Request.Session[currentDateKey];
                return Text($"Stored date: {currentDate}!");
            }
            return Text("Current date stored!");
        }
    }
}