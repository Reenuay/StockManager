namespace StockManager.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class IconName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Icons", "Name", c => c.String(nullable: false, maxLength: 4000));
        }

        public override void Down()
        {
            DropColumn("dbo.Icons", "Name");
        }
    }
}
