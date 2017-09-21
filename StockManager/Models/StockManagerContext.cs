using System.Data.Entity;

namespace StockManager.Models
{
    /// <summary>
    /// Являет собой контекст базы данных приложения.
    /// </summary>
    class StockManagerContext : DbContext
    {
        public DbSet<Icon> Icons { get; set; }

        public DbSet<Keyword> Keywords { get; set; }
    }
}
