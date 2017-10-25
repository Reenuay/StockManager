namespace StockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class LogEntry : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogEntries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CallSite = c.String(maxLength: 4000),
                        Date = c.String(maxLength: 4000),
                        Exception = c.String(maxLength: 4000),
                        Level = c.String(maxLength: 4000),
                        Logger = c.String(maxLength: 4000),
                        MachineName = c.String(maxLength: 4000),
                        Message = c.String(maxLength: 4000),
                        StackTrace = c.String(maxLength: 4000),
                        Thread = c.String(maxLength: 4000),
                        Username = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);

        }

        public override void Down()
        {
            DropTable("dbo.LogEntries");
        }
    }
}
