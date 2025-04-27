namespace TravelBridgeAPI.Models.FlightModels.FlightMinPrice
{
    public class Rootobject
    {
        public bool status { get; set; }
        public string message { get; set; }
        public long timestamp { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public string departureDate { get; set; }
        public string returnDate { get; set; }
        public string[] searchDates { get; set; }
        public int offsetDays { get; set; }
        public bool isCheapest { get; set; }
        public Price price { get; set; }
        public Pricerounded? priceRounded { get; set; }
    }

    public class Price
    {
        public string currencyCode { get; set; }
        public int units { get; set; }
        public int nanos { get; set; }
    }

    public class Pricerounded
    {
        public string currencyCode { get; set; }
        public int units { get; set; }
        public int nanos { get; set; }
    }
}
