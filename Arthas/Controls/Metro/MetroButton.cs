
/****************************** ghost1372.github.io ******************************\
*	Module Name:	MetroButton.cs
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
using System.Windows.Controls.Primitives;

namespace Arthas.Controls.Metro
{
    public enum ButtonState
    {
        None,
        Red,
        Green
    }

    public class MetroButton : Button
    {
        public static readonly DependencyProperty MetroButtonStateProperty = ElementBase.Property<MetroButton, ButtonState>(nameof(MetroButtonStateProperty), ButtonState.None);

        public ButtonState MetroButtonState { get { return (ButtonState)GetValue(MetroButtonStateProperty); } set { SetValue(MetroButtonStateProperty, value); } }

        public MetroButton()
        {
            Utility.Refresh(this);
        }

        static MetroButton()
        {
            ElementBase.DefaultStyle<MetroButton>(DefaultStyleKeyProperty);
        }
    }
}