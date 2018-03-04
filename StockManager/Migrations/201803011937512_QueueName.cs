namespace StockManager.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class QueueName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Queues", "Name", c => c.String(nullable: false, maxLength: 4000));
            CreateIndex("dbo.Queues", "Name", unique: true);
        }

        public override void Down()
        {
            DropIndex("dbo.Queues", new[] { "Name" });
            DropColumn("dbo.Queues", "Name");
        }
    }
}
