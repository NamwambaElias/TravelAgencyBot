using Microsoft.Bot.Schema;
using System.Collections.Generic;

namespace TravelAgencyBot.Cards
{
    public static class HotelCardHelper
    {
        public static List<Attachment> GetHotelCards()
        {
            var hotels = new List<(string Name, string ImageUrl, string Description, double Price)>
            {
                ("Azure Coast Resort", "https://via.placeholder.com/300x180?text=Azure+Resort", "Beachfront luxury with spa.", 160),
                ("Skyline View Hotel", "https://via.placeholder.com/300x180?text=Skyline+View", "Modern downtown stay with skyline views.", 140),
                ("Rustic Trails Lodge", "https://via.placeholder.com/300x180?text=Rustic+Lodge", "Cozy mountain retreat with fireplaces.", 120)
            };

            var cards = new List<Attachment>();

            foreach (var hotel in hotels)
            {
                var card = new HeroCard
                {
                    Title = hotel.Name,
                    Subtitle = hotel.Description,
                    Images = new List<CardImage> { new CardImage(hotel.ImageUrl) },
                    Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.ImBack, $"Select {hotel.Name}", value: hotel.Name)
                    }
                };

                cards.Add(card.ToAttachment());
            }

            return cards;
        }
    }
}
