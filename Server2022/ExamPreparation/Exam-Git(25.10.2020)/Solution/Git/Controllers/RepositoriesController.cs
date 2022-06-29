using Git.Data;
using Git.Data.Models;
using Git.Services;
using Git.ViewModels.Repositories;
using MyWebServer.Controllers;
using MyWebServer.Http;
using System.Linq;

namespace Git.Controllers
{
    public class RepositoriesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IValidator validator;

        public RepositoriesController(ApplicationDbContext db, IValidator validator)
        {
            this.db = db;
            this.validator = validator;
        }

        public HttpResponse All()
        {
            var repositories = this.db.Repositories
                                      .Where(r => r.IsPublic)
                                      .OrderBy(r => r.CreatedOn)
                                      .Select(r => new AllRepositoriesViewModel
                                      {
                                          Id = r.Id,
                                          Name = r.Name,
                                          Owner = r.Owner.Username,
                                          CreatedOn = r.CreatedOn.ToLocalTime().ToString("d"),
                                          Commits = r.Commits.Count()
                                      })
                                      .ToList();

            return this.View(repositories);
        }

        [Authorize]
        public HttpResponse Create()
        {
            return this.View();
        }

        [HttpPost]
        [Authorize]
        public HttpResponse Create(CreateRepositoryViewModel model)
        {
            var modelsError = this.validator.ValidateRepository(model);

            if (modelsError.Any())
            {
                return Error(modelsError);
            }

            var repository = new Repository
            {
                Name = model.Name,
                IsPublic = model.RepositoryType == "Public",
                OwnerId = this.User.Id
            };

            this.db.Repositories.Add(repository);
            this.db.SaveChanges();

            return this.Redirect("/Repositories/All");
        }
    }
}
