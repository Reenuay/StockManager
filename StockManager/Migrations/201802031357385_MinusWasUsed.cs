namespace StockManager.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class MinusWasUsed : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Compositions", "WasUsed");
        }

        public override void Down()
        {
            AddColumn("dbo.Compositions", "WasUsed", c => c.Boolean(nullable: false));
        }
    }
}
