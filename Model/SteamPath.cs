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
            return File.Exists(steamPath + "/Steam.exe");
        }

        public static bool CheckDota2Path(string dotaPath)
        {
            return File.Exists(dotaPath + "/dota.exe");
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
                    var data = File.ReadAllText(manifest, Encoding.UTF8);
                    var nodes = VDF.VdfParser.Parse(data);
                    var folder = nodes["installdir"];
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
            string configFile = steamPath + "/config/config.vdf";
            if (File.Exists(configFile))
            {
                var data = File.ReadAllText(configFile, Encoding.UTF8);
                var nodes = VDF.VdfParser.Parse(data);
                var folders = nodes["Software"]["Valve"]["Steam"]["BaseInstallFolder"];

                foreach (var folder_node in folders.ChildNodes)
                {
                    yield return folder_node.Val.Replace("\\\\", "/") + "/SteamApps";
                }
            }
        }
    }
}
