using CarShop.Data;
using CarShop.Services;
using CarShop.ViewModels.Cars;
using MyWebServer.Controllers;
using MyWebServer.Http;
using System.Linq;

namespace CarShop.Controllers
{
    public class CarsController : Controller
    {
        private IValidator validator;
        private IUsersService usersService;
        private readonly ApplicationDbContext db;

        public CarsController(IValidator validator, IUsersService usersService, ApplicationDbContext db)
        {
            this.validator = validator;
            this.usersService = usersService;
            this.db = db;
        }

        [Authorize]
        public HttpResponse Add()
        {
            if (this.usersService.IsMechanic(this.User.Id))
            {
                return Error("Mechanics cannot add cars.");
            }

            return this.View();
        }

        [HttpPost]
        [Authorize]
        public HttpResponse Add(CarsAddFormModel model)
        {
            var modelErrors = this.validator.ValidateCar(model);

            if (modelErrors.Any())
            {
                return Error(modelErrors);
            }

            var car = new Car
            {
                Model = model.Model,
                Year = model.Year,
                PictureUrl = model.Image,
                PlateNumber = model.PlateNumber,
                OwnerId = this.User.Id
            };

            this.db.Cars.Add(car);
            this.db.SaveChanges();

            return this.Redirect("/Cars/All");
        }

        [Authorize]
        public HttpResponse All()
        {
            var carsQuery = this.db.Cars.AsQueryable();

            if (this.usersService.IsMechanic(this.User.Id))
            {
                carsQuery = carsQuery.Where(i => i.Issues.Any(i => !i.IsFixed));
            }
            else
            {
                carsQuery = carsQuery.Where(o => o.OwnerId == this.User.Id);
            }

            var cars = carsQuery
                .Select(c => new AllCarsViewModel
            {
                Id = c.Id,
                Model = c.Model,
                Year = c.Year,
                Image = c.PictureUrl,
                PlateNumber = c.PlateNumber,
                FixedIssues = c.Issues.Where(i => i.IsFixed).Count(),
                RemainingIssues = c.Issues.Where(i => !i.IsFixed).Count()
            })
                .ToList();

            return this.View(cars);
        }
    }
}
