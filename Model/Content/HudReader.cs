using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doto_Unlocker.VPK;

namespace Doto_Unlocker.Model
{
    interface IHudReader
    {
        Image ReadImage(string imgName);
    }

    abstract class HudReaderBase: IHudReader
    {
        protected VpkArchive arc;
        protected List<VpkEntry> defaultFiles;

        protected HudReaderBase(VpkArchive arc, List<VpkEntry> defaultFiles)
        {
            this.arc = arc;
            this.defaultFiles = defaultFiles;
        }

        public abstract Image ReadImage(string imgName);
    }


    class DefaultHudReader: HudReaderBase
    {
        string basePath;
        string relPath;

        public DefaultHudReader(VpkArchive arc, List<VpkEntry> defaultFiles, string basePath, string relPath): base(arc, defaultFiles)
        {
            this.basePath = basePath;
            this.relPath = relPath;
        }

        public override Image ReadImage(string imgName)
        {
            var relPath = this.relPath + imgName;
            var fullPath = basePath + relPath;
            byte[] data = ReadFile(fullPath, relPath);

            return Image.FromStream(new MemoryStream(data), false, false);
        }

        public byte[] ReadFile(string full, string rel)
        {
            if (File.Exists(full))
            {
                return File.ReadAllBytes(full);
            }
            else
            {
                var file = defaultFiles.SingleOrDefault((f) => rel.Equals(f.Path + '/' + f.Name + '.' + f.Ext, StringComparison.OrdinalIgnoreCase));
                if (file != null)
                {
                    return arc.ReadFile(file);
                }
                else
                    return null;
            }
        }
    }

    class ArchivedHudReader: HudReaderBase
    {
        List<VpkEntry> hudFiles;
        string styleName;

        public ArchivedHudReader(VpkArchive arc, List<VpkEntry> defaultFiles, List<VpkEntry> hudFiles, string styleName): base(arc, defaultFiles)
        {
            this.hudFiles = hudFiles;
            this.styleName = styleName;
        }

        public override Image ReadImage(string imgName)
        {
            var data = ReadFile(imgName);
            if (data != null)
                return Image.FromStream(new MemoryStream(data), false, false);
            else
                return null;
        }

        public byte[] ReadFile(string imgName)
        {
            if (styleName != null) // try to read styled one
            {
                var divider = imgName.LastIndexOfAny(new char[]{'\\','/'});
                var fileNameStyled = imgName.Substring(0, divider) + '/' + styleName + '/' + imgName.Substring(divider+1);

                var fileStyled = hudFiles.SingleOrDefault(entry => CheckPath(entry, fileNameStyled));
                if (fileStyled != null)
                {
                    return arc.ReadFile(fileStyled);
                }
            }

            // try to read nonstyled img
            var file = hudFiles.SingleOrDefault(entry => CheckPath(entry, imgName));
            if (file != null)
            {
                return arc.ReadFile(file);
            }
            else
            {
                // try to read default
                file = defaultFiles.SingleOrDefault(entry => CheckPath(entry, imgName));
                if (file != null)
                {
                    return arc.ReadFile(file);
                }
                else
                {
                    return null;
                }
            }
        }

        private bool CheckPath(VpkEntry entry, string filename)
        {
            var path = entry.Path + '/' + entry.Name + '.' + entry.Ext;
            return path.EndsWith(filename, StringComparison.OrdinalIgnoreCase);
        }
    }
}
