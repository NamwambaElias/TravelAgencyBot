using AdaptiveCards;
using System;
using System.Collections.Generic;
using TravelAgencyBot.Models;
using Microsoft.Bot.Schema;  // ✅ THIS IS THE CORRECT Attachment CLASS


public static class SummaryCardHelper
{
    public static Attachment CreateBookingSummaryCard(BookingSummary summary)
    {
        var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 2))
        {
            Body = new List<AdaptiveElement>
            {
                new AdaptiveTextBlock("📋 Your Travel Summary")
                {
                    Size = AdaptiveTextSize.Large,
                    Weight = AdaptiveTextWeight.Bolder
                },
                new AdaptiveTextBlock($"✈️ Flight: {summary.Flight?.From} → {summary.Flight?.To} on {FormatDate(summary.Flight?.DepartureDate)}"),
                new AdaptiveTextBlock($"🏨 Hotel: {summary.Hotel?.HotelName} from {FormatDate(summary.Hotel?.CheckInDate)}"),
                new AdaptiveTextBlock($"🧭 Guide: {summary.TourGuide?.Name} ({summary.TourGuide?.Specialty})"),
                new AdaptiveTextBlock($"💰 Total Price: ${summary.Price} USD")
                {
                    Size = AdaptiveTextSize.Medium,
                    Weight = AdaptiveTextWeight.Bolder,
                    Color = AdaptiveTextColor.Attention
                }
            },
            Actions = new List<AdaptiveAction>
            {
                new AdaptiveSubmitAction
                {
                    Title = "✅ Confirm Booking",
                    Data = new { action = "confirm" }
                },
                new AdaptiveSubmitAction
                {
                    Title = "✏️ Edit Booking",
                    Data = new { action = "edit" }
                },
                new AdaptiveSubmitAction
                {
                    Title = "❌ Cancel Booking",
                    Data = new { action = "cancel" }
                }
            }
        };

        return new Attachment
        {
            ContentType = AdaptiveCard.ContentType,
            Content = card
        };
    }

    private static string FormatDate(string rawDate)
    {
        if (DateTime.TryParse(rawDate, out DateTime parsed))
        {
            return parsed.ToShortDateString();
        }
        return rawDate ?? "N/A";
    }
}
