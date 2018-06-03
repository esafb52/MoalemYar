/****************************** ghost1372.github.io ******************************\
*	Module Name:	Books.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 1, 11:28 ب.ظ
*
***********************************************************************************/

using System;
using System.Windows.Controls;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Books.xaml
    /// </summary>
    public partial class Books : UserControl
    {
        public Books()
        {
            InitializeComponent();
        }

        private void exEbtedayi_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.chap.sch.ir/school-books");
        }

        private void exMot1_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.chap.sch.ir/guide-books");
        }

        private void exMot2_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.chap.sch.ir/motevasete-list");
        }

        private void exJografi_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.chap.sch.ir/joghrafia-books");
        }

        private void exFani_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.chap.sch.ir/fanni-list");
        }

        private void exRahnam_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.chap.sch.ir/rahnama-tadris");
        }
    }
}