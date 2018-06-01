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
        public virtual DbSet<Attendance> Attendances { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Score> Scores { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<AQuestion> AQuestions { get; set; }

        public myDbContext()
       : base("default") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new SqliteMigrateDatabaseToLatestVersion<myDbContext, Migrations.Configuration>(modelBuilder, true));
        }
    }
}