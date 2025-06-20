using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TravelAgencyBot.Bots;
using TravelAgencyBot.Dialogs;
using TravelAgencyBot.Middleware;
using TravelAgencyBot.Models;

namespace TravelAgencyBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Called by the runtime to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add controller support (required by Bot Framework)
            services.AddControllers().AddNewtonsoftJson();

            // ✅ Add Authorization to fix InvalidOperationException
            services.AddAuthorization();

            // Register Bot Framework Adapter with error handling
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // State management
            services.AddSingleton<ConversationState>();
            services.AddSingleton<UserState>();

            // Accessors for strongly-typed user data
            services.AddSingleton<IStatePropertyAccessor<UserDataModel>>(sp =>
            {
                var userState = sp.GetRequiredService<UserState>();
                return userState.CreateProperty<UserDataModel>("UserProfile");
            });

            services.AddSingleton<IStatePropertyAccessor<BookingSummaryModel>>(sp =>
            {
                var userState = sp.GetRequiredService<UserState>();
                return userState.CreateProperty<BookingSummaryModel>("BookingSummary");
            });

            // Register in-memory storage for ConversationState and UserState
            services.AddSingleton<IStorage, MemoryStorage>();

            // Register the state objects
            services.AddSingleton<ConversationState>();
            services.AddSingleton<UserState>();


            // Dialogs (main and child dialogs)
            services.AddSingleton<MainDialog>();
            services.AddSingleton<GreetingDialog>();
            services.AddSingleton<FlightBookingDialog>();
            services.AddSingleton<HotelBookingDialog>();
            services.AddSingleton<TourGuideBookingDialog>();
            services.AddSingleton<SummaryDialog>();

            // Register custom middleware (optional)
            services.AddSingleton<IMiddleware, BookingDataResetMiddleware>();

            // Register the main bot (EchoBot)
            services.AddTransient<IBot, TravelAgencyDialogBot>();


        }

        // Called by the runtime to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // ✅ Authorization middleware must be registered if services.AddAuthorization() is used
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
