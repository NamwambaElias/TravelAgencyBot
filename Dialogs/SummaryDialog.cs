using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TravelAgencyBot.Cards;
using TravelAgencyBot.Models;

namespace TravelAgencyBot.Dialogs
{
    public class SummaryDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<FlightDetailsModel> _flightAccessor;
        private readonly IStatePropertyAccessor<HotelDetails> _hotelAccessor;
        private readonly IStatePropertyAccessor<TourGuideModel> _guideAccessor;

        public SummaryDialog(UserState userState) : base(nameof(SummaryDialog))
        {
            _flightAccessor = userState.CreateProperty<FlightDetailsModel>("FlightDetails");
            _hotelAccessor = userState.CreateProperty<HotelDetails>("HotelDetails");
            _guideAccessor = userState.CreateProperty<TourGuideModel>("TourGuide");

            var waterfallSteps = new WaterfallStep[]
            {
                ShowSummaryAsync,
                HandleResponseAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt("ResponsePrompt"));

            InitialDialogId = nameof(WaterfallDialog);
        }

        // Step 1: Show booking summary using adaptive card
        private async Task<DialogTurnResult> ShowSummaryAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var flight = await _flightAccessor.GetAsync(stepContext.Context, () => null, cancellationToken);
            var hotel = await _hotelAccessor.GetAsync(stepContext.Context, () => null, cancellationToken);
            var guide = await _guideAccessor.GetAsync(stepContext.Context, () => null, cancellationToken);

            // Calculate total price (simple estimation)
            double total = 0;
            if (flight != null) total += 300; // Static flight price
            if (hotel != null) total += hotel.Price;
            if (guide != null) total += guide.DailyRate;

            var summary = new BookingSummaryModel
            {
                Flight = flight,
                Hotel = hotel,
                TourGuide = guide,
                Price = total
            };

            var card = SummaryCardHelper.CreateBookingSummaryCard(summary);
            await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(card), cancellationToken);

            return Dialog.EndOfTurn;
        }

        // Step 2: Handle adaptive card submit or user text
        private async Task<DialogTurnResult> HandleResponseAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string action = null;

            if (stepContext.Context.Activity.Value != null && stepContext.Context.Activity.Value is IDictionary<string, object> valueDict)
            {
                if (valueDict.TryGetValue("action", out var actionValue))
                {
                    action = actionValue.ToString();
                }
            }

            if (string.IsNullOrEmpty(action))
            {
                action = stepContext.Context.Activity.Text?.Trim().ToLower();
            }

            switch (action)
            {
                case "confirm":
                    await stepContext.Context.SendActivityAsync("🎉 Booking confirmed! We wish you a fantastic journey!", cancellationToken: cancellationToken);
                    break;
                case "edit":
                    await stepContext.Context.SendActivityAsync("✏️ Sure! You can restart booking from the main menu.", cancellationToken: cancellationToken);
                    break;
                case "cancel":
                    await stepContext.Context.SendActivityAsync("❌ Booking cancelled. Let us know if you change your mind!", cancellationToken: cancellationToken);
                    break;
                default:
                    await stepContext.Context.SendActivityAsync("I didn't catch that. Please click one of the buttons on the card.", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(this.Id, null, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
