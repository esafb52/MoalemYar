/****************************** ghost1372.github.io ******************************\
*	Module Name:	Azmon.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 1, 06:13 ب.ظ
*
***********************************************************************************/

using System.Windows.Controls;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Azmon.xaml
    /// </summary>
    public partial class AzmonView : UserControl
    {
        internal static AzmonView main;

        public AzmonView()
        {
            InitializeComponent();
            this.DataContext = this;
            main = this;
        }

        private void TreeViewItem_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            var tree = sender as TreeViewItem;
            switch (tree.Tag)
            {
                case "exAddorUpdateGroup":
                    if (exContent.Content == null)
                        exContent.Content = new AddAzmonGroupView();
                    else if (!exContent.Content.ToString().Equals("MoalemYar.UserControls.AddAzmonGroup"))
                        exContent.Content = new AddAzmonGroupView();
                    break;

                case "exAddorUpdateQuestion":
                    if (exContent.Content == null)
                        exContent.Content = new AddQuestionsView();
                    else if (!exContent.Content.ToString().Equals("MoalemYar.UserControls.AddQuestions"))
                        exContent.Content = new AddQuestionsView();
                    break;

                case "exAzmon":
                    if (exContent.Content == null)
                        exContent.Content = new StartAzmonView();
                    else if (!exContent.Content.ToString().Equals("MoalemYar.UserControls.StartAzmon"))
                        exContent.Content = new StartAzmonView();
                    break;

                case "exHistory":
                    if (exContent.Content == null)
                        exContent.Content = new AzmonHistoryView();
                    else if (!exContent.Content.ToString().Equals("MoalemYar.UserControls.AzmonHistory"))
                        exContent.Content = new AzmonHistoryView();
                    break;
            }
        }
    }
}