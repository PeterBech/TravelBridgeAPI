using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TravelBridgeAPI.Models.FlightModels.FlightLocations
{
    public class Rootobject
    {
        [JsonIgnore]
        public string Keyword { get; set; } // Part of the composite PK
        [JsonIgnore]
        public string Language { get; set; } // Part of the composite PK

        public bool status { get; set; }
        public string message { get; set; }
        public long timestamp { get; set; }

        public ICollection<Datum> data { get; set; }
    }

    public class Datum
    {
        [Key]
        [JsonIgnore]
        public int DataId { get; set; } // Primary Key (Auto-incremented)

        public string id { get; set; }
        public string? type { get; set; }
        public string? name { get; set; }
        public string? code { get; set; }
        public string? city { get; set; }
        public string? cityName { get; set; }
        public string? regionName { get; set; }
        public string? country { get; set; }
        public string? countryName { get; set; }
        public string? countryNameShort { get; set; }
        public string? photoUri { get; set; }

        public Distancetocity? distanceToCity { get; set; }
        public string? parent { get; set; }
        public string? region { get; set; }

        [JsonIgnore]
        [ForeignKey("Rootobject")]
        public string Keyword { get; set; } // Foreign key part of the composite PK

        [JsonIgnore]
        [ForeignKey("Rootobject")]
        public string Language { get; set; } // Foreign key part of the composite PK

        [JsonIgnore]
        public Rootobject rootobject { get; set; } // Navigation property
    }

    public class Distancetocity
    {
        [Key]
        [JsonIgnore]
        public int Id { get; set; } // Internal PK (Auto-incremented)

        public float? value { get; set; }
        public string? unit { get; set; }

        [JsonIgnore]
        public int DatumId { get; set; }

        [ForeignKey("DatumId")]
        [JsonIgnore]
        public Datum Datum { get; set; } // Navigation property
    }
}
