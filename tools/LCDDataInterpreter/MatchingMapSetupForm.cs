using System;
using System.Drawing;
using System.Windows.Forms;

namespace LCDDataInterpreter {
    public partial class MatchingMapSetupForm : Form {

        private readonly PatternDefinitionBindingList dict = Program.PatternDefinitionList as PatternDefinitionBindingList;

        public MatchingMapSetupForm() {
            InitializeComponent();
            dataGridView1.DataSource = dict;
        }

        Bitmap source;
        private void button1_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog(this) == DialogResult.OK) {
                source = new Bitmap(ofd.OpenFile());
                pictureBox1.Image = source;
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            foreach (Bitmap b in pictureBox1.SelectedRegions) {
                dict.Add(new PatternDefinition(Program.FilterChain.Process(b, false), "value?"));
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Pattern Definition Map|*.pdm";
            if (sfd.ShowDialog(this) == DialogResult.OK) {
                dict.Save(sfd.OpenFile());
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Pattern Definition Map|*.pdm";
            if (ofd.ShowDialog(this) == DialogResult.OK) {
                dict.Load(ofd.OpenFile());
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            pictureBox1.NumberOfRegions = (int)numericUpDown1.Value;
        }

        private void button5_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
