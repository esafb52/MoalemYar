/****************************** ghost1372.github.io ******************************\
*	Module Name:	MetroBorder.cs
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

namespace Arthas.Controls.Metro
{
    public class MetroBorder : Border
    {
        public static readonly DependencyProperty AutoCornerRadiusProperty = ElementBase.Property<MetroBorder, bool>(nameof(AutoCornerRadiusProperty));
        public bool AutoCornerRadius { get { return (bool)GetValue(AutoCornerRadiusProperty); } set { SetValue(AutoCornerRadiusProperty, value); } }

        public MetroBorder()
        {
            Loaded += delegate { SizeChang(); };
            SizeChanged += delegate { SizeChang(); };
        }

        private void SizeChang()
        {
            if (AutoCornerRadius)
            {
                if (IsLoaded)
                {
                    CornerRadius = new CornerRadius(ActualWidth >= ActualHeight ? ActualHeight / 2 : ActualWidth / 2);
                }
            }
        }
    }
}