namespace StockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Theme : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Themes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);

            CreateTable(
                "dbo.ThemeKeywords",
                c => new
                    {
                        Theme_Id = c.Int(nullable: false),
                        Keyword_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Theme_Id, t.Keyword_Id })
                .ForeignKey("dbo.Themes", t => t.Theme_Id, cascadeDelete: true)
                .ForeignKey("dbo.Keywords", t => t.Keyword_Id, cascadeDelete: true)
                .Index(t => t.Theme_Id)
                .Index(t => t.Keyword_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.ThemeKeywords", "Keyword_Id", "dbo.Keywords");
            DropForeignKey("dbo.ThemeKeywords", "Theme_Id", "dbo.Themes");
            DropIndex("dbo.ThemeKeywords", new[] { "Keyword_Id" });
            DropIndex("dbo.ThemeKeywords", new[] { "Theme_Id" });
            DropIndex("dbo.Themes", new[] { "Name" });
            DropTable("dbo.ThemeKeywords");
            DropTable("dbo.Themes");
        }
    }
}
