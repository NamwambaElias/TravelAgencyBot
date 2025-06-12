using AdaptiveCards;
using Microsoft.Bot.Schema;
using System.Collections.Generic;

namespace TravelAgencyBot.Cards
{
    public static class FlightCardHelper
    {
        public static Attachment CreateFlightBookingCard()
        {
            var card = new AdaptiveCard("1.4")
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock("✈️ Flight Booking")
                    {
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Large
                    },
                    new AdaptiveTextInput
                    {
                        Id = "departureCity",
                        Placeholder = "Enter departure city",
                        Style = AdaptiveTextInputStyle.Text
                    },
                    new AdaptiveTextInput
                    {
                        Id = "destinationCity",
                        Placeholder = "Enter destination city",
                        Style = AdaptiveTextInputStyle.Text
                    },
                    new AdaptiveDateInput
                    {
                        Id = "departureDate",
                        Placeholder = "Select departure date"
                    },
                    new AdaptiveDateInput
                    {
                        Id = "returnDate",
                        Placeholder = "Optional: Select return date",
                        IsRequired = false
                    }
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveSubmitAction
                    {
                        Title = "Submit",
                        Data = new { type = "flightSubmit" }
                    }
                }
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = card
            };
        }
    }
}
