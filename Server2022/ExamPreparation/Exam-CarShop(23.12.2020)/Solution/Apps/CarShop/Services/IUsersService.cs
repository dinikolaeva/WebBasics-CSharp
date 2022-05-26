namespace CarShop.Services
{
    public interface IUsersService
    {
        bool IsMechanic(string userId);

        bool IsUserOwnsCar(string userId, string carId);
    }
}
