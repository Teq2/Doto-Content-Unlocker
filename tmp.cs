using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Doto_Unlocker
{
    public partial class tmp : Form
    {
        public tmp()
        {
            InitializeComponent();
            const string schemaPath = "scripts/items/items_game.txt";

            var arc = new VPK.VpkArchive(Settings.Dota2Path + @"\dota", "pak01");
            var raw = arc.ReadFile(schemaPath);
            var text = Encoding.ASCII.GetString(raw);
            var schema = VDF.VdfParser.Parse(text);
            var huds = Model.Huds.CreateInstance(arc, schema, Settings.Dota2Path);

            pictureBox1.Image = huds.GetInstalledContentImage();
        }
    }
}
