namespace StockManager.Migrations
{
    using System.Data.Entity.Migrations;
    using StockManager.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    }
}
