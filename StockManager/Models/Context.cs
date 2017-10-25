using System.Data.Entity;
using EntityFramework.Triggers;

namespace StockManager.Models
{
    /// <summary>
    /// Являет собой контекст базы данных приложения.
    /// </summary>
    class Context : DbContextWithTriggers
    {
        public DbSet<Icon> Icons { get; set; }

        public DbSet<Keyword> Keywords { get; set; }

        public DbSet<LogEntry> LogEntries { get; set; }
    }
}
