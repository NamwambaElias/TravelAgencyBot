using Microsoft.Bot.Schema;
using System.Collections.Generic;

namespace TravelAgencyBot.Cards
{
    public static class CardFactoryHelper
    {
        public static Attachment CreateMainMenuCard()
        {
            return new HeroCard
            {
                Title = "Main Menu",
                Subtitle = "Please select an option below:",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.PostBack, "✈️ Book a Flight", value: new { choice = "flight" }),
                    new CardAction(ActionTypes.PostBack, "🏨 Book a Hotel", value: new { choice = "hotel" }),
                    new CardAction(ActionTypes.PostBack, "🧭 Book a Tour Guide", value: new { choice = "tour" }),
                    new CardAction(ActionTypes.PostBack, "💼 View Booking Summary", value: new { choice = "summary" })
                }
            }.ToAttachment();
        }
    }
}
