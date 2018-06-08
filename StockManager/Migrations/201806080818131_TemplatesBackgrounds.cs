namespace StockManager.Migrations {
    using System.Data.Entity.Migrations;

    public partial class TemplatesBackgrounds : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TemplateBackgrounds",
                c => new
                    {
                        Template_Id = c.Int(nullable: false),
                        Background_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Template_Id, t.Background_Id })
                .ForeignKey("dbo.Templates", t => t.Template_Id, cascadeDelete: true)
                .ForeignKey("dbo.Backgrounds", t => t.Background_Id, cascadeDelete: true)
                .Index(t => t.Template_Id)
                .Index(t => t.Background_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.TemplateBackgrounds", "Background_Id", "dbo.Backgrounds");
            DropForeignKey("dbo.TemplateBackgrounds", "Template_Id", "dbo.Templates");
            DropIndex("dbo.TemplateBackgrounds", new[] { "Background_Id" });
            DropIndex("dbo.TemplateBackgrounds", new[] { "Template_Id" });
            DropTable("dbo.TemplateBackgrounds");
        }
    }
}
