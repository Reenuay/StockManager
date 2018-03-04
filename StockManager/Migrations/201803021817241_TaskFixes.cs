namespace StockManager.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class TaskFixes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.QueueTasks", "Queue_Id", "dbo.Queues");
            DropForeignKey("dbo.QueueTasks", "Task_Id", "dbo.Tasks");
            DropForeignKey("dbo.Tasks", "BackgroundId", "dbo.Backgrounds");
            DropIndex("dbo.Tasks", new[] { "BackgroundId" });
            DropIndex("dbo.QueueTasks", new[] { "Queue_Id" });
            DropIndex("dbo.QueueTasks", new[] { "Task_Id" });
            AddColumn("dbo.Tasks", "QueueId", c => c.Int(nullable: false));
            AlterColumn("dbo.Tasks", "BackgroundId", c => c.Int());
            CreateIndex("dbo.Tasks", "BackgroundId");
            CreateIndex("dbo.Tasks", "QueueId");
            AddForeignKey("dbo.Tasks", "QueueId", "dbo.Queues", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tasks", "BackgroundId", "dbo.Backgrounds", "Id");
            DropTable("dbo.QueueTasks");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.QueueTasks",
                c => new
                    {
                        Queue_Id = c.Int(nullable: false),
                        Task_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Queue_Id, t.Task_Id });

            DropForeignKey("dbo.Tasks", "BackgroundId", "dbo.Backgrounds");
            DropForeignKey("dbo.Tasks", "QueueId", "dbo.Queues");
            DropIndex("dbo.Tasks", new[] { "QueueId" });
            DropIndex("dbo.Tasks", new[] { "BackgroundId" });
            AlterColumn("dbo.Tasks", "BackgroundId", c => c.Int(nullable: false));
            DropColumn("dbo.Tasks", "QueueId");
            CreateIndex("dbo.QueueTasks", "Task_Id");
            CreateIndex("dbo.QueueTasks", "Queue_Id");
            CreateIndex("dbo.Tasks", "BackgroundId");
            AddForeignKey("dbo.Tasks", "BackgroundId", "dbo.Backgrounds", "Id", cascadeDelete: true);
            AddForeignKey("dbo.QueueTasks", "Task_Id", "dbo.Tasks", "Id", cascadeDelete: true);
            AddForeignKey("dbo.QueueTasks", "Queue_Id", "dbo.Queues", "Id", cascadeDelete: true);
        }
    }
}
