namespace StockManager.Migrations {
    using System.Data.Entity.Migrations;

    public partial class Colors : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Colors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HEX = c.String(nullable: false, maxLength: 6),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.HEX, unique: true);

            CreateTable(
                "dbo.ColorBackgrounds",
                c => new
                    {
                        Color_Id = c.Int(nullable: false),
                        Background_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Color_Id, t.Background_Id })
                .ForeignKey("dbo.Colors", t => t.Color_Id, cascadeDelete: true)
                .ForeignKey("dbo.Backgrounds", t => t.Background_Id, cascadeDelete: true)
                .Index(t => t.Color_Id)
                .Index(t => t.Background_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.ColorBackgrounds", "Background_Id", "dbo.Backgrounds");
            DropForeignKey("dbo.ColorBackgrounds", "Color_Id", "dbo.Colors");
            DropIndex("dbo.ColorBackgrounds", new[] { "Background_Id" });
            DropIndex("dbo.ColorBackgrounds", new[] { "Color_Id" });
            DropIndex("dbo.Colors", new[] { "HEX" });
            DropTable("dbo.ColorBackgrounds");
            DropTable("dbo.Colors");
        }
    }
}
