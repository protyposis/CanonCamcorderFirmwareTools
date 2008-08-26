using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace HF10_Bitmap_Viewer {
    public partial class ColorForm : Form {
        private Stream _stream;
        private int count = 0;
        public ColorForm() {
            InitializeComponent();
        }

        public ColorForm(String file)
            : this() {
            if (file != null) {
                _stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                numericUpDown1.Maximum = _stream.Length;
                button1.Text = file.Contains("\\") ? file.Split('\\')[file.Split('\\').Length - 1] : file;
            }
        }

        private void ColorForm_Load(object sender, EventArgs e) {

        }

        private void button1_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog(this) == DialogResult.OK) {
                //lblFilename.Text = ofd.FileName;
                _stream = ofd.OpenFile();
                numericUpDown1.Maximum = _stream.Length;
                button1.Text = ofd.SafeFileName;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            byte[] bytes = new byte[4];
            _stream.Seek((int)numericUpDown1.Value, SeekOrigin.Begin);
            _stream.Read(bytes, 0, 4);

            byte alpha = bytes[3];
            if(cbInvert.Checked)
                alpha = (byte)(0xFF - bytes[3]);

            Color c = Color.FromArgb(alpha, bytes[0], bytes[1], bytes[2]);
            Color ci = Color.FromArgb(alpha, (byte)~bytes[0], (byte)~bytes[1], (byte)~bytes[2]);

            label1.Text = c.IsNamedColor ? c.Name : String.Format("{0:X8}", c.ToArgb());
            label1.BackColor = c;
            label1.ForeColor = ci;

        }

        private void button2_Click(object sender, EventArgs e) {
            Label l = new Label();
            l.Text = String.Format("{0:X8} ({1})", label1.BackColor.ToArgb(), count++);
            l.BackColor = label1.BackColor;
            toolTip1.SetToolTip(l, String.Format("pos: 0x{0:X8}", _stream.Position - 4));

            flowLayoutPanel1.Controls.Add(l);
            flowLayoutPanel1.ScrollControlIntoView(l);
            numericUpDown1.Value += 4;
        }

        private void button3_Click(object sender, EventArgs e) {
            numericUpDown1.Value -= 4;
        }

        private void flowLayoutPanel1_ControlAdded(object sender, ControlEventArgs e) {
        }

        private void button4_Click(object sender, EventArgs e) {
            flowLayoutPanel1.Controls.Clear();
            count = 0;
        }

        private void cbInvert_CheckedChanged(object sender, EventArgs e) {

        }

        private void btnNext50_Click(object sender, EventArgs e) {
            List<Control> ctrls = new List<Control>(50);
            for (int x = 0; x < 50; x++) {
                Label l = new Label();
                l.Text = String.Format("{0:X8} ({1})", label1.BackColor.ToArgb(), ++count);
                l.BackColor = label1.BackColor;
                toolTip1.SetToolTip(l, String.Format("pos: 0x{0:X8}", _stream.Position - 4));
                ctrls.Add(l);
                numericUpDown1.Value += 4;
            }
            flowLayoutPanel1.Controls.AddRange(ctrls.ToArray());
            flowLayoutPanel1.ScrollControlIntoView(ctrls[ctrls.Count - 1]);
        }
    }
}
