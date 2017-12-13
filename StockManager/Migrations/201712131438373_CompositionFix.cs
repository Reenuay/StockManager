namespace StockManager.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CompositionFix : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KeywordCompositions",
                c => new
                    {
                        Keyword_Id = c.Int(nullable: false),
                        Composition_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Keyword_Id, t.Composition_Id })
                .ForeignKey("dbo.Keywords", t => t.Keyword_Id, cascadeDelete: true)
                .ForeignKey("dbo.Compositions", t => t.Composition_Id, cascadeDelete: true)
                .Index(t => t.Keyword_Id)
                .Index(t => t.Composition_Id);

            AddColumn("dbo.Compositions", "Name", c => c.String(nullable: false, maxLength: 4000));
        }

        public override void Down()
        {
            DropForeignKey("dbo.KeywordCompositions", "Composition_Id", "dbo.Compositions");
            DropForeignKey("dbo.KeywordCompositions", "Keyword_Id", "dbo.Keywords");
            DropIndex("dbo.KeywordCompositions", new[] { "Composition_Id" });
            DropIndex("dbo.KeywordCompositions", new[] { "Keyword_Id" });
            DropColumn("dbo.Compositions", "Name");
            DropTable("dbo.KeywordCompositions");
        }
    }
}
