using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doto_Unlocker.VDF;
using Doto_Unlocker.VPK;

namespace Doto_Unlocker.Model
{
    class Announcers: AnnouncersBase
    {
        const string ann_category = "announcers";

        override protected string defaultName
        {
            get { return "announcer"; }
        }

        override protected string defaultNameInternal
        {
            get { return "npc_dota_hero_announcer"; }
        }

        override protected string itemsSlot
        {
            get { return "announcer"; }
        }

        public override string ID
        {
            get { return ann_category; }
        }

        public static IContentProvider CreateInstance(VPK.VpkArchive vpk, VDF.VdfNode contentSchema, string contentFilesPath)
        {
            return new Announcers(vpk, contentSchema, contentFilesPath);
        }

        private Announcers(VpkArchive vpk, VdfNode schema, string dotaPath): base(vpk, schema, dotaPath) { }
    }
}
