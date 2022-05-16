using MyWebServer.Controllers;
using MyWebServer.Http;
using System;

namespace CarShop.Controllers
{
    public class HomeController : Controller
    {
        public HttpResponse Index()
        {
            if (this.User.IsAuthenticated)
            {
                this.Redirect("/Cars/All");
            }

            return this.View();
        }

        private bool IsUserSignedIn()
        {
            throw new NotImplementedException();
        }
    }
}
