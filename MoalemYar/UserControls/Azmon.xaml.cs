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
using System.Linq;
using System.Windows.Controls;

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

        public void getHint()
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Groups.Count();
                    exAddorUpdateGroup.Hint = query.ToString();
                    var query2 = db.AQuestions.Count();
                    exAddorUpdateQuestion.Hint = query2.ToString();
                }
            }
            catch (Exception)
            {
            }
        }

        private void exAddorUpdateGroup_Click(object sender, EventArgs e)
        {
            if (exContent.Content == null)
                exContent.Content = new AddAzmonGroup();
            else if (!exContent.Content.ToString().Equals("MoalemYar.UserControls.AddAzmonGroup"))
                exContent.Content = new AddAzmonGroup();
        }

        private void exAddorUpdateQuestion_Click(object sender, EventArgs e)
        {
            if (exContent.Content == null)
                exContent.Content = new AddQuestions();
            else if (!exContent.Content.ToString().Equals("MoalemYar.UserControls.AddQuestions"))
                exContent.Content = new AddQuestions();
        }

        private void exAzmon_Click(object sender, EventArgs e)
        {
            if (exContent.Content == null)
                exContent.Content = new StartAzmon();
            else if (!exContent.Content.ToString().Equals("MoalemYar.UserControls.StartAzmon"))
                exContent.Content = new StartAzmon();
        }

        private void exHistory_Click(object sender, EventArgs e)
        {
            if (exContent.Content == null)
                exContent.Content = new AzmonHistory();
            else if (!exContent.Content.ToString().Equals("MoalemYar.UserControls.AzmonHistory"))
                exContent.Content = new AzmonHistory();
        }
    }
}