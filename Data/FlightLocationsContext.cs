using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TravelBridgAPI.Models.FlightLocations;

namespace TravelBridgAPI.Data
{
    public class FlightLocationsContext : DbContext
    {
        public FlightLocationsContext (DbContextOptions<FlightLocationsContext> options)
            : base(options)
        {
        }

        public DbSet<TravelBridgAPI.Models.FlightLocations.Rootobject> Rootobject { get; set; } = default!;
    }
}
