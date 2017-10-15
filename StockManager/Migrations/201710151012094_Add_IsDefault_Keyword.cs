namespace StockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class Add_IsDefault_Keyword : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Keywords", "IsDefault", c => c.Boolean(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Keywords", "IsDefault");
        }
    }
}
