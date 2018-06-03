/****************************** ghost1372.github.io ******************************\
*	Module Name:	Magazine.xaml.cs
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
    /// Interaction logic for Magazine.xaml
    /// </summary>
    public partial class Magazine : UserControl
    {
        public Magazine()
        {
            InitializeComponent();
        }

        private void exKodak_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/2");
        }

        private void exNoAmoz_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/3");
        }

        private void exNoAmozRoshan_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/roshdmagazines/generalmahnameh/studentsmagazines/roshdroshannoamooz");
        }

        private void exDaneshAmoz_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/4");
        }

        private void exDaneshAmozRoshan_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/roshdmagazines/generalmahnameh/studentsmagazines/roshdroshandaneshamooz");
        }

        private void exNojavan_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/5");
        }

        private void exNojavanRoshan_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/roshdmagazines/generalmahnameh/studentsmagazines/roshdroshannojavan");
        }

        private void exJavan_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/6");
        }

        private void exJavanRoshan_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/roshdmagazines/generalmahnameh/studentsmagazines/roshdroshanjavan");
        }

        private void exBorhan_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/13");
        }

        private void exBorhanDovom_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/14");
        }

        private void exEbtedayi_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/7");
        }

        private void exTech_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/10");
        }

        private void exFarda_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/12");
        }

        private void exMoalem_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/9");
        }

        private void exMotavasete_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/8");
        }

        private void exQuran_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/15");
        }

        private void exMaref_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/16");
        }

        private void exModiriat_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/11");
        }

        private void exMoshaver_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/17");
        }

        private void exAdabiat_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/20");
        }

        private void exPish_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/18");
        }

        private void exJografi_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/21");
        }

        private void exTarikh_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/22");
        }

        private void exTarbiatBadani_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/23");
        }

        private void exZaban_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/24");
        }

        private void exJavane_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/42");
        }

        private void exHonar_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/25");
        }

        private void exRiazi_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/26");
        }

        private void exFizik_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/27");
        }

        private void exShimi_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/28");
        }

        private void exZist_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/29");
        }

        private void exZamin_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/30");
        }

        private void exFani_Click(object sender, EventArgs e)
        {
            web.Source = new Uri("http://www.roshdmag.ir/fa/magazine2/issue/archive/31");
        }
    }
}