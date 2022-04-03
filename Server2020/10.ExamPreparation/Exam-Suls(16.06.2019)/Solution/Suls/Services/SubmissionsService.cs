using Suls.Data;
using SulsApp.Data;
using System;
using System.Linq;

namespace Suls.Services
{
    public class SubmissionsService : ISubmissionsService
    {
        private readonly ApplicationDbContext db;
        private readonly Random random;

        public SubmissionsService(ApplicationDbContext db, Random random)
        {
            this.db = db;
            this.random = random;
        }
        public void Create(string problemId, string userId, string code)
        {
            var problemMaxPoints = this.db.Problems
                                          .Where(p => p.Id == problemId)
                                          .Select(p => p.Points)
                                          .FirstOrDefault();

            var submission = new Submission
            {
                ProblemId = problemId,
                Code = code,
                UserId = userId,
                AchievedResult = (ushort)this.random.Next(0, problemMaxPoints + 1),
                CreatedOn = DateTime.UtcNow
            };

            this.db.Submissions.Add(submission);
            this.db.SaveChanges();
        }

        public void Delete(string id)
        {
            var submission = this.db.Submissions.Where(s => s.Id == id).FirstOrDefault();

            this.db.Submissions.Remove(submission);
            this.db.SaveChanges();
        }
    }
}
