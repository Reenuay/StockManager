namespace StockManager.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Tasks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ThemeId = c.Int(nullable: false),
                        TemplateId = c.Int(nullable: false),
                        BackgroundId = c.Int(nullable: false),
                        Percentage = c.Int(nullable: false),
                        Maximum = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Backgrounds", t => t.BackgroundId, cascadeDelete: true)
                .ForeignKey("dbo.Templates", t => t.TemplateId, cascadeDelete: true)
                .ForeignKey("dbo.Themes", t => t.ThemeId, cascadeDelete: true)
                .Index(t => t.ThemeId)
                .Index(t => t.TemplateId)
                .Index(t => t.BackgroundId);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "ThemeId", "dbo.Themes");
            DropForeignKey("dbo.Tasks", "TemplateId", "dbo.Templates");
            DropForeignKey("dbo.Tasks", "BackgroundId", "dbo.Backgrounds");
            DropIndex("dbo.Tasks", new[] { "BackgroundId" });
            DropIndex("dbo.Tasks", new[] { "TemplateId" });
            DropIndex("dbo.Tasks", new[] { "ThemeId" });
            DropTable("dbo.Tasks");
        }
    }
}
