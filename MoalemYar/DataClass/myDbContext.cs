/****************************** ghost1372.github.io ******************************\
*	Module Name:	myDbContext.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 28, 11:56 ق.ظ
*
***********************************************************************************/

using MoalemYar.DataClass.Tables;
using SQLite.CodeFirst;
using System.Data.Entity;

namespace MoalemYar.DataClass
{
    public class myDbContext : DbContext
    {
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<School> Schools { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public myDbContext()
       : base("default")
        {
            //base.Database.Connection.ConnectionString = @"data source=" + AppVariable.myPath + @"database\data.db;";
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new MyDbContextInitializer(modelBuilder);

            Database.SetInitializer(sqliteConnectionInitializer);
            // Database.CreateIfNotExists();
        }

        public class MyDbContextInitializer : SqliteDropCreateDatabaseWhenModelChanges<myDbContext>
        {
            public MyDbContextInitializer(DbModelBuilder modelBuilder)
                : base(modelBuilder)
            {
            }

            protected override void Seed(myDbContext context)
            {
                //context.Users.Add(new User
                //{
                //    Username = "admin",
                //    Password = "1"
                //});

                //base.Seed(context);
            }
        }
    }
}