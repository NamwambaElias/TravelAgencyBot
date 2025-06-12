using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System.IO;

namespace TravelAgencyBot.Cards
{
    public static class AdaptiveCardHelper
    {
        public static Attachment CreateWelcomeCard()
        {
            var cardJson = File.ReadAllText("Cards/WelcomeCard.json");

            return new Attachment
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(cardJson)
            };
        }
    }
}
