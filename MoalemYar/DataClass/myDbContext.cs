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
using System.Data.Entity.Migrations;

namespace MoalemYar.DataClass
{
    public class myDbContext : DbContext
    {
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<School> Schools { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Attendance> Attendances { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Score> Scores { get; set; }

        public myDbContext()
       : base("default") {}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<myDbContext, MyDbContextInitializer>(true));
        }

        internal sealed class MyDbContextInitializer : DbMigrationsConfiguration<myDbContext>
        {
            public MyDbContextInitializer()
            {
                AutomaticMigrationsEnabled = true;

                // This command alter the class to support Migration to SQLite. 
                SetSqlGenerator("System.Data.SQLite", new SqliteMigrationSqlGenerator());
            }

            protected override void Seed(myDbContext context)
            {
                //  This method will be called after migrating to the latest version.
            }
        }
    }
}