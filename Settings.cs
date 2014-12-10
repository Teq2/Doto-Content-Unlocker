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
