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
using MetroFramework;
using System.Text.RegularExpressions;

namespace Doto_Unlocker
{
    public class PathEventArgs : EventArgs
    {
        public string NewPath
        {
            get; set;
        }
    }

    public class StyleChangedEventArgs : EventArgs
    {
        public MetroColorStyle NewStyle
        {
            get;
            set;
        }
    }

    public delegate void PathChangedEventHandler (PathEventArgs e);
    public delegate void StyleChangedEventHandler(StyleChangedEventArgs e);

    public static class Settings
    {
        private const string settingsFile = "dcu.cfg";
        private static readonly string settingsFilePath;

        public static event PathChangedEventHandler SteamPathChanged;
        public static event PathChangedEventHandler DotaPathChanged;
        public static event StyleChangedEventHandler StyleChanged;

        static string assemblyDirectory;
        public static string AssemblyDirectory
        {
            get
            {
                return assemblyDirectory;
            }
        }

        /// <summary>
        /// After initialization contains Valid path or Null
        /// </summary>
        static string steamPath;
        public static string SteamPath
        {
            get
            {
                return steamPath;
            }
            set
            {
                string newPath = PreprocessPathValue(value);

                if (!ComparePaths(steamPath, newPath))
                {
                    steamPath = newPath;
                    PathChangedEventHandler handler = SteamPathChanged;
                    if (handler != null) handler(new PathEventArgs() { NewPath = steamPath });
                }
            }
        }

        /// <summary>
        /// After initialization contains Valid path or Null
        /// </summary>
        static string dota2Path;
        public static string Dota2Path
        {
            get
            {
                return dota2Path;
            }
            set
            {
                string newPath = PreprocessPathValue(value);

                if (!ComparePaths(dota2Path, newPath))
                {
                    dota2Path = newPath;
                    PathChangedEventHandler handler = DotaPathChanged;
                    if (handler != null) handler(new PathEventArgs() { NewPath = dota2Path });
                }
            }
        }

        private static MetroColorStyle style;
        public static MetroColorStyle Style
        {
            get
            {
                return style;
            }
            set
            {
                style = value;
                var handler = StyleChanged;
                if (handler != null) handler(new StyleChangedEventArgs() { NewStyle = value });    
            }
        }

        public static Dictionary<string, string> LastInstalled
        {
            get;
            private set;
        }

        /// <summary>
        /// Static field initializers can't be used in this class
        /// </summary>
        static Settings()
        {
            assemblyDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            settingsFilePath = Path.Combine(assemblyDirectory, settingsFile);

            try {
                Load();
            }
            catch (Exception) { /* prevent TypeInitializationException, all problems will be detected on Save(); */ }

            if (!SteamPaths.CheckSteamPath(steamPath) || !SteamPaths.CheckDota2Path(dota2Path))
            {
                steamPath = SteamPaths.DefaultSteamPath;
                dota2Path = SteamPaths.DefaultDota2Path;
            }
            if (LastInstalled == null) LastInstalled = new Dictionary<string, string>();
        }

        public static void Save()
        {
            var ser = new System.Runtime.Serialization.DataContractSerializer(typeof(SettingsData));
            using (var s = System.Xml.XmlWriter.Create(settingsFilePath))
            {
                ser.WriteObject(s, new SettingsData()
                {
                    SteamPath = steamPath,
                    Dota2Path = dota2Path,
                    Style = style,
                    LastInstalled = LastInstalled
                });
            }
        }

        private static void Load()
        {
            if (!File.Exists(settingsFile)) return;
            try
            {
                var des = new System.Runtime.Serialization.DataContractSerializer(typeof(SettingsData));
                using (var s = System.Xml.XmlReader.Create(settingsFilePath))
                {
                    SettingsData _tmp = (SettingsData)des.ReadObject(s);
                    CopyFields(_tmp);
                }
            }
            catch (SerializationException)
            {
                // file contains wrong formatted data
                File.Delete(settingsFile);
            }
        }

        private static string PreprocessPathValue(string value)
        {
            if (value != null)
                value = Regex.Replace(value, @"[\\|/]+", "/").TrimEnd(' ', '/', '\\');
            return value;
        }

        private static bool ComparePaths(string path1, string path2)
        {
            if (path1 == null) 
                return path1 == path2;
            else
                return path1.Equals(path2, StringComparison.CurrentCultureIgnoreCase);
        }

        private static void CopyFields(SettingsData src)
        {
            steamPath = src.SteamPath;
            dota2Path = src.Dota2Path;
            style = src.Style;
            LastInstalled = src.LastInstalled;
        }

        /// <summary>
        /// Used this private class for make Settings class static
        /// </summary>
        [DataContract(Name="Settings")]
        private class SettingsData
        {
            [DataMember]
            public string SteamPath;
            [DataMember]
            public string Dota2Path;
            [DataMember]
            public MetroColorStyle Style;
            [DataMember]
            public Dictionary<string, string> LastInstalled;
        }
    }
}
