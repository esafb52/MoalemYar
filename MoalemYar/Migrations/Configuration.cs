namespace MoalemYar.Migrations
{
    using SQLite.CodeFirst;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<MoalemYar.DataClass.myDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            SetSqlGenerator("System.Data.SQLite", new SqliteMigrationSqlGenerator());
        }

        protected override void Seed(MoalemYar.DataClass.myDbContext context)
        {
            context.Set<DataClass.Tables.User>().Add(new DataClass.Tables.User
            {
                Username = "test",
                Password = "test"
            });

            context.SaveChanges();
        }
    }
}