using Microsoft.EntityFrameworkCore;
using TrashBinTracker.Model;

namespace TrashBinTracker.Data
{
    public class TrashDbContext : DbContext
    {
        public TrashDbContext(DbContextOptions<TrashDbContext> options)
            : base(options)
        {
        }

        public DbSet<TrashBin> TrashBins { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EmptyHistory> EmptyHistory { get; set; }
    }
}