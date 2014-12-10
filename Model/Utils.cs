using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Doto_Unlocker.Model
{
    static class Utils
    {
        /// <summary>
        /// Keeps directories, deletes files only
        /// </summary>
        /// <param name="dir"></param>
        public static void ClearDir(string dir)
        {
            var di = new DirectoryInfo(dir);

            foreach (var file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (var subDir in di.GetDirectories())
            {
                ClearDir(subDir.FullName);
            }
        }

        public static void CheckAndCreateDirectory(string fullPath)
        {
            var dir = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }

        public static float GetRatio(this Image i)
        {
            return (float)i.Height / (float)i.Width;
        }
    }
}
