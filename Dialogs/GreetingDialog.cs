using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using TravelAgencyBot.Cards;
using TravelAgencyBot.Models;

namespace TravelAgencyBot.Dialogs
{
    public class GreetingDialog : ComponentDialog
    {
        private const string UserProfileId = "UserProfile";
        private readonly IStatePropertyAccessor<UserDataModel> _userProfileAccessor;

        public GreetingDialog(UserState userState)
            : base(nameof(GreetingDialog))
        {
            _userProfileAccessor = userState.CreateProperty<UserDataModel>(UserProfileId);

            // Define the dialog steps in a waterfall
            var waterfallSteps = new WaterfallStep[]
            {
                ShowWelcomeCardAsync,
                AskNameStepAsync,
                CaptureNameStepAsync,
                EndDialogStepAsync
            };

            // Add named dialogs
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt))); // Prompt for user name
        }

        // Step 1: Show the welcome card
        private async Task<DialogTurnResult> ShowWelcomeCardAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var welcomeCard = AdaptiveCardHelper.CreateWelcomeCard(); // We'll create this helper soon
            var activity = MessageFactory.Attachment(welcomeCard);
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);
            return Dialog.EndOfTurn;
        }

        // Step 2: Ask for the user's name
        private async Task<DialogTurnResult> AskNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(TextPrompt),
                new PromptOptions { Prompt = MessageFactory.Text("Before we begin, what's your name?") },
                cancellationToken);
        }

        // Step 3: Save the name to UserState
        private async Task<DialogTurnResult> CaptureNameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var name = (string)stepContext.Result;
            var userProfile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserDataModel(), cancellationToken);
            userProfile.Name = name;

            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"Nice to meet you, {name}! Let’s start planning your adventure 🧳"),
                cancellationToken);

            return await stepContext.NextAsync(null, cancellationToken);
        }

        // Step 4: End this dialog and signal handoff to the main dialog
        private async Task<DialogTurnResult> EndDialogStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
