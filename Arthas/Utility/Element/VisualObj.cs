
/****************************** ghost1372.github.io ******************************\
*	Module Name:	VisualObj.cs
*	Project:		Arthas
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 22, 05:54 ب.ظ
*	
***********************************************************************************/
using System;
using System.Windows.Media;

namespace Arthas.Utility.Element
{
    public class VisualObj
    {
        public static void ActionOnAllElement(Visual visual, Action<Visual> action)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
            {
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(visual, i);
                if (childVisual != null)
                {
                    action(childVisual);
                    ActionOnAllElement(childVisual, action);
                }
            }
        }
    }
}