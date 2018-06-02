
/****************************** ghost1372.github.io ******************************\
*	Module Name:	Azmon.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 1, 06:13 ب.ظ
*	
***********************************************************************************/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Azmon.xaml
    /// </summary>
    public partial class Azmon : UserControl
    {
        internal static Azmon main;

        public Azmon()
        {
            InitializeComponent();
            this.DataContext = this;
            main = this;
            getHint();
        }

        public async static Task<List<DataClass.Tables.Group>> GetAllGroupAsync()
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Groups.Select(x => x);
                return await query.ToListAsync();
            }
        }
     
        public void getHint()
        {
            try
            {
                var query = GetAllGroupAsync();
                query.Wait();


                List<DataClass.Tables.Group> data = query.Result;
                using (var db = new DataClass.myDbContext())
                {
                    var query2 = db.AQuestions.Count();
                    exAddorUpdateQuestion.Hint = query2.ToString();

                }
                exAddorUpdateGroup.Hint = data.Count().ToString();
            }
            catch (Exception)
            {
            }
          
        }
        private void exActivity_Click(object sender, EventArgs e)
        {

        }

        private void exAddorUpdateGroup_Click(object sender, EventArgs e)
        {
            exContent.Content = new AddAzmonGroup();

        }

        private void exAddorUpdateQuestion_Click(object sender, EventArgs e)
        {
            exContent.Content = new AddQuestions();

        }

        private void exAzmon_Click(object sender, EventArgs e)
        {
            exContent.Content = new StartAzmon();

        }

        private void exHistory_Click(object sender, EventArgs e)
        {
            exContent.Content = new AzmonHistory();
        }
    }
}
