namespace StockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_DateChanged : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Icons", "DateChanged", c => c.DateTime(nullable: false));
            AddColumn("dbo.Keywords", "DateChanged", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Keywords", "DateChanged");
            DropColumn("dbo.Icons", "DateChanged");
        }
    }
}
