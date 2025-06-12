using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TravelAgencyBot.Models;

namespace TravelAgencyBot.Middleware
{
    public class BookingDataResetMiddleware : IMiddleware
    {
        private readonly IStatePropertyAccessor<FlightDetails> _flightAccessor;
        private readonly IStatePropertyAccessor<HotelDetails> _hotelAccessor;
        private readonly IStatePropertyAccessor<TourGuide> _guideAccessor;
        private readonly IStatePropertyAccessor<BookingSummary> _summaryAccessor;

        public BookingDataResetMiddleware(UserState userState)
        {
            _flightAccessor = userState.CreateProperty<FlightDetails>("FlightDetails");
            _hotelAccessor = userState.CreateProperty<HotelDetails>("HotelDetails");
            _guideAccessor = userState.CreateProperty<TourGuide>("TourGuide");
            _summaryAccessor = userState.CreateProperty<BookingSummary>("BookingSummary");
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var text = turnContext.Activity.Text?.ToLowerInvariant();

                if (text != null && (text.Contains("cancel") || text.Contains("exit") || text.Contains("confirm")))
                {
                    await _flightAccessor.DeleteAsync(turnContext, cancellationToken);
                    await _hotelAccessor.DeleteAsync(turnContext, cancellationToken);
                    await _guideAccessor.DeleteAsync(turnContext, cancellationToken);
                    await _summaryAccessor.DeleteAsync(turnContext, cancellationToken);
                }
            }

            await next(cancellationToken);
        }
    }
}
