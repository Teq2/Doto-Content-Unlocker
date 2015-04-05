/*
 * Doto-Content-Unlocker, tool for unlocking custom dota2 content.
 * 
 *  Copyright (c) 2014 Teq, https://github.com/Teq2/Doto-Content-Unlocker
 * 
 *  Permission is hereby granted, free of charge, to any person obtaining a copy of 
 *  this software and associated documentation files (the "Software"), to deal in the 
 *  Software without restriction, including without limitation the rights to use, copy, 
 *  modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
 *  and to permit persons to whom the Software is furnished to do so, subject to the 
 *  following conditions:
 * 
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 *  INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 *  PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 *  HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 *  CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
 *  OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Doto_Unlocker
{
    static class SteamPaths
    {
        static string defaultSteamPath;
        public static string DefaultSteamPath
        {
            get
            {
                return defaultSteamPath;
            }
        }

        static string defaultDota2Path;
        public static string DefaultDota2Path
        {
            get
            {
                return defaultDota2Path;
            }
        }

        static SteamPaths()
        {
            defaultSteamPath = DefaultSteamFolder();
            defaultDota2Path = GetDota2Path(defaultSteamPath);
        }

        public static bool CheckSteamPath(string steamPath)
        {
            return Path.IsPathRooted(steamPath) && File.Exists(steamPath + "/Steam.exe");
        }

        public static bool CheckDota2Path(string dotaPath)
        {
            return Path.IsPathRooted(dotaPath) && File.Exists(dotaPath + "/dota.exe");
        }

        public static string DefaultSteamFolder()
        {
            return (string)Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null);
        }

        public static string GetDota2Path(string steamPath)
        {
            foreach (var lib in Libraries(steamPath))
            {
                var manifest = lib + "/appmanifest_570.acf";

                if (File.Exists(manifest))
                {
                    string data = File.ReadAllText(manifest, Encoding.UTF8);
                    VDF.VdfNode nodes = VDF.VdfParser.Parse(data);
                    VDF.VdfNode folder = nodes["installdir"];
                    if (folder != null)
                    {
                        return lib + "/common/" + folder;
                    }
                }
            }
            return null;
        }

        private static IEnumerable<String> Libraries(string steamPath)
        {
            // default library
            yield return steamPath + "/SteamApps";

            // additional libraries
            string configFile = steamPath + "/SteamApps/libraryfolders.vdf";
            if (File.Exists(configFile))
            {
                var data = File.ReadAllText(configFile, Encoding.UTF8);
                var folders = VDF.VdfParser.Parse(data);

                if (folders != null)
                foreach (var folder_node in folders.ChildNodes)
                {
                    if (Path.IsPathRooted(folder_node))
                        yield return folder_node.Val.Replace("\\\\", "/") + "/SteamApps";
                }
            }
        }
    }
}
