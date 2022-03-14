using SUS.MvcFramework;

namespace BattleCards.Data
{
    public class User : IdentityUser<string>
    {
        public User()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Role = IdentityRole.User;
            this.Cards = new HashSet<UserCard>();
        }
        public virtual ICollection<UserCard> Cards { get; set; }
    }
}
