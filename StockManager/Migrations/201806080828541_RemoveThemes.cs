namespace StockManager.Migrations {
    using System.Data.Entity.Migrations;

    public partial class RemoveThemes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ThemeKeywords", "Theme_Id", "dbo.Themes");
            DropForeignKey("dbo.ThemeKeywords", "Keyword_Id", "dbo.Keywords");
            DropForeignKey("dbo.Compositions", "ThemeId", "dbo.Themes");
            DropForeignKey("dbo.Tasks", "ThemeId", "dbo.Themes");
            DropIndex("dbo.Compositions", new[] { "ThemeId" });
            DropIndex("dbo.Themes", new[] { "Name" });
            DropIndex("dbo.Tasks", new[] { "ThemeId" });
            DropIndex("dbo.ThemeKeywords", new[] { "Theme_Id" });
            DropIndex("dbo.ThemeKeywords", new[] { "Keyword_Id" });
            DropColumn("dbo.Compositions", "ThemeId");
            DropColumn("dbo.Tasks", "ThemeId");
            DropTable("dbo.Themes");
            DropTable("dbo.ThemeKeywords");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.ThemeKeywords",
                c => new
                    {
                        Theme_Id = c.Int(nullable: false),
                        Keyword_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Theme_Id, t.Keyword_Id });

            CreateTable(
                "dbo.Themes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.Tasks", "ThemeId", c => c.Int(nullable: false));
            AddColumn("dbo.Compositions", "ThemeId", c => c.Int(nullable: false));
            CreateIndex("dbo.ThemeKeywords", "Keyword_Id");
            CreateIndex("dbo.ThemeKeywords", "Theme_Id");
            CreateIndex("dbo.Tasks", "ThemeId");
            CreateIndex("dbo.Themes", "Name", unique: true);
            CreateIndex("dbo.Compositions", "ThemeId");
            AddForeignKey("dbo.Tasks", "ThemeId", "dbo.Themes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Compositions", "ThemeId", "dbo.Themes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ThemeKeywords", "Keyword_Id", "dbo.Keywords", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ThemeKeywords", "Theme_Id", "dbo.Themes", "Id", cascadeDelete: true);
        }
    }
}
