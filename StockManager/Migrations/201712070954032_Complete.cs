namespace StockManager.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Complete : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Backgrounds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CheckSum = c.String(nullable: false, maxLength: 32),
                        FullPath = c.String(nullable: false, maxLength: 4000),
                        IsDeleted = c.Boolean(nullable: false),
                        DateChanged = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CheckSum, unique: true);

            CreateTable(
                "dbo.Cells",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        X = c.Single(nullable: false),
                        Y = c.Single(nullable: false),
                        Width = c.Single(nullable: false),
                        Height = c.Single(nullable: false),
                        TemplateId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Templates", t => t.TemplateId, cascadeDelete: true)
                .Index(t => t.TemplateId);

            CreateTable(
                "dbo.Templates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);

            CreateTable(
                "dbo.Compositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SetId = c.Int(nullable: false),
                        BackgroundId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Backgrounds", t => t.BackgroundId, cascadeDelete: true)
                .ForeignKey("dbo.Sets", t => t.SetId, cascadeDelete: true)
                .Index(t => t.SetId)
                .Index(t => t.BackgroundId);

            CreateTable(
                "dbo.Mappings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IconId = c.Int(nullable: false),
                        CellId = c.Int(nullable: false),
                        CompositionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Cells", t => t.CellId, cascadeDelete: true)
                .ForeignKey("dbo.Compositions", t => t.CompositionId, cascadeDelete: true)
                .ForeignKey("dbo.Icons", t => t.IconId, cascadeDelete: true)
                .Index(t => t.IconId)
                .Index(t => t.CellId)
                .Index(t => t.CompositionId);

            CreateTable(
                "dbo.Sets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Snapshot = c.String(nullable: false, maxLength: 32),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Snapshot, unique: true);

            CreateTable(
                "dbo.SetIcons",
                c => new
                    {
                        Set_Id = c.Int(nullable: false),
                        Icon_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Set_Id, t.Icon_Id })
                .ForeignKey("dbo.Sets", t => t.Set_Id, cascadeDelete: true)
                .ForeignKey("dbo.Icons", t => t.Icon_Id, cascadeDelete: true)
                .Index(t => t.Set_Id)
                .Index(t => t.Icon_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.Compositions", "SetId", "dbo.Sets");
            DropForeignKey("dbo.Mappings", "IconId", "dbo.Icons");
            DropForeignKey("dbo.SetIcons", "Icon_Id", "dbo.Icons");
            DropForeignKey("dbo.SetIcons", "Set_Id", "dbo.Sets");
            DropForeignKey("dbo.Mappings", "CompositionId", "dbo.Compositions");
            DropForeignKey("dbo.Mappings", "CellId", "dbo.Cells");
            DropForeignKey("dbo.Compositions", "BackgroundId", "dbo.Backgrounds");
            DropForeignKey("dbo.Cells", "TemplateId", "dbo.Templates");
            DropIndex("dbo.SetIcons", new[] { "Icon_Id" });
            DropIndex("dbo.SetIcons", new[] { "Set_Id" });
            DropIndex("dbo.Sets", new[] { "Snapshot" });
            DropIndex("dbo.Mappings", new[] { "CompositionId" });
            DropIndex("dbo.Mappings", new[] { "CellId" });
            DropIndex("dbo.Mappings", new[] { "IconId" });
            DropIndex("dbo.Compositions", new[] { "BackgroundId" });
            DropIndex("dbo.Compositions", new[] { "SetId" });
            DropIndex("dbo.Templates", new[] { "Name" });
            DropIndex("dbo.Cells", new[] { "TemplateId" });
            DropIndex("dbo.Backgrounds", new[] { "CheckSum" });
            DropTable("dbo.SetIcons");
            DropTable("dbo.Sets");
            DropTable("dbo.Mappings");
            DropTable("dbo.Compositions");
            DropTable("dbo.Templates");
            DropTable("dbo.Cells");
            DropTable("dbo.Backgrounds");
        }
    }
}
