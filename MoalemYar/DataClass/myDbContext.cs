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
        public virtual DbSet<Student> Student { get; set; }

        public myDbContext()
       : base("default")
        { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new MyDbContextInitializer(modelBuilder);

            Database.SetInitializer(sqliteConnectionInitializer);
        }

        public class MyDbContextInitializer : SqliteCreateDatabaseIfNotExists<myDbContext>
        {
            public MyDbContextInitializer(DbModelBuilder modelBuilder)
                : base(modelBuilder)
            {
            }

            protected override void Seed(myDbContext context)
            {
            }
        }
    }
}