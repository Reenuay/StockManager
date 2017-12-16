namespace StockManager.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class NullableBackground : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Compositions", "BackgroundId", "dbo.Backgrounds");
            DropIndex("dbo.Compositions", new[] { "BackgroundId" });
            AlterColumn("dbo.Compositions", "BackgroundId", c => c.Int());
            CreateIndex("dbo.Compositions", "BackgroundId");
            AddForeignKey("dbo.Compositions", "BackgroundId", "dbo.Backgrounds", "Id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Compositions", "BackgroundId", "dbo.Backgrounds");
            DropIndex("dbo.Compositions", new[] { "BackgroundId" });
            AlterColumn("dbo.Compositions", "BackgroundId", c => c.Int(nullable: false));
            CreateIndex("dbo.Compositions", "BackgroundId");
            AddForeignKey("dbo.Compositions", "BackgroundId", "dbo.Backgrounds", "Id", cascadeDelete: true);
        }
    }
}
