using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelBridgAPI.Models.FlightLocations
{
    public class Rootobject
    {
        [Key]
        public string Keyword { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
        public long timestamp { get; set; }
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        [Key]
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
        //public Distancetocity distanceToCity { get; set; }
        public string parent { get; set; }
        public string region { get; set; }

        public string RootobjectKeyword { get; set; }
        [ForeignKey("RootobjectKeyword")]
        public Rootobject Rootobject { get; set; }

        public Distancetocity DistanceToCity { get; set; }
    }

    public class Distancetocity
    {
        [Key]
        public int Id { get; set; }
        public float value { get; set; }
        public string unit { get; set; }

        public string DatumId { get; set; }
        [ForeignKey("DatumId")]
        public Datum Datum { get; set; }
    }
}

