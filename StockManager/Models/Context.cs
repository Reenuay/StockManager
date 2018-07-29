using System.Data.Entity;
using EntityFramework.Triggers;
using StockManager.Migrations;

namespace StockManager.Models {
    /// <summary>
    /// Являет собой контекст базы данных приложения.
    /// </summary>
    class Context : DbContextWithTriggers {
        public DbSet<Icon> Icons { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<IconKeyword> IconKeywords { get; set; }
        public DbSet<Set> Sets { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Cell> Cells { get; set; }
        public DbSet<Background> Backgrounds { get; set; }
        public DbSet<Mapping> Mappings { get; set; }
        public DbSet<Composition> Compositions { get; set; }
        public DbSet<Color> Colors { get; set; }

        public DbSet<LogEntry> LogEntries { get; set; }

        public Context() {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Context, Configuration>());
        }
    }
}
