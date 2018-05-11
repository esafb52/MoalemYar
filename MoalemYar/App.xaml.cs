/****************************** ghost1372.github.io ******************************\
*	Module Name:	App.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 22, 05:53 ب.ظ
*
***********************************************************************************/

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
            #region This is for Change Database Location we change DataDirectory to Other Location

            AppDomain.CurrentDomain.SetData("DataDirectory", AppVariable.fileName + @"\");

            #endregion This is for Change Database Location we change DataDirectory to Other Location

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
    }
}