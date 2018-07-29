namespace StockManager.Migrations {
    using System.Data.Entity.Migrations;

    public partial class IconKeywordsWithPriorities : DbMigration {
        public override void Up() {
            DropForeignKey("dbo.IconKeywords", "Icon_Id", "dbo.Icons");
            DropForeignKey("dbo.IconKeywords", "Keyword_Id", "dbo.Keywords");
            DropIndex("dbo.IconKeywords", new[] { "Icon_Id" });
            DropIndex("dbo.IconKeywords", new[] { "Keyword_Id" });
            CreateTable(
                "dbo.IconKeywords",
                c => new
                {
                    IconId = c.Int(nullable: false),
                    KeywordId = c.Int(nullable: false),
                    Priority = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.IconId, t.KeywordId })
                .ForeignKey("dbo.Icons", t => t.IconId, cascadeDelete: true)
                .ForeignKey("dbo.Keywords", t => t.KeywordId, cascadeDelete: true)
                .Index(t => new { t.IconId, t.KeywordId }, unique: true, name: "UniqueIconKeyword")
                .Index(t => new { t.IconId, t.KeywordId, t.Priority }, unique: true, name: "UniqueIconKeywordPriority");

            DropTable("dbo.IconKeywords");
        }

        public override void Down() {
            CreateTable(
                "dbo.IconKeywords",
                c => new
                {
                    Icon_Id = c.Int(nullable: false),
                    Keyword_Id = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.Icon_Id, t.Keyword_Id });

            DropForeignKey("dbo.IconKeywords", "KeywordId", "dbo.Keywords");
            DropForeignKey("dbo.IconKeywords", "IconId", "dbo.Icons");
            DropIndex("dbo.IconKeywords", "UniqueIconKeywordPriority");
            DropIndex("dbo.IconKeywords", "UniqueIconKeyword");
            DropTable("dbo.IconKeywords");
            CreateIndex("dbo.IconKeywords", "Keyword_Id");
            CreateIndex("dbo.IconKeywords", "Icon_Id");
            AddForeignKey("dbo.IconKeywords", "Keyword_Id", "dbo.Keywords", "Id", cascadeDelete: true);
            AddForeignKey("dbo.IconKeywords", "Icon_Id", "dbo.Icons", "Id", cascadeDelete: true);
        }
    }
}
