namespace TravelAgencyBot.Models
{
    public class BookingSummary
    {
        public FlightDetails Flight { get; set; }
        public HotelDetails Hotel { get; set; }
        public TourGuide TourGuide { get; set; }
        public double Price { get; set; }
    }
}
