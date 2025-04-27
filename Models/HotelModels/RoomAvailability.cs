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
        public string currency { get; set; }

        // Liste af små Dictionarys
        public List<Dictionary<string, int>> lengthsOfStay { get; set; }

        public List<Dictionary<string, float>> avDates { get; set; }
    }
}
