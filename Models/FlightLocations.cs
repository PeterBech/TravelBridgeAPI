using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelBridgeAPI.Models.FlightLocations
{
    public class Rootobject
    {
        [Key]
        [MaxLength(50)]
        public string Keyword { get; set; } // Primary Key
        public bool status { get; set; }
        public string message { get; set; }
        public long timestamp { get; set; }

        // Navigate to the data property
        public ICollection<Datum> data { get; set; }
    }

    public class Datum
    {
        [Key]
        [MaxLength(50)]
        public string id { get; set; } // Primary Key
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

        // 1:1 relationship with the Distancetocity class
        public Distancetocity distanceToCity { get; set; }

        public string parent { get; set; }
        public string? region { get; set; } // Ændret til nullable type
        [ForeignKey("Rootobject")]
        public string Keyword { get; set; }
        public Rootobject rootobject { get; set; } // Navigation property
    }


    public class Distancetocity
    {
        [Key]
        public int Id { get; set; } // Internal PK (Auto-incremented)
        public float value { get; set; }
        public string unit { get; set; }

        // FK to Datum
        public string DatumId { get; set; }

        [ForeignKey("DatumId")]
        public Datum Datum { get; set; } // Navigation property
    }

}

