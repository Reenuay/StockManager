namespace StockManager.Migrations {
    using System.Data.Entity.Migrations;

    public partial class RemoveQueues : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tasks", "BackgroundId", "dbo.Backgrounds");
            DropForeignKey("dbo.Tasks", "QueueId", "dbo.Queues");
            DropForeignKey("dbo.Tasks", "TemplateId", "dbo.Templates");
            DropIndex("dbo.Tasks", new[] { "TemplateId" });
            DropIndex("dbo.Tasks", new[] { "BackgroundId" });
            DropIndex("dbo.Tasks", new[] { "QueueId" });
            DropIndex("dbo.Queues", new[] { "Name" });
            DropTable("dbo.Tasks");
            DropTable("dbo.Queues");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.Queues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 4000),
                        DateChanged = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TemplateId = c.Int(nullable: false),
                        BackgroundId = c.Int(),
                        QueueId = c.Int(nullable: false),
                        Percentage = c.Int(nullable: false),
                        Maximum = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateIndex("dbo.Queues", "Name", unique: true);
            CreateIndex("dbo.Tasks", "QueueId");
            CreateIndex("dbo.Tasks", "BackgroundId");
            CreateIndex("dbo.Tasks", "TemplateId");
            AddForeignKey("dbo.Tasks", "TemplateId", "dbo.Templates", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tasks", "QueueId", "dbo.Queues", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Tasks", "BackgroundId", "dbo.Backgrounds", "Id");
        }
    }
}
