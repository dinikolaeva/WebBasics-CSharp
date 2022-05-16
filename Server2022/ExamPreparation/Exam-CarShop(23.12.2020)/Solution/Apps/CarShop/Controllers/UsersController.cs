using CarShop.Data;
using CarShop.Services;
using CarShop.ViewModels.Users;
using MyWebServer.Controllers;
using MyWebServer.Http;
using System.Linq;

namespace CarShop.Controllers
{
    using static Data.DataConstants;
    public class UsersController : Controller
    {
        private readonly IValidator validator;
        private readonly IPasswordHasher passwordHasher;
        private readonly ApplicationDbContext db;

        public UsersController(IValidator validator, IPasswordHasher passwordHasher, ApplicationDbContext db)
        {
            this.validator = validator;
            this.passwordHasher = passwordHasher;
            this.db = db;
        }

        public HttpResponse Register()
        {
            return this.View();
        }

        [HttpPost]
        public HttpResponse Register(RegisterUserFormModel model)
        {
            var modelErrors = this.validator.ValidateUser(model);

            if (this.db.Users.Any(u => u.Username == model.Username))
            {
                modelErrors.Add($"User with '{model.Username}' username already exists.");
            }

            if (this.db.Users.Any(u => u.Email == model.Email))
            {
                modelErrors.Add($"User with '{model.Email}' email already exists.");
            }

            if (modelErrors.Any())
            {
                return this.Error(modelErrors); ;
            }

            var user = new User
            {
                Username = model.Username,
                Password = this.passwordHasher.HashPassword(model.Password),
                Email = model.Email,
                IsMechanic = model.UserType == UserTypeMechanic
            };

            this.db.Users.Add(user);
            this.db.SaveChanges();

            return this.Redirect("/Users/Login");
        }

        public HttpResponse Login()
        {
            return this.View();
        }

        [HttpPost]
        public HttpResponse Login(LoginUserFormModel model)
        {
            var hashedPassword = this.passwordHasher.HashPassword(model.Password);

            var userId = this.db.Users
                              .Where(u => u.Username == model.Username &&
                                     u.Password == hashedPassword)
                              .Select(u => u.Id)
                              .FirstOrDefault();
            if (userId == null)
            {
                return Error("Username and password combination is not valid.");
            }

            this.SignIn(userId);

            return this.Redirect("/Cars/All");
        }

        public HttpResponse Logout()
        {
            this.SignOut();

            return this.Redirect("/");
        }
    }
}
