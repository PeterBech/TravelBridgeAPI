namespace TravelBridgeAPI.Models.FlightModels.FlightSearches
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
        public FlightOffer[] FlightOffers { get; set; }
    }

    public class FlightOffer
    {
        public string Token { get; set; }
        public Segment[] Segments { get; set; }
        public PriceBreakdown PriceBreakdown { get; set; }
    }

    public class Segment
    {
        public Airport DepartureAirport { get; set; }
        public Airport ArrivalAirport { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        // (eventuelt legs hvis du vil understøtte detaljerede flysegmenter)
    }

    public class Airport
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string CityName { get; set; }
        public string Country { get; set; }
        public string CountryName { get; set; }
        public string Province { get; set; }
    }

    public class PriceBreakdown
    {
        public Total Total { get; set; }
    }

    public class Total
    {
        public string CurrencyCode { get; set; }
        public int Units { get; set; }
        public int Nanos { get; set; }
    }



}


