/****************************** ghost1372.github.io ******************************\
*	Module Name:	MetroTitleMenuItem.cs
*	Project:		Arthas
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 22, 05:54 ب.ظ
*
***********************************************************************************/

using Arthas.Utility.Element;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Arthas.Controls.Metro
{
    public class MetroTitleMenuItem : MenuItem
    {
        public static readonly new DependencyProperty IconProperty = ElementBase.Property<MetroTitleMenuItem, ImageSource>(nameof(IconProperty), null);

        public new ImageSource Icon { get { return (ImageSource)GetValue(IconProperty); } set { SetValue(IconProperty, value); } }

        public MetroTitleMenuItem()
        {
            Utility.Refresh(this);
        }

        static MetroTitleMenuItem()
        {
            ElementBase.DefaultStyle<MetroTitleMenuItem>(DefaultStyleKeyProperty);
        }
    }
}