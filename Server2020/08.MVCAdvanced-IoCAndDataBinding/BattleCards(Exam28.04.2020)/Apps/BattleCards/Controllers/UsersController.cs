using BattleCards.Services;
using SUS.HTTP;
using SUS.MvcFramework;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BattleCards.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUsersService usersService;

        //initialize service
        public UsersController(IUsersService usersService)
        {
            this.usersService = usersService;
        }
        public HttpResponse Login()
        {
            if (this.IsUserSignedIn())
            {
                return this.Redirect("/");
            }

            return this.View();
        }

        [HttpPost("/Users/Login")]
        public HttpResponse DoLogin(string username, string password)
        {
            /*
            var username = this.Request.FormData["username"];
            var password = this.Request.FormData["password"];
            */
            var userId = this.usersService.GetUserId(username, password);

            if (userId == null)
            {
                return this.Error("Invalid username or password!");
            }

            this.SignIn(userId);

            return this.Redirect("/Cards/All");
        }

        public HttpResponse Register()
        {
            if (this.IsUserSignedIn())
            {
                return this.Redirect("/");
            }

            return this.View();
        }

        [HttpPost("/Users/Register")]
        public HttpResponse DoRegister(string username, string email,string password, string confirmPassword)
        {
            /*
            var username = this.Request.FormData["username"];
            var email = this.Request.FormData["email"];
            var password = this.Request.FormData["password"];
            var confirmPassword = this.Request.FormData["confirmPassword"];
            */
            if (username == null || username.Length < 5 || username.Length > 20)
            {
                return this.Error("The username must be between 5 and 20 charecters!");
            }
            if (!Regex.IsMatch(username, @"^[a-zA-z0-9\.]+$"))
            {
                return this.Error("Invalid username. Only alphanumeric characters are allowed.");
            }

            if (string.IsNullOrWhiteSpace(email) || !new EmailAddressAttribute().IsValid(email))
            {
                return this.Error("Invalid email!");
            }

            if (password == null || password.Length < 6 || password.Length > 20)
            {
                return this.Error("The password must be between 6 and 20 charecters!");
            }

            if (password != confirmPassword)
            {
                return this.Error("Passwords don`t match!");
            }

            if (!this.usersService.IsUsernameAvailable(username))
            {
                return this.Error("This username is not available!");
            }

            if (!this.usersService.IsEmailAvailable(email))
            {
                return this.Error("This email is already taken!");
            }

            var userId = this.usersService.CreateUser(username, email, password);
            
            return this.Redirect("/Users/Login");
        }

        public HttpResponse Logout()
        {
            if (!this.IsUserSignedIn())
            {
                return this.Error("Only logged-in users can logout.");
            }

            this.SignOut();

            return this.Redirect("/");
        }
    }
}
