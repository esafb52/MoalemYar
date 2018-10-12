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

public class ISettings : JsonSettings
{
    public override string FileName { get; set; } = "TheDefaultFilename.extension"; //for loading and saving.

    #region Property

    public virtual bool CredentialLogin { get; set; } = false;
    public virtual bool Autorun { get; set; } = false;
    public virtual int? DefaultSchool { get; set; } = 0;
    public virtual string DefaultServer { get; set; } = "http://5743.zanjan.medu.ir";

    #endregion Property

    public ISettings()
    {
    }

    public ISettings(string fileName) : base(fileName)
    {
    }
}