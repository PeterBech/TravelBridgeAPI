namespace TravelBridgeAPI.Models.FlightLocations
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
        public string id { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string city { get; set; }
        public string cityName { get; set; }
        public string regionName { get; set; }
        public string country { get; set; }
        public string countryName { get; set; }
        public string countryNameShort { get; set; }
        public string photoUri { get; set; }
        public Distancetocity distanceToCity { get; set; }
        public string parent { get; set; }
        public string region { get; set; }
    }

    public class Distancetocity
    {
        public float value { get; set; }
        public string unit { get; set; }
    }

}

