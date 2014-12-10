using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doto_Unlocker.Model
{
    interface IContentProvider
    {
        string ID { get; }

        IEnumerable<IGraphicContent> GetContentList();
        System.Drawing.Image GetInstalledContentImage();
        IGraphicContent InstallContent(int ID);
    }

    delegate IContentProvider ContentProviderFactory(VPK.VpkArchive vpk, VDF.VdfNode contentSchema, string contentFilesPath);
}
