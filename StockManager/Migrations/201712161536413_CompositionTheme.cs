namespace StockManager.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CompositionTheme : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Compositions", "ThemeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Compositions", "ThemeId");
            AddForeignKey("dbo.Compositions", "ThemeId", "dbo.Themes", "Id", cascadeDelete: true);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Compositions", "ThemeId", "dbo.Themes");
            DropIndex("dbo.Compositions", new[] { "ThemeId" });
            DropColumn("dbo.Compositions", "ThemeId");
        }
    }
}
