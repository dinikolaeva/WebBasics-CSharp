using Suls.Data;
using Suls.ViewModels.Problems;
using SulsApp.Data;
using System.Collections.Generic;
using System.Linq;

namespace Suls.Services
{
    public class ProblemsService : IProblemsService
    {
        private readonly ApplicationDbContext db;

        public ProblemsService(ApplicationDbContext db)
        {
            this.db = db;
        }
        public void Create(string name, ushort points)
        {
            var problem = new Problem { Name = name, Points = points };

            this.db.Problems.Add(problem);
            this.db.SaveChanges();
        }

        public IEnumerable<HomePageProblemViewModel> GetAll()
        {
            var problems = this.db.Problems
                                  .Select(p => new HomePageProblemViewModel
                                  {
                                      Id = p.Id,
                                      Name = p.Name,
                                      Count = p.Submissions.Count
                                  })
                                  .ToList();

            return problems;
        }

        public string GetNameById(string id)
        {
            var problemName = this.db.Problems
                                     .Where(p => p.Id == id)
                                     .Select(p => p.Name)
                                     .FirstOrDefault();
            return problemName;
        }

        public ProblemViewModel GetById(string id)
        {
            return this.db.Problems
                          .Where(p => p.Id == id)
                          .Select(p => new ProblemViewModel
                          {
                              Name = p.Name,
                              Submissions = p.Submissions
                                             .Select(s => new SubmissionViewModel
                                             {
                                                 SubmissionId = s.Id,
                                                 AchievedResult = s.AchievedResult,
                                                 CreatedOn = s.CreatedOn,
                                                 Username = s.User.Username,
                                                 MaxPoints = p.Points
                                             })
                          })
                          .FirstOrDefault();
        }
    }
}
