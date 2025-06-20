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
    public class TourGuideBookingDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<TourGuideModel> _tourGuideAccessor;

        public TourGuideBookingDialog(UserState userState) : base(nameof(TourGuideBookingDialog))
        {
            _tourGuideAccessor = userState.CreateProperty<TourGuideModel>("TourGuide");

            var waterfallSteps = new WaterfallStep[]
            {
                ShowTourGuidesAsync,
                CaptureSelectionAsync,
                ConfirmTourGuideAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt("TourGuidePrompt"));

            InitialDialogId = nameof(WaterfallDialog);
        }

        // Step 1: Show tour guide cards
        private async Task<DialogTurnResult> ShowTourGuidesAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Carousel(TourGuideCardHelper.GetTourGuideCards());
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return Dialog.EndOfTurn;
        }

        // Step 2: Capture selection
        private async Task<DialogTurnResult> CaptureSelectionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var userInput = stepContext.Context.Activity.Text;
            if (!userInput.StartsWith("Select "))
            {
                await stepContext.Context.SendActivityAsync("❗ Please choose a guide by clicking a button.", cancellationToken: cancellationToken);
                return await stepContext.ReplaceDialogAsync(this.Id, null, cancellationToken);
            }

            var selectedName = userInput.Replace("Select ", "").Trim();
            stepContext.Values["SelectedGuide"] = selectedName;

            return await stepContext.NextAsync(null, cancellationToken);
        }

        // Step 3: Save selection and confirm
        private async Task<DialogTurnResult> ConfirmTourGuideAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var selectedName = stepContext.Values["SelectedGuide"].ToString();

            // Hardcoded guide details matching the cards
            var guideData = new Dictionary<string, TourGuideModel>
            {
                ["Lena Rivera"] = new TourGuideModel { Name = "Lena Rivera", Specialty = "Historical Tours", Rating = 4.9, DailyRate = 120 },
                ["Marcus Kent"] = new TourGuideModel { Name = "Marcus Kent", Specialty = "Adventure & Hiking", Rating = 4.7, DailyRate = 100 },
                ["Aya Tanaka"] = new TourGuideModel { Name = "Aya Tanaka", Specialty = "Cultural Immersion", Rating = 5.0, DailyRate = 130 },
            };

            var selectedGuide = guideData[selectedName];
            await _tourGuideAccessor.SetAsync(stepContext.Context, selectedGuide, cancellationToken);

            await stepContext.Context.SendActivityAsync($"✅ You've selected **{selectedGuide.Name}** specializing in *{selectedGuide.Specialty}*.", cancellationToken: cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
