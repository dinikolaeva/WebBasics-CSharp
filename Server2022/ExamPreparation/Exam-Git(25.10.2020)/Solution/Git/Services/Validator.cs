using Git.ViewModels;
using Git.ViewModels.Commits;
using Git.ViewModels.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Git.Services
{
    internal class Validator : IValidator
    {
        public ICollection<string> ValidateUser(RegisterUserFormViewModel user)
        {
            var errors = new List<string>();

            if (user.Username == null || user.Username.Length < 5 || user.Username.Length > 20)
            {
                errors.Add($"Username '{user.Username}' is not valid. It must be between 5 and 20 characters long.");
            }

            if (user.Password == null || user.Password.Length <6 || user.Password.Length > 20)
            {
                errors.Add($"The provided password is not valid. It must be between 6 and 20 characters long.");
            }

            if (user.Password != null && user.Password.Any(x => x == ' '))
            {
                errors.Add($"The provided password cannot contain whitespaces.");
            }

            if (user.Password != user.ConfirmPassword)
            {
                errors.Add("Password and its confirmation are different.");
            }

            return errors;
        }

        public ICollection<string> ValidateRepository(CreateRepositoryViewModel repository)
        {
            var errors = new List<string>();

            if (repository.Name == null || repository.Name.Length < 3 || repository.Name.Length > 10)
            {
                errors.Add($"Repository name '{repository.Name}' is not valid. It must be between 3 and 10 characters long.");
            }

            if (repository.RepositoryType != "Public" && repository.RepositoryType != "Private")
            {
                errors.Add("Repository type can be either 'Public' or 'Private'.");
            }

            return errors;
        }

        public ICollection<string> ValidateCommit(CreateCommitFormViewModel commit)
        {
            var errors = new List<string>();

            if (commit.Description == null || commit.Description.Length < 5)
            {
                errors.Add($"Commit description is not valid. It must be at least 5 characters long.");
            }

            return errors;
        }
    }
}
