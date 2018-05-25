namespace MoalemYar.Migrations
{
    using SQLite.CodeFirst;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MoalemYar.DataClass.myDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            SetSqlGenerator("System.Data.SQLite", new SqliteMigrationSqlGenerator());
        }

        protected override void Seed(MoalemYar.DataClass.myDbContext context)
        {
            if (context.Set<DataClass.Tables.User>().Count() != 0)
            {
                return;
            }
            context.Set<DataClass.Tables.User>().Add(new DataClass.Tables.User
            {
                Username = "test",
                Password = "test"
            });

            context.SaveChanges();
        }
    }
}