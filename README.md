````markdown
# ✈️ TravelAgencyBot

**TravelAgencyBot** is a modular, feature-rich conversational bot built with the Microsoft Bot Framework SDK in C#. It simulates a virtual assistant for a travel agency, helping users book flights, hotels, and tour guides while managing state and offering a dynamic UI with Adaptive Cards.

---

## 📦 Features

- 🛫 **Flight Booking** — Collects origin, destination, and date.
- 🏨 **Hotel Booking** — Collects location, check-in date, and nights.
- 🧭 **Tour Guide Booking** — Collects tour guide type and preferred language.
- 📋 **Booking Summary** — Shows a summary with options to confirm, edit, or cancel.
- 🔁 **State Management** — Uses `UserState` and `ConversationState` via in-memory storage.
- 💬 **Adaptive Cards** — Used for menus and summaries.
- 🔄 **Custom Middleware** — Resets booking data when needed.
- ✅ **Hero Cards** — Clean, friendly UI for choices.

---

## 🛠️ Technologies

- [.NET 6+](https://dotnet.microsoft.com/download)
- [Microsoft Bot Framework SDK](https://dev.botframework.com/)
- Adaptive Cards
- In-Memory State Management

---

## 🚀 Getting Started

### 1. Clone the repo

```bash
git clone https://github.com/yourusername/TravelAgencyBot.git
cd TravelAgencyBot
````

### 2. Install dependencies

Make sure you have the **.NET SDK** and **Bot Framework Emulator** installed.

* [.NET SDK](https://dotnet.microsoft.com/en-us/download)
* [Bot Framework Emulator](https://github.com/microsoft/BotFramework-Emulator)

### 3. Build and run the bot

```bash
dotnet build
dotnet run
```

### 4. Open in Bot Framework Emulator

1. Launch the Emulator.
2. Connect using:

   ```
   Endpoint: http://localhost:3978/api/messages
   Microsoft App ID: (leave empty)
   Microsoft App Password: (leave empty)
   ```

---

## 📁 Project Structure

```plaintext
TravelAgencyBot/
│
├── Bots/
│   └── TravelAgencyDialogBot.cs          # Main bot logic entry point
│
├── Cards/
│   ├── CardFactoryHelper.cs              # Hero card generator
│   └── SummaryCardHelper.cs             # Adaptive card summary
│
├── Dialogs/
│   ├── GreetingDialog.cs
│   ├── FlightBookingDialog.cs
│   ├── HotelBookingDialog.cs
│   ├── TourGuideBookingDialog.cs
│   ├── SummaryDialog.cs
│   └── MainDialog.cs                     # Routes to all other dialogs
│
├── Middleware/
│   └── BookingDataResetMiddleware.cs     # Clears state on cancel/reset
│
├── Models/
│   ├── UserProfile.cs
│   ├── BookingSummary.cs
│   ├── FlightDetails.cs
│   ├── HotelDetails.cs
│   └── TourGuideDetails.cs
│
├── AdapterWithErrorHandler.cs            # Adapter with error handling
├── Startup.cs                            # Service registration & pipeline
├── Program.cs                            # Host builder entry point
└── README.md                             # You’re reading this!
```

---

## 🧪 Testing Suggestions

* Test each dialog flow separately (flight, hotel, tour).
* Try navigating the bot using buttons only vs typing input.
* Confirm the summary shows your previous inputs.
* Test canceling midway and restarting to ensure state clears.

---

## 💡 Future Enhancements

* Add LUIS/QnA for NLP-based input.
* Integrate real APIs (e.g., Skyscanner, Booking.com).
* Add authentication for user accounts.
* Deploy to Azure Bot Services.

---

## 🤝 Contributions

Pull requests are welcome! For major changes, please open an issue first to discuss what you would like to change.

---

## 📄 License

[MIT](LICENSE)

---

> Built with ❤️ using the Microsoft Bot Framework SDK

```

