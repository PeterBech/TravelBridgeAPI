using System.Text.Json.Serialization;

namespace TravelBridgeAPI.Models.HotelModels.HotelDetails
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
        public int hotel_id { get; set; }
        public string hotel_name { get; set; }
        public string url { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string countrycode { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int review_nr { get; set; }
        public string currency_code { get; set; }
        public RawData rawData { get; set; }
    }

    public class RawData
    {
        public string[] photoUrls { get; set; }
        public string checkinDate { get; set; }
        public string checkoutDate { get; set; }
        public string reviewScoreWord { get; set; }
        public int reviewScore { get; set; }
        public string currency { get; set; }
    }


}
