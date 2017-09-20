namespace StockManager.Migrations
{
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Model;
    using System.Data.Entity.SqlServerCompact;

    internal sealed class Configuration : DbMigrationsConfiguration<StockManager.Models.StockManagerContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("System.Data.SqlServerCe.4.0", new StockManagerSqlMigrationSqlGenerator());
        }
    }

    internal class StockManagerSqlMigrationSqlGenerator : SqlCeMigrationSqlGenerator
    {
        protected override void Generate(AddColumnOperation addColumnOperation)
        {
            SetDateCreatedColumn(addColumnOperation.Column);
            base.Generate(addColumnOperation);
        }

        protected override void Generate(CreateTableOperation createTableOperation)
        {
            SetDateCreatedColumn(createTableOperation.Columns);
            base.Generate(createTableOperation);
        }

        private static void SetDateCreatedColumn(IEnumerable<ColumnModel> columns)
        {
            foreach (ColumnModel columnModel in columns)
            {
                SetDateCreatedColumn(columnModel);
            }
        }

        private static void SetDateCreatedColumn(PropertyModel column)
        {
            if (column.Name == "DateCreated")
            {
                column.DefaultValueSql = "getdate()";
            }
        }
    }
}
