using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BikesRoadServer.Controllers
{
    public class RoadContext : DbContext
    {    
        public RoadContext(DbContextOptions<RoadContext> options) : base(options) { }
        public RoadContext()
        {            
        }        
        public DbSet<Road> RoadItems { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Road>().OwnsMany(typeof(Road.MapPoint), "MapPoints");
        }
    }
}
