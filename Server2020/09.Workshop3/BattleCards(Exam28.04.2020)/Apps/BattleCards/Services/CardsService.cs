using BattleCards.Data;
using BattleCards.ViewModels.Cards;

namespace BattleCards.Services
{
    public class CardsService : ICardsService
    {
        private readonly ApplicationDbContext db;

        public CardsService(ApplicationDbContext db)
        {
            this.db = db;
        }
        public int AddCard(AddCardInputModel input)
        {
            var card = new Card
            {
                Attack = input.Attack,
                Health = input.Health,
                Description = input.Description,
                Name = input.Name,
                ImageUrl = input.Image,
                Keyword = input.Keyword,
            };

            this.db.Cards.Add(card);

            this.db.SaveChanges();
            return card.Id;
        }

        public IEnumerable<CardViewModel> GetAll()
        {
            return this.db.Cards
                          .Select(c => new CardViewModel
                          {
                              Id = c.Id,
                              Name = c.Name,
                              ImageUrl = c.ImageUrl,
                              Attack = c.Attack,
                              Health = c.Health,
                              Type = c.Keyword,
                              Description = c.Description,
                          })
                          .ToList();
        }

        public IEnumerable<CardViewModel> GetByUserId(string userId)
        {
            return this.db.UserCards
                          .Where(u => u.UserId == userId)
                          .Select(c => new CardViewModel
                          {
                              Attack = c.Card.Attack,
                              Health = c.Card.Health,
                              Name = c.Card.Name,
                              Description = c.Card.Description,
                              ImageUrl = c.Card.ImageUrl,
                              Type = c.Card.Keyword,
                              Id = c.CardId
                          })
                          .ToList();
        }
        public void AddCardToUserCollection(string userId, int cardId)
        {
            if (this.db.UserCards.Any(x => x.UserId == userId &&
                                           x.CardId == cardId))
            {
                return;
            }

            this.db.UserCards.Add(new UserCard
            {
                CardId = cardId,
                UserId = userId
            });

            this.db.SaveChanges();
        }

        public void RemoveCardFromUserCollection(string userId, int cardId)
        {
            var userCard = this.db.UserCards.FirstOrDefault(c => c.UserId == userId && c.CardId == cardId);

            if (userCard == null)
            {
                return;
            }

            this.db.UserCards.Remove(userCard);
            this.db.SaveChanges();
        }
    }
}
