namespace StockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class ReorganizeHierarchy : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Icons", "DateChanged", c => c.DateTime());
            DropColumn("dbo.Keywords", "DateChanged");
        }

        public override void Down()
        {
            AddColumn("dbo.Keywords", "DateChanged", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Icons", "DateChanged", c => c.DateTime(nullable: false));
        }
    }
}
