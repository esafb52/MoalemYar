
/****************************** ghost1372.github.io ******************************\
*	Module Name:	MaterialCircular.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 2, 01:57 ب.ظ
*	
***********************************************************************************/
using System;
using System.Collections.Generic;
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
    /// Interaction logic for MaterialCircular.xaml
    /// </summary>
    public partial class MaterialCircular : UserControl
    {
        public Brush BorderColor { get; set; }

        public MaterialCircular(string Row ,string Title, string Category, string Type, string SubType, string Date, Brush Background)
        {
            InitializeComponent();
            DataContext = this;
            BorderColor = Background;
            txtCategory.Text = Category;
            txtDate.Text = Date;
            txtSubType.Text = SubType;
            txtTitle.Text = Title;
            txtType.Text = Type;
            txtRow.Text = Row;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
