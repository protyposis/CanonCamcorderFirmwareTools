using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using LCDDataInterpreter.Distances;

namespace LCDDataInterpreter {
    public partial class PatternMatcherForm : Form {
        public PatternMatcherForm() {
            InitializeComponent();
            regionSelectPictureBox1.RegionSelectionAlwaysActive = false;
        }

        private void button1_Click(object sender, EventArgs e) {
            new MatchingMapSetupForm().ShowDialog(this);
        }

        private void button2_Click(object sender, EventArgs e) {
            regionSelectPictureBox1.RegionSelectionActive = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            regionSelectPictureBox1.NumberOfRegions = (int)numericUpDown1.Value;
        }

        private void button4_Click(object sender, EventArgs e) {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog(this) == DialogResult.OK) {
                regionSelectPictureBox1.Image = new Bitmap(ofd.OpenFile());
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            PatternDefinition match;
            var matcher = new ImageMatcher(Program.PatternDefinitionList, new AbsoluteDistance(), Program.FilterChain);
            var now = DateTime.Now;

            lblMatch.Text = "";
            foreach (var region in regionSelectPictureBox1.SelectedRegions) {
                match = matcher.Match(region);
                lblMatch.Text += match.Value;
                AddToList(now, region, match);
            }

            if (dataGridView1.Rows.Count > 0)
                dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
        }

        private void AddToList(DateTime time, Bitmap regionToMatch, PatternDefinition matchedPattern) {
            dataGridView1.Rows.Add(time.ToLongTimeString() + " " + time.Millisecond, regionToMatch, matchedPattern.Pattern, matchedPattern.Value);
        }

        private void button5_Click(object sender, EventArgs e) {
            var sfd = new SaveFileDialog {Filter = "Plain text file|*.txt"};
            
            if(sfd.ShowDialog(this) == DialogResult.OK) {
                var stream = sfd.OpenFile();
                var tw = new StreamWriter(stream);
                foreach (DataGridViewRow row in dataGridView1.Rows) {
                    tw.Write((String)row.Cells[3].Value);
                }
                tw.Flush();
                stream.Flush();
                stream.Close();
                stream.Dispose();
            }
        }
    }
}
