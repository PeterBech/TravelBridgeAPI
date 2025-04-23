namespace TravelBridgeAPI.Models.HotelModels.HotelPhotos
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
        public int id { get; set; }
        public string url { get; set; }
    }
}


