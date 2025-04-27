namespace TravelBridgeAPI.Models.HotelModels.RoomAvailability
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
        public Dictionary<string, int> lengthsOfStay { get; set; }
        public string currency { get; set; }
        public List<Avdate> avDates { get; set; }
    }

    public class Avdate
    {
        public Dictionary<string, float> PricesPerDate { get; set; }
    }

}
