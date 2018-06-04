
/****************************** ghost1372.github.io ******************************\
*	Module Name:	ISettings.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 3, 07:46 ب.ظ
*	
***********************************************************************************/
using nucs.JsonSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ISettings : JsonSettings
{
    public override string FileName { get; set; } = "TheDefaultFilename.extension"; //for loading and saving.

    #region Property
    public virtual bool CredentialLogin { get; set; }
    public virtual bool Autorun { get; set; }
    public virtual bool? HamburgerMenu { get; set; }
    public virtual bool AutoSendReport { get; set; }
    public virtual string SkinCode { get; set; }
    public virtual string VersionCode { get; set; }
    public virtual int? ChartType { get; set; }
    public virtual string ChartColor { get; set; }
    public virtual int? ChartColorIndex { get; set; }
    public virtual int? DefaultSchool { get; set; }
    public virtual string DefaultServer { get; set; }

    #endregion
    public ISettings() { }
    public ISettings(string fileName) : base(fileName) { }
}