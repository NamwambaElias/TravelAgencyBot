using Microsoft.Bot.Schema;
using System.Collections.Generic;

namespace TravelAgencyBot.Cards
{
    public static class TourGuideCardHelper
    {
        public static List<Attachment> GetTourGuideCards()
        {
            var guides = new List<(string Name, string Specialty, double Rating, double Price, string ImageUrl)>
            {
                ("Lena Rivera", "Historical Tours", 4.9, 120, "https://via.placeholder.com/300x180?text=Lena"),
                ("Marcus Kent", "Adventure & Hiking", 4.7, 100, "https://via.placeholder.com/300x180?text=Marcus"),
                ("Aya Tanaka", "Cultural Immersion", 5.0, 130, "https://via.placeholder.com/300x180?text=Aya")
            };

            var cards = new List<Attachment>();

            foreach (var guide in guides)
            {
                var card = new HeroCard
                {
                    Title = guide.Name,
                    Subtitle = $"{guide.Specialty}\n⭐ {guide.Rating} stars\n💰 ${guide.Price}/day",
                    Images = new List<CardImage> { new CardImage(guide.ImageUrl) },
                    Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.ImBack, $"Select {guide.Name}", value: guide.Name)
                    }
                };

                cards.Add(card.ToAttachment());
            }

            return cards;
        }
    }
}
