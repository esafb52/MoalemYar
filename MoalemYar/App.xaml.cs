/****************************** ghost1372.github.io ******************************\
*	Module Name:	App.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 22, 05:53 ب.ظ
*
***********************************************************************************/

using HandyControl.Data.Enum;
using HandyControl.Tools;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace MoalemYar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.SetData("DataDirectory", AppVariable.fileName + @"\");

            #region Load Embedded Assembly

            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var name in assembly.GetManifestResourceNames())
            {
                if (name.ToLower()
                         .EndsWith(".resources") ||
                     !name.ToLower()
                          .EndsWith(".dll"))
                    continue;
                EmbeddedAssembly.Load(name,
                                       name);
            }

            #endregion Load Embedded Assembly

            #region Check AppData Folder Existen and Create Config.json

            if (!Directory.Exists(AppVariable.fileName))
                Directory.CreateDirectory(AppVariable.fileName);

            if (!Directory.Exists(AppVariable.fileNameBakhsh))
                Directory.CreateDirectory(AppVariable.fileNameBakhsh);

            #endregion Check AppData Folder Existen and Create Config.json

            
        }

        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            var fields = args.Name.Split(',');
            var name = fields[0];
            var culture = fields[2];
            if (name.EndsWith(".resources") &&
                 !culture.EndsWith("neutral"))
                return null;

            return EmbeddedAssembly.Get(args.Name);
        }

        internal void UpdateSkin(SkinType skin)
        {
            var skins0 = Resources.MergedDictionaries[0];
            skins0.MergedDictionaries.Clear();
            skins0.MergedDictionaries.Add(ResourceHelper.GetSkin(skin));
            skins0.MergedDictionaries.Add(ResourceHelper.GetSkin(typeof(App).Assembly, "Resources/Themes", skin));

            var skins1 = Resources.MergedDictionaries[1];
            skins1.MergedDictionaries.Clear();
            skins1.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
            });
            skins1.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/MoalemYar;component/Resources/Themes/Theme.xaml")
            });
        }
    }
}