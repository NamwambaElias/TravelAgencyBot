using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TravelAgencyBot.Models;

namespace TravelAgencyBot.Dialogs
{
    public class ClosureDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;

        public ClosureDialog(UserState userState) : base(nameof(ClosureDialog))
        {
            _userProfileAccessor = userState.CreateProperty<UserProfile>("UserProfile");

            var waterfallSteps = new WaterfallStep[]
            {
                SendFarewellAsync,
                HandleChoiceAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt("ChoicePrompt"));

            InitialDialogId = nameof(WaterfallDialog);
        }

        // Step 1: Send thank you and options
        private async Task<DialogTurnResult> SendFarewellAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userProfile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            var card = new HeroCard
            {
                Title = $"Thank you, {userProfile.Name}!",
                Text = "We hope you have a wonderful trip! Would you like to start a new booking or exit?",
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, "🔁 Start Over", value: "start over"),
                    new CardAction(ActionTypes.ImBack, "🚪 Exit", value: "exit")
                }
            };

            var reply = MessageFactory.Attachment(card.ToAttachment());
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            return Dialog.EndOfTurn;
        }

        // Step 2: Handle user's button selection
        private async Task<DialogTurnResult> HandleChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choice = stepContext.Context.Activity.Text?.ToLower();

            if (choice.Contains("start"))
            {
                await stepContext.Context.SendActivityAsync("Great! Let's begin planning your next trip ✈️", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(nameof(MainDialog), null, cancellationToken); // Return to Main Menu
            }
            else if (choice.Contains("exit"))
            {
                await stepContext.Context.SendActivityAsync("Goodbye! Thank you for choosing Wanderlust Wonders 🌍", cancellationToken: cancellationToken);
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("Please choose an option by clicking one of the buttons.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(this.Id, null, cancellationToken);
            }
        }
    }
}
