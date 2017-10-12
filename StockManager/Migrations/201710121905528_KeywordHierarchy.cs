namespace StockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KeywordHierarchy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Keywords", "Parent_Id", c => c.Int());
            CreateIndex("dbo.Keywords", "Parent_Id");
            AddForeignKey("dbo.Keywords", "Parent_Id", "dbo.Keywords", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Keywords", "Parent_Id", "dbo.Keywords");
            DropIndex("dbo.Keywords", new[] { "Parent_Id" });
            DropColumn("dbo.Keywords", "Parent_Id");
        }
    }
}
