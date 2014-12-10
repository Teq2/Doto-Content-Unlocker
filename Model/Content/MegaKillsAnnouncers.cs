using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Doto_Unlocker.VDF;
using Doto_Unlocker.VPK;

namespace Doto_Unlocker.Model
{
    class MegaKillsAnnouncers: AnnouncersBase
    {
        const string ann_category = "announcers_killing_spree";

        override protected string defaultName
        {
            get { return "announcer_killing_spree"; }
        }

        override protected string defaultNameInternal
        {
            get { return "npc_dota_hero_announcer_killing_spree"; }
        }

        override protected string itemsSlot
        {
            get { return "mega_kills"; }
        }

        public override string ID
        {
            get { return ann_category; }
        }

        public static IContentProvider CreateInstance(VPK.VpkArchive vpk, VDF.VdfNode contentSchema, string contentFilesPath)
        {
            return new MegaKillsAnnouncers(vpk, contentSchema, contentFilesPath);
        }

        private MegaKillsAnnouncers(VpkArchive vpk, VdfNode schema, string dotaPath) : base(vpk, schema, dotaPath) { }
    }
}
