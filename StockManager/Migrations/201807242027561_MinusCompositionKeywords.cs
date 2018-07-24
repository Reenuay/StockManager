namespace StockManager.Migrations {
    using System.Data.Entity.Migrations;

    public partial class MinusCompositionKeywords : DbMigration {
        public override void Up() {
            DropForeignKey("dbo.KeywordCompositions", "Keyword_Id", "dbo.Keywords");
            DropForeignKey("dbo.KeywordCompositions", "Composition_Id", "dbo.Compositions");
            DropIndex("dbo.KeywordCompositions", new[] { "Keyword_Id" });
            DropIndex("dbo.KeywordCompositions", new[] { "Composition_Id" });
            DropTable("dbo.KeywordCompositions");
        }

        public override void Down() {
            CreateTable(
                "dbo.KeywordCompositions",
                c => new
                {
                    Keyword_Id = c.Int(nullable: false),
                    Composition_Id = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.Keyword_Id, t.Composition_Id });

            CreateIndex("dbo.KeywordCompositions", "Composition_Id");
            CreateIndex("dbo.KeywordCompositions", "Keyword_Id");
            AddForeignKey("dbo.KeywordCompositions", "Composition_Id", "dbo.Compositions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.KeywordCompositions", "Keyword_Id", "dbo.Keywords", "Id", cascadeDelete: true);
        }
    }
}
