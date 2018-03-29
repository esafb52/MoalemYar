/****************************** ghost1372.github.io ******************************\
*	Module Name:	MetroCanvasGrid.cs
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
    public class MetroCanvasGrid : ContentControl
    {
        public static readonly DependencyProperty ViewportProperty = ElementBase.Property<MetroCanvasGrid, Rect>(nameof(ViewportProperty));
        public Rect Viewport { get { return (Rect)GetValue(ViewportProperty); } set { SetValue(ViewportProperty, value); } }

        public static readonly DependencyProperty GridOpacityProperty = ElementBase.Property<MetroCanvasGrid, double>(nameof(GridOpacityProperty));
        public static readonly DependencyProperty GridSizeProperty = ElementBase.Property<MetroCanvasGrid, double>(nameof(GridSizeProperty));
        public static readonly DependencyProperty CornerRadiusProperty = ElementBase.Property<MetroCanvasGrid, CornerRadius>(nameof(CornerRadiusProperty));

        public double GridOpacity { get { return (double)GetValue(GridOpacityProperty); } set { SetValue(GridOpacityProperty, value); } }
        public CornerRadius CornerRadius { get { return (CornerRadius)GetValue(CornerRadiusProperty); } set { SetValue(CornerRadiusProperty, value); } }

        public bool IsApplyTheme { get; set; } = true;

        public MetroCanvasGrid()
        {
            Utility.Refresh(this);
        }

        static MetroCanvasGrid()
        {
            ElementBase.DefaultStyle<MetroCanvasGrid>(DefaultStyleKeyProperty);
        }
    }
}