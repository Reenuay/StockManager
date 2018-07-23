namespace StockManager.Migrations {
    using System.Data.Entity.Migrations;

    public partial class CompositionThemeAndColors : DbMigration {
        public override void Up() {
            AddColumn("dbo.Compositions", "ThemeId", c => c.Int());
            AddColumn("dbo.Compositions", "ColorId", c => c.Int());
            CreateIndex("dbo.Compositions", "ThemeId");
            CreateIndex("dbo.Compositions", "ColorId");
            AddForeignKey("dbo.Compositions", "ColorId", "dbo.Colors", "Id");
            AddForeignKey("dbo.Compositions", "ThemeId", "dbo.Keywords", "Id");
        }

        public override void Down() {
            DropForeignKey("dbo.Compositions", "ThemeId", "dbo.Keywords");
            DropForeignKey("dbo.Compositions", "ColorId", "dbo.Colors");
            DropIndex("dbo.Compositions", new[] { "ColorId" });
            DropIndex("dbo.Compositions", new[] { "ThemeId" });
            DropColumn("dbo.Compositions", "ColorId");
            DropColumn("dbo.Compositions", "ThemeId");
        }
    }
}
