namespace TravelBridgeAPI.Models.FlightModels.FlightDetails
{
    public class Rootobject
    {
        public bool status { get; set; }
        public string message { get; set; }
        public long timestamp { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string fromLocation { get; set; }
        public string toLocation { get; set; }
        public DateTime departureDate { get; set; }
        public DateTime returnDate { get; set; }
        public FlightMinPrice flightMinPrice { get; set; }
    }

    public class FlightMinPrice
    {
        public Price price { get; set; }
    }

    public class Price
    {
        public string currencyCode { get; set; }
        public int units { get; set; }
        public int nanos { get; set; }
    }
}
