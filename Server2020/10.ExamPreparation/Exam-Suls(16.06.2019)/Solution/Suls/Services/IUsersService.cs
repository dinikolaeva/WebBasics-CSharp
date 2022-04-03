namespace Suls.Services
{
    public interface IUsersService
    {
        void CreateUser(string username, string email, string password);
        string GetUserId(string username, string password);
        bool IsUsernameAvalable(string username);
        bool IsEmailAvalable(string email);
    }
}
