using Git.Data;
using Git.Data.Models;
using Git.Services;
using Git.ViewModels.Commits;
using MyWebServer.Controllers;
using MyWebServer.Http;
using System.Linq;

namespace Git.Controllers
{
    public class CommitsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IValidator validator;

        public CommitsController(ApplicationDbContext db, IValidator validator)
        {
            this.db = db;
            this.validator = validator;
        }

        [Authorize]
        public HttpResponse Create(string id)
        {
            var repository = this.db.Repositories
                                    .Where(r => r.Id == id)
                                    .Select(r => new CommitToRepositoryViewModel
                                    {
                                        Id = r.Id,
                                        Name = r.Name
                                    })
                                    .FirstOrDefault();

            if (repository == null)
            {
                return BadRequest();
            }

            return this.View(repository);
        }

        [HttpPost]
        [Authorize]
        public HttpResponse Create(CreateCommitFormViewModel model)
        {
            var modelsError = this.validator.ValidateCommit(model);

            if (modelsError.Any())
            {
                return Error(modelsError);
            }

            if (!this.db.Repositories.Any(r => r.Id == model.Id))
            {
                return NotFound();
            }

            var commit = new Commit
            {
                Description = model.Description,
                CreatorId = this.User.Id,
                RepositoryId = model.Id
            };

            this.db.Commits.Add(commit);
            this.db.SaveChanges();

            return this.Redirect("/Repositories/All");
        }

        [Authorize]
        public HttpResponse All()
        {
            var commits = this.db.Commits
                .Where(c => c.CreatorId == this.User.Id)
                .OrderByDescending(c => c.CreatedOn)
                .Select(c => new AllCommitsViewModel
                {
                    Id = c.Id,
                    Description = c.Description,
                    CreatedOn = c.CreatedOn.ToLocalTime().ToString("d"),
                    Repository = c.Repository.Name
                })
                .ToList();

            return this.View(commits);
        }

        [Authorize]
        public HttpResponse Delete(string id)
        {
            var commit = this.db.Commits.Find(id);

            if (commit == null|| commit.CreatorId != this.User.Id)
            {
                return BadRequest();
            }

            this.db.Commits.Remove(commit);
            this.db.SaveChanges();

            return this.Redirect("/Commits/All");
        }
    }
}
