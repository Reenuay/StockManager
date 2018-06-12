namespace StockManager.Migrations {
    using System.Data.Entity.Migrations;

    public partial class RemoveIconName : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Icons", "Name");
        }

        public override void Down()
        {
            AddColumn("dbo.Icons", "Name", c => c.String(nullable: false, maxLength: 4000));
        }
    }
}
