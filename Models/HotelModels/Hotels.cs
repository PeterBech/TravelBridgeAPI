namespace TravelBridgeAPI.Models.HotelModels
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
        public Hotel[] hotels { get; set; }
    }

    public class Hotel
    {
        public int hotel_id { get; set; }
        public string accessibilityLabel { get; set; }
        public Property property { get; set; }
    }

    public class Property
    {
        public string name { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public string[] photoUrls { get; set; }
        public PriceBreakdown priceBreakdown { get; set; }
    }

    public class PriceBreakdown
    {
        public GrossPrice grossPrice { get; set; }
    }

    public class GrossPrice
    {
        public string currency { get; set; }
        public float value { get; set; }
    }
}
