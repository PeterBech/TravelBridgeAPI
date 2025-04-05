namespace TravelBridgeAPI.Models.FlightSearches
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
        public Aggregation aggregation { get; set; }
        public Flightoffer[] flightOffers { get; set; }
        public string atolProtectedStatus { get; set; }
        public bool isOffersCabinClassExtended { get; set; }
        public Baggagepolicy[] baggagePolicies { get; set; }
    }

    public class Aggregation
    {
    }

    public class Flightoffer
    {
        public object id { get; set; }
        public object departureAirport { get; set; }
        public object arrivalAirport { get; set; }
        public object departureTime { get; set; }
        public object arrivalTime { get; set; }
        public object airline { get; set; }
        public object flightNumber { get; set; }
        public int duration { get; set; }
        public int price { get; set; }
        public object currency { get; set; }
        public int stops { get; set; }
    }

    public class Baggagepolicy
    {
        public object airline { get; set; }
        public object policy { get; set; }
    }
}


