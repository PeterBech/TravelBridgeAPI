using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TravelBridgeAPI.Models.FlightLocations;

namespace TravelBridgeAPI.Data
{
    public class FlightLocationsContext : DbContext
    {
        public FlightLocationsContext(DbContextOptions<FlightLocationsContext> options) : base(options) { }

        public DbSet<Rootobject> Rootobjects { get; set; }
        public DbSet<Datum> Data { get; set; }
        public DbSet<Distancetocity> DistancesToCity { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relation: Rootobject -> Datum (1-many)
            modelBuilder.Entity<Rootobject>()
                .HasMany(r => r.data)
                .WithOne(d => d.rootobject)
                .HasForeignKey(d => d.Keyword)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation: Datum -> Distancetocity (1-1)
            modelBuilder.Entity<Datum>()
                .HasOne(d => d.distanceToCity)
                .WithOne(dc => dc.Datum)
                .HasForeignKey<Distancetocity>(dc => dc.DatumId);
        }
    }
}
