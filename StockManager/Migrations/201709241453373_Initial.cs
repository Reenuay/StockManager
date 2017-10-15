namespace StockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Icons",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CheckSum = c.String(nullable: false, maxLength: 32),
                        FullPath = c.String(nullable: false, maxLength: 4000),
                        IsDeleted = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CheckSum, unique: true);

            CreateTable(
                "dbo.Keywords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 4000),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);

            CreateTable(
                "dbo.KeywordIcons",
                c => new
                    {
                        Keyword_Id = c.Int(nullable: false),
                        Icon_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Keyword_Id, t.Icon_Id })
                .ForeignKey("dbo.Keywords", t => t.Keyword_Id, cascadeDelete: true)
                .ForeignKey("dbo.Icons", t => t.Icon_Id, cascadeDelete: true)
                .Index(t => t.Keyword_Id)
                .Index(t => t.Icon_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.KeywordIcons", "Icon_Id", "dbo.Icons");
            DropForeignKey("dbo.KeywordIcons", "Keyword_Id", "dbo.Keywords");
            DropIndex("dbo.KeywordIcons", new[] { "Icon_Id" });
            DropIndex("dbo.KeywordIcons", new[] { "Keyword_Id" });
            DropIndex("dbo.Keywords", new[] { "Name" });
            DropIndex("dbo.Icons", new[] { "CheckSum" });
            DropTable("dbo.KeywordIcons");
            DropTable("dbo.Keywords");
            DropTable("dbo.Icons");
        }
    }
}
