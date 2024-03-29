﻿using BattleCards.Data;
using BattleCards.ViewModels;
using BattleCards.ViewModels;
using SUS.HTTP;
using SUS.MvcFramework;

namespace BattleCards.Controllers
{
    public class CardsController : Controller
    {
        public HttpResponse Add()
        {
            return this.View();
        }

        [HttpPost("/Cards/Add")]
        public HttpResponse DoAdd()
        {
            var dbContext = new ApplicationDbContext();

            if (this.Request.FormData["name"].Length < 5)
            {
                return this.Error("Name should be at least 5 characters long.");
            }

            dbContext.Cards.Add(new Card
            {
                Attack = int.Parse(this.Request.FormData["attack"]),
                Health = int.Parse(this.Request.FormData["health"]),
                Description = this.Request.FormData["description"],
                Name = this.Request.FormData["name"],
                ImageUrl = this.Request.FormData["image"],
                Keyword = this.Request.FormData["keyword"],
            });

            dbContext.SaveChanges();

            return this.Redirect("/");
        }

        public HttpResponse All()
        {
            var db = new ApplicationDbContext();

            var cardsViewModel = db.Cards
                                   .Select(c => new CardViewModel
                                   {
                                       Name = c.Name,
                                       ImageUrl = c.ImageUrl,
                                       Attack = c.Attack,
                                       Health = c.Health,
                                       Type = c.Keyword,
                                       Description = c.Description,
                                   })
                                   .ToList();

            return this.View(new AllCardsViewModel { Cards = cardsViewModel });
        }

        public HttpResponse Collection()
        {
            return this.View();
        }
    }
}
