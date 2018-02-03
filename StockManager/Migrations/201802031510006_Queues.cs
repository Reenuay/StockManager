namespace StockManager.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Queues : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Queues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateChanged = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.QueueTasks",
                c => new
                    {
                        Queue_Id = c.Int(nullable: false),
                        Task_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Queue_Id, t.Task_Id })
                .ForeignKey("dbo.Queues", t => t.Queue_Id, cascadeDelete: true)
                .ForeignKey("dbo.Tasks", t => t.Task_Id, cascadeDelete: true)
                .Index(t => t.Queue_Id)
                .Index(t => t.Task_Id);

        }

        public override void Down()
        {
            DropForeignKey("dbo.QueueTasks", "Task_Id", "dbo.Tasks");
            DropForeignKey("dbo.QueueTasks", "Queue_Id", "dbo.Queues");
            DropIndex("dbo.QueueTasks", new[] { "Task_Id" });
            DropIndex("dbo.QueueTasks", new[] { "Queue_Id" });
            DropTable("dbo.QueueTasks");
            DropTable("dbo.Queues");
        }
    }
}
