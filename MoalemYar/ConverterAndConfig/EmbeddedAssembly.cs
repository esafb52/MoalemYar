/****************************** ghost1372.github.io ******************************\
*	Module Name:	EmbeddedAssembly.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 4, 6, 08:15 ب.ظ
*
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace MoalemYar
{
    public static class EmbeddedAssembly
    {
        private static Dictionary<string, Assembly> _dic;

        public static void Load(string embeddedResource,
                                    string fileName)
        {
            if (_dic == null)
                _dic = new Dictionary<string, Assembly>();

            byte[] ba;
            Assembly asm;
            var curAsm = Assembly.GetExecutingAssembly();

            using (var stm = curAsm.GetManifestResourceStream(embeddedResource))
            {
                if (stm == null)
                    return;

                ba = new byte[(int)stm.Length];
                stm.Read(ba,
                          0,
                          (int)stm.Length);
                try
                {
                    asm = Assembly.Load(ba);

                    _dic.Add(asm.GetName().Name,
                                asm);
                    return;
                }
                catch
                {
                }
            }

            bool fileOk;
            string tempFile;

            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var fileHash = BitConverter.ToString(sha1.ComputeHash(ba))
                                            .Replace("-",
                                                        string.Empty);

                tempFile = Path.GetTempPath() + fileName;

                if (File.Exists(tempFile))
                {
                    var bb = File.ReadAllBytes(tempFile);
                    var fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb))
                                                .Replace("-",
                                                            string.Empty);

                    fileOk = fileHash == fileHash2;
                }
                else
                {
                    fileOk = false;
                }
            }

            if (!fileOk)
            {
                File.WriteAllBytes(tempFile,
                                    ba);
            }

            asm = Assembly.LoadFile(tempFile);

            _dic.Add(asm.GetName().Name,
                        asm);
        }

        public static Assembly Get(string assemblyFullName)
        {
            if (_dic == null ||
                    _dic.Count == 0)
                return null;

            var name = new AssemblyName(assemblyFullName).Name;
            return _dic.ContainsKey(name)
                ? _dic[name]
                : null;
        }
    }
}