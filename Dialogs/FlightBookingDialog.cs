using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using TravelAgencyBot.Cards;
using TravelAgencyBot.Models;
using System.Collections.Generic;

namespace TravelAgencyBot.Dialogs
{
    public class FlightBookingDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<FlightDetailsModel> _flightAccessor;

        public FlightBookingDialog(UserState userState) : base(nameof(FlightBookingDialog))
        {
            _flightAccessor = userState.CreateProperty<FlightDetailsModel>("FlightDetails");

            var steps = new WaterfallStep[]
            {
                ShowFlightFormAsync,
                ProcessFlightFormAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), steps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog);
        }

        // Step 1: Send Adaptive Card form
        private async Task<DialogTurnResult> ShowFlightFormAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Attachment(FlightCardHelper.CreateFlightBookingCard());
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return Dialog.EndOfTurn; // Wait for user to submit card
        }

        // Step 2: Process and validate input
        private async Task<DialogTurnResult> ProcessFlightFormAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var value = stepContext.Context.Activity.Value as Dictionary<string, object>;

            if (value == null || !value.ContainsKey("type") || value["type"].ToString() != "flightSubmit")
            {
                await stepContext.Context.SendActivityAsync("Please fill out the form and click submit.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(this.Id, null, cancellationToken);
            }

            var departure = value["departureCity"]?.ToString();
            var destination = value["destinationCity"]?.ToString();
            var departDate = value["departureDate"]?.ToString();
            var returnDate = value["returnDate"]?.ToString();

            if (string.IsNullOrWhiteSpace(departure) || string.IsNullOrWhiteSpace(destination) || string.IsNullOrWhiteSpace(departDate))
            {
                await stepContext.Context.SendActivityAsync("Departure, destination, and departure date are required.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(this.Id, null, cancellationToken);
            }

            // Save to state
            var flightDetails = new FlightDetailsModel
            {
                From = departure,
                To = destination,
                DepartureDate = departDate,
                ReturnDate = returnDate,
                Price = 420.0 // Placeholder — can be calculated later
            };

            await _flightAccessor.SetAsync(stepContext.Context, flightDetails, cancellationToken);

            await stepContext.Context.SendActivityAsync($"✅ Flight booked from {departure} to {destination} on {departDate}.", cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
