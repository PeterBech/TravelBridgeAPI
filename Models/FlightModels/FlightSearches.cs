namespace TravelBridgeAPI.Models.FlightModels.FlightSearches
{

    public class Rootobject
    {
        public bool status { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public FlightOffer[] flightOffers { get; set; }
    }

    public class FlightOffer
    {
        public string token { get; set; }
        public Segment1[] segments { get; set; }
        public Pricebreakdown priceBreakdown { get; set; }
    }

    public class Segment1
    {
        public DepartureAirport departureAirport { get; set; }
        public ArrivalAirport arrivalAirport { get; set; }
        public DateTime departureTime { get; set; }
        public DateTime arrivalTime { get; set; }
    }

    public class DepartureAirport
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class ArrivalAirport
    {
        public string code { get; set; }
        public string name { get; set; }
    }

    public class Pricebreakdown
    {
        public Total total { get; set; }
    }

    public class Total
    {
        public string currencyCode { get; set; }
        public int units { get; set; }
        public int nanos { get; set; }
    }


}


