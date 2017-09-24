namespace StockManager.Migrations
{
    using System.Data.Entity.Migrations;
    using StockManager.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<StockManagerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("System.Data.SqlServerCe.4.0", new StockManagerSqlMigrationSqlGenerator());
        }
    }
}
