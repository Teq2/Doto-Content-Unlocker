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

    [DataContract]
    public class Settings
    {
        const string settingsFile = "dcu.cfg";

        public event PathChangedEventHandler SteamPathChanged;
        public event PathChangedEventHandler DotaPathChanged;
        public event StyleChangedEventHandler StyleChanged;

        static Settings instance = new Settings();
        public static Settings Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// After initialization contains Valid path or Null
        /// </summary>
        [DataMember]
        string steamPath;
        public string SteamPath
        {
            get
            {
                return steamPath;
            }
            set
            {
                bool eq = ComparePathes(steamPath, value); 
                steamPath = value;
                if (!eq)
                {
                    PathChangedEventHandler handler = SteamPathChanged;
                    if (handler != null) handler(new PathEventArgs() { NewPath = steamPath });
                }
            }
        }

        /// <summary>
        /// After initialization contains Valid path or Null
        /// </summary>
        [DataMember]
        string dota2Path;
        public string Dota2Path
        {
            get
            {
                return dota2Path;
            }
            set
            {
                bool eq = ComparePathes(dota2Path, value);
                dota2Path = value;
                if (!eq)
                {
                    PathChangedEventHandler handler = DotaPathChanged;
                    if (handler != null) handler(new PathEventArgs() { NewPath = dota2Path });
                }
            }
        }

        [DataMember]
        private MetroColorStyle style;
        public MetroColorStyle Style
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

        [DataMember]
        public Dictionary<string,string> LastInstalled
        {
            get; private set;
        }

        Settings()
        {
            Load();

            if (!SteamPaths.CheckSteamPath(steamPath) || !SteamPaths.CheckDota2Path(dota2Path))
            {
                steamPath = SteamPaths.DefaultSteamPath;
                dota2Path = SteamPaths.DefaultDota2Path;
            }
            if (LastInstalled == null) LastInstalled = new Dictionary<string, string>();
        }

        bool ComparePathes(string path1, string path2)
        {
            if (path1 == null) return path1 == path2;
            return path1.Replace('/', '\\').Equals(path2, StringComparison.InvariantCultureIgnoreCase);
        }

        void CopyFields(Settings src)
        {
            steamPath = src.SteamPath;
            dota2Path = src.Dota2Path;
            LastInstalled = src.LastInstalled;
            style = src.Style;
        }

        void Load(String SettingsFile = settingsFile)
        {
            if (!File.Exists(SettingsFile)) return;

            var des = new System.Runtime.Serialization.DataContractSerializer(typeof(Settings));
            using (var s = System.Xml.XmlReader.Create(SettingsFile))
            {
                Settings _tmp = (Settings) des.ReadObject(s);
                CopyFields(_tmp);
            }
        }

        public void Save(String SettingsFile = settingsFile)
        {
            var ser = new System.Runtime.Serialization.DataContractSerializer(typeof(Settings));
            using (var s = System.Xml.XmlWriter.Create(SettingsFile))
            {
                ser.WriteObject(s, this);
            }
        }
    }
}
