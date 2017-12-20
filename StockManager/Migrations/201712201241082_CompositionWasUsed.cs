namespace StockManager.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CompositionWasUsed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Compositions", "WasUsed", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Compositions", "WasUsed");
        }
    }
}
