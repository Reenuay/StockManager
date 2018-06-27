namespace StockManager.Migrations {
    using System.Data.Entity.Migrations;

    public partial class TemplateHidden : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Templates", "IsHidden", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Templates", "IsHidden");
        }
    }
}
