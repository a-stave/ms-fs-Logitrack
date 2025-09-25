using Microsoft.EntityFrameworkCore;
using LogiTrack;

public class LogiTrackContext : DbContext
{
    public LogiTrackContext(DbContextOptions<LogiTrackContext> options): base(options)
    {
    }

    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<Order> Orders { get; set; }
}