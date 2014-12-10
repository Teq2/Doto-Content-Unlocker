using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doto_Unlocker.Model
{
    delegate byte[] ThumbnailerProc ( );

    class Thumbnails
    {
        const string cacheDir = "Thumbnail cache\\";
        string thumbnailsDir;

        public Thumbnails(string category)
        {
            thumbnailsDir = cacheDir + category + "\\";

            if (!Directory.Exists(thumbnailsDir))
            {
                Directory.CreateDirectory(thumbnailsDir);
            }
        }

        public Image GetThumbnail(string name)
        {
            string path = thumbnailsDir + name;
            if (File.Exists(path))
            {
                var bytes =  File.ReadAllBytes(path);
                using (var ms = new MemoryStream(bytes))
                {
                    return Image.FromStream(ms, false, false);
                }
            }
            return null;
        }

        public void StoreThumbnail(string name, Image data)
        {
            string path = thumbnailsDir + name;
            using (var file = new FileStream(path, FileMode.Create))
            {
                data.Save(file, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

    }
}
