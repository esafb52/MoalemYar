private static void CreateAndSeedDatabase(DbContext context)
        {
            context.Database.Initialize(true);
            context.Set<DataClass.Tables.User>().Add(new DataClass.Tables.User
            {
                Username = "test",
                Password = "test"
                //,
                //Coach = new Coach
                //{
                //    City = "Zürich",
                //    FirstName = "Masssaman",
                //    LastName = "Nachn",
                //    Street = "Testingstreet 844"
                //},
                //Players = new List<Player>
                //{
                //    new Player
                //    {
                //        City = "Bern",
                //        FirstName = "Marco",
                //        LastName = "Bürki",
                //        Street = "Wunderstrasse 43",
                //        Number = 12
                //    },
                //    new Player
                //    {
                //        City = "Berlin",
                //        FirstName = "Alain",
                //        LastName = "Rochat",
                //        Street = "Wonderstreet 13",
                //        Number = 14
                //    }
                //}
            });

            context.SaveChanges();

        }
