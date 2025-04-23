using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using TravelBridgeAPI.Models.FlightModels.FlightLocations;

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

            // Composite key for Rootobject
            modelBuilder.Entity<Rootobject>()
                .HasKey(r => new { r.Keyword, r.Language });

            // 1-to-many: Rootobject -> Datum (via composite FK)
            modelBuilder.Entity<Datum>()
                .HasOne(d => d.rootobject)
                .WithMany(r => r.data)
                .HasForeignKey(d => new { d.Keyword, d.Language })
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-1: Datum -> Distancetocity
            modelBuilder.Entity<Datum>()
                .HasOne(d => d.distanceToCity)
                .WithOne(dc => dc.Datum)
                .HasForeignKey<Distancetocity>(dc => dc.DatumId);

            // Specify auto-increment for Datum's primary key (dataId)
            modelBuilder.Entity<Datum>()
                .Property(d => d.DataId)
                .ValueGeneratedOnAdd(); // Auto-increment on dataId

            // Specify that DatumId in Distancetocity is a foreign key
            modelBuilder.Entity<Distancetocity>()
                .HasOne(dc => dc.Datum)
                .WithOne(d => d.distanceToCity)
                .HasForeignKey<Distancetocity>(dc => dc.DatumId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: set delete behavior based on your requirements
        }
    }
}
