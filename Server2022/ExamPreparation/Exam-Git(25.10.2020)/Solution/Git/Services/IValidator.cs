using Git.ViewModels;
using Git.ViewModels.Commits;
using Git.ViewModels.Repositories;
using System.Collections.Generic;

namespace Git.Services
{
    public interface IValidator
    {
        ICollection<string> ValidateUser(RegisterUserFormViewModel user);
        ICollection<string> ValidateRepository(CreateRepositoryViewModel repository);
        ICollection<string> ValidateCommit(CreateCommitFormViewModel commit);
    }
}
