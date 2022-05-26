using CarShop.Data;
using System.Linq;

namespace CarShop.Services
{
    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext db;

        public UsersService(ApplicationDbContext db) => this.db = db;

        public bool IsMechanic(string userId) => this.db.Users.Any(u => u.Id == userId && u.IsMechanic);

        public bool IsUserOwnsCar(string userId, string carId) => this.db.Cars
                                                                         .Any(c => c.Id == carId && 
                                                                                   c.OwnerId == userId);
    }
}
