using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using TravelAgencyBot.Cards;
using TravelAgencyBot.Models;
using System.Collections.Generic;
using System;

namespace TravelAgencyBot.Dialogs
{
    public class HotelBookingDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<HotelDetails> _hotelAccessor;

        public HotelBookingDialog(UserState userState) : base(nameof(HotelBookingDialog))
        {
            _hotelAccessor = userState.CreateProperty<HotelDetails>("HotelDetails");

            var steps = new WaterfallStep[]
            {
                ShowHotelsAsync,
                CaptureHotelChoiceAsync,
                AskCheckInDateAsync,
                AskCheckOutDateAsync,
                ConfirmHotelBookingAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), steps));
            AddDialog(new TextPrompt("HotelPrompt"));
            AddDialog(new TextPrompt("DatePrompt"));

            InitialDialogId = nameof(WaterfallDialog);
        }

        // Step 1: Show hotel carousel
        private async Task<DialogTurnResult> ShowHotelsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Carousel(HotelCardHelper.GetHotelCards());
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return Dialog.EndOfTurn;
        }

        // Step 2: Capture user’s hotel selection
        private async Task<DialogTurnResult> CaptureHotelChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var text = stepContext.Context.Activity.Text;
            if (!text.StartsWith("Select "))
            {
                await stepContext.Context.SendActivityAsync("Please select a hotel from the options above.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(this.Id, null, cancellationToken);
            }

            var hotelName = text.Replace("Select ", "").Trim();
            stepContext.Values["hotelName"] = hotelName;
            return await stepContext.PromptAsync("DatePrompt", new PromptOptions
            {
                Prompt = MessageFactory.Text("📅 What is your check-in date? (YYYY-MM-DD)")
            }, cancellationToken);
        }

        // Step 3: Ask for check-in
        private async Task<DialogTurnResult> AskCheckInDateAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["checkIn"] = stepContext.Result.ToString();
            return await stepContext.PromptAsync("DatePrompt", new PromptOptions
            {
                Prompt = MessageFactory.Text("📅 Optional: Enter your check-out date (or say 'skip'):")
            }, cancellationToken);
        }

        // Step 4: Optional check-out
        private async Task<DialogTurnResult> AskCheckOutDateAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["checkOut"] = stepContext.Result.ToString();
            return await stepContext.NextAsync(null, cancellationToken);
        }

        // Step 5: Confirm and save hotel booking
        private async Task<DialogTurnResult> ConfirmHotelBookingAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var hotelName = stepContext.Values["hotelName"].ToString();
            var checkIn = stepContext.Values["checkIn"].ToString();
            var checkOut = stepContext.Values["checkOut"].ToString();
            double nightlyRate = hotelName.Contains("Azure") ? 160 : hotelName.Contains("Skyline") ? 140 : 120;

            // Try to calculate total nights
            double totalCost = nightlyRate;
            if (DateTime.TryParse(checkIn, out var inDate) && DateTime.TryParse(checkOut, out var outDate) && outDate > inDate)
            {
                var nights = (outDate - inDate).Days;
                totalCost = nightlyRate * nights;
            }

            var hotelDetails = new HotelDetails
            {
                HotelName = hotelName,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                NightlyRate = nightlyRate,
               Price = totalCost
            };

            await _hotelAccessor.SetAsync(stepContext.Context, hotelDetails, cancellationToken);

            await stepContext.Context.SendActivityAsync($"✅ You’ve booked **{hotelName}** from **{checkIn}** to **{checkOut}**.", cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
