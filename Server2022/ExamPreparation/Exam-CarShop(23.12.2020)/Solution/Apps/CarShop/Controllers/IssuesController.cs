using CarShop.Data;
using CarShop.Data.Models;
using CarShop.Services;
using CarShop.ViewModels.Cars;
using CarShop.ViewModels.Issues;
using MyWebServer.Controllers;
using MyWebServer.Http;
using System.Linq;

namespace CarShop.Controllers
{
    public class IssuesController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IValidator validator;
        private readonly ApplicationDbContext db;

        public IssuesController(IUsersService usersService, IValidator validator, ApplicationDbContext db)
        {
            this.usersService = usersService;
            this.validator = validator;
            this.db = db;
        }

        [Authorize]
        public HttpResponse CarIssues(string carId)
        {
            if (!this.IsUserCanAccessCar(carId))
            {
                return this.Unauthorized();
            }

            var carWithIssues = this.db.Cars
                                   .Where(c => c.Id == carId)
                                   .Select(c => new CarIssuesViewModel
                                   {
                                       Id = c.Id,
                                       Model = c.Model,
                                       Year = c.Year,
                                       UserIsMechanic = this.usersService.IsMechanic(this.User.Id),
                                       Issues = c.Issues.Select(i => new CarAddIssuesViewModel
                                       {
                                           Id = i.Id,
                                           IsFixed = i.IsFixed,
                                           Description = i.Description,
                                           IsFixedInformation = i.IsFixed ? "Yes" : "Not yet"
                                       })
                                   })
                                   .FirstOrDefault();

            if (carWithIssues == null)
            {
                return Error($"Car with ID '{carId}' doesn`t exist.");
            }

            return this.View(carWithIssues);
        }

        [Authorize]
        public HttpResponse Add(string carId)
        {
            return this.View(new AddIssueViewModel
            {
                CarId = carId
            });
        }

        [Authorize]
        [HttpPost]
        public HttpResponse Add(AddIssueFormModel model)
        {
            if (!this.IsUserCanAccessCar(model.CarId))
            {
                return this.Unauthorized();
            }

            var modelErrors = this.validator.ValidateIssue(model);

            if (modelErrors.Any())
            {
                return Error(modelErrors);
            }

            var issue = new Issue
            {
                CarId = model.CarId,
                Description = model.Description,
            };

            this.db.Issues.Add(issue);
            this.db.SaveChanges();

            return this.Redirect($"/Issues/CarIssues?carId={model.CarId}");
        }

        [Authorize]
        public HttpResponse Fix(string issueId, string carId)
        {
            var userIsMechanic = this.usersService.IsMechanic(this.User.Id);

            if (!userIsMechanic)
            {
                return this.Unauthorized();
            }

            var issue = this.db.Issues.Find(issueId);

            issue.IsFixed = true;

            this.db.SaveChanges();

            return this.Redirect($"/Issues/CarIssues?carId={carId}");
        }

        [Authorize]
        public HttpResponse Delete(string issueId, string carId)
        {
            if (!this.IsUserCanAccessCar(carId))
            {
                return Unauthorized();
            }

            var issue = this.db.Issues.Find(issueId);

            this.db.Issues.Remove(issue);

            this.db.SaveChanges();

            return Redirect($"/Issues/CarIssues?carId={carId}");
        }

        private bool IsUserCanAccessCar(string carId)
        {
            var userIsMechanic = this.usersService.IsMechanic(this.User.Id);

            if (!userIsMechanic)
            {
                var userOwnsCar = this.usersService.IsUserOwnsCar(this.User.Id, carId);

                if (!userOwnsCar)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
