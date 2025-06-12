using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TravelAgencyBot.Dialogs;
using TravelAgencyBot.Models;

namespace TravelAgencyBot.Bots
{
    public class TravelAgencyDialogBot : ActivityHandler
    {
        private readonly Dialog _mainDialog;
        private readonly ConversationState _conversationState;
        private readonly UserState _userState;
        private readonly ILogger<TravelAgencyDialogBot> _logger;

        public TravelAgencyDialogBot(MainDialog mainDialog, ConversationState conversationState, UserState userState, ILogger<TravelAgencyDialogBot> logger)
        {
            _mainDialog = mainDialog;
            _conversationState = conversationState;
            _userState = userState;
            _logger = logger;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message Activity.");

            var dialogSet = new DialogSet(_conversationState.CreateProperty<DialogState>("DialogState"));
            dialogSet.Add(_mainDialog);
            var dialogContext = await dialogSet.CreateContextAsync(turnContext, cancellationToken);

            var result = await dialogContext.ContinueDialogAsync(cancellationToken);
            if (result.Status == DialogTurnStatus.Empty)
            {
                await dialogContext.BeginDialogAsync(_mainDialog.Id, null, cancellationToken);
            }

            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("👋 Welcome to VeloVoyage! Type anything to get started."), cancellationToken);
                }
            }
        }
    }
}
