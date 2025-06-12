using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TravelAgencyBot.Models;
using TravelAgencyBot.Cards;

namespace TravelAgencyBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;

        public MainDialog(UserState userState) : base(nameof(MainDialog))
        {
            // Accessor for retrieving user info from UserState
            _userProfileAccessor = userState.CreateProperty<UserProfile>("UserProfile");

            // Define waterfall steps
            var waterfallSteps = new WaterfallStep[]
            {
                ShowMainMenuAsync,
                RouteToDialogAsync
            };

            // Register dialog steps
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt))); // Used as a fallback input prompt

            // Register child dialogs
            AddDialog(new FlightBookingDialog(userState));
            AddDialog(new HotelBookingDialog(userState));
            AddDialog(new TourGuideBookingDialog(userState));
            AddDialog(new SummaryDialog(userState));

            // Set starting dialog
            InitialDialogId = nameof(WaterfallDialog);
        }

        // Step 1: Show main menu as a Hero Card
        private async Task<DialogTurnResult> ShowMainMenuAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);
            var welcomeText = $"What would you like to do next, {userProfile.Name}?";

            var reply = MessageFactory.Text(welcomeText);
            reply.Attachments = new List<Attachment> { CardFactoryHelper.CreateMainMenuCard() };

            // Send the card and wait for button click
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return Dialog.EndOfTurn;
        }

        // Step 2: Route user selection to correct dialog
        private async Task<DialogTurnResult> RouteToDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Expecting button input via Adaptive Card or Hero Card
            var value = stepContext.Context.Activity.Value as Dictionary<string, string>;

            if (value == null || !value.ContainsKey("choice"))
            {
                // If user didn't click a card, fallback to prompt
                await stepContext.Context.SendActivityAsync("Please select an option from the buttons below.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(this.Id, null, cancellationToken);
            }

            // Route to the appropriate dialog
            switch (value["choice"])
            {
                case "flight":
                    return await stepContext.BeginDialogAsync(nameof(FlightBookingDialog), null, cancellationToken);

                case "hotel":
                    return await stepContext.BeginDialogAsync(nameof(HotelBookingDialog), null, cancellationToken);

                case "tour":
                    return await stepContext.BeginDialogAsync(nameof(TourGuideBookingDialog), null, cancellationToken);

                case "summary":
                    return await stepContext.BeginDialogAsync(nameof(SummaryDialog), null, cancellationToken);

                default:
                    await stepContext.Context.SendActivityAsync("I didn't understand your selection. Please try again.", cancellationToken: cancellationToken);
                    return await stepContext.ReplaceDialogAsync(this.Id, null, cancellationToken);
            }
        }
    }
}
