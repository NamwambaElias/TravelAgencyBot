namespace TravelAgencyBot.Models
{
    public class FlightDetails
    {
        public string From { get; set; }
        public string To { get; set; }
        public string DepartureDate { get; set; }
        public string ReturnDate { get; set; }
        public double Price { get; set; } // Optional: calculated in summary
    }
}
