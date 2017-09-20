using System.Data.Entity;

namespace StockManager.Models
{
    class StockManagerContext : DbContext
    {
        public DbSet<Icon> Icons { get; set; }

        public DbSet<Keyword> Keywords { get; set; }
    }
}
