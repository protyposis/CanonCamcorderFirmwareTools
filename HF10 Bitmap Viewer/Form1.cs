using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace HF10_Bitmap_Viewer {
    public partial class Form1 : Form {

        private Stream _dataFile;
        private Image _currentImage;
        private Stack<long> bmpPointers;

        private byte PAD_TO = 4;

        public Form1() {
            InitializeComponent();
            bmpPointers = new Stack<long>();
        }

        private void btnLoadFile_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog(this) == DialogResult.OK) {
                lblFilename.Text = ofd.FileName;
                _dataFile = new BufferedStream(ofd.OpenFile(), 1024 * 1024 * 10);
            }
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e) {
            String decInfo = "pos: {0:g} / w: {1:g}px / h: {2:g}px";
            lblDecimalValues.Text = String.Format(decInfo, 
                nudPos.Value, nudWidth.Value, nudHeight.Value);

            GenerateBitmap((long)nudPos.Value);
            ShowBitmap(_currentImage);
        }

        private void printBitmapHeader(CanonBitmapHeader header) {
            string hInfo = "header hex / dec: {0:x} {1:x} / {0} {1}";
            lblBitmapHeader.Text = String.Format(hInfo, header.Width, header.Unknown);
        }

        private void GenerateBitmap(long pos) {
            Image i = GetBitmap(pos);
            _currentImage = i;
        }

        private Image GetBitmap(long pos) {
            CanonBitmapHeader header;
            CanonBitmap cb;

            if (_dataFile == null)
                return null;
            //throw new Exception("no file loaded");

            try {
                byte[] headerData = new byte[CanonBitmapHeader.SIZE];

                if (_dataFile.Position != pos)
                    _dataFile.Seek(pos, SeekOrigin.Begin); // skip if next button (we are already at the right position)

                _dataFile.Read(headerData, 0, CanonBitmapHeader.SIZE);

                header = new CanonBitmapHeader(headerData);
                printBitmapHeader(header);

                Console.WriteLine("header: {0} {1}", header.Width, header.Unknown);

                //if(!cbFixedWidth.Checked)
                //    nudWidth.Value = header.Width;

                int size = (int)(header.Width * nudHeight.Value);
                int padding = (size + CanonBitmapHeader.SIZE) % PAD_TO == 0 ? 0 : PAD_TO - (size + CanonBitmapHeader.SIZE) % PAD_TO;
                Console.WriteLine("wanting to read {0} bytes, {0}%{1}={2} -> padding {3} bytes (pad_to {4})",
                    size, CanonBitmapHeader.SIZE, (size + CanonBitmapHeader.SIZE) % PAD_TO, padding, PAD_TO);

                byte[] data = new byte[size + padding];

                _dataFile.Read(data, 0, size + padding);


                cb = new CanonBitmap(header, data);
                //cb.Height = (byte)nudHeight.Value;
                //cb.Width = (byte)nudWidth.Value;

                return cb.Pic;

            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                //MessageBox.Show(this, e.StackTrace);
            }

            return null;
        }

        private void ShowBitmap(Image i) {
            pbZoomed.Image = pbMediumZoom.Image = pbOriginal.Image = i;
        }

        private void btnNext_Click(object sender, EventArgs e) {
            if(_dataFile != null) {
                bmpPointers.Push((long)nudPos.Value);
                AddImageToPanel(pbOriginal.Image);
                nudPos.Value = _dataFile.Position;
                ShowBitmap(_currentImage);
                //GenerateBitmap(_dataFile.Position);

                btnPrev.Enabled = bmpPointers.Count > 0;
            }
        }

        private void btnPrev_Click(object sender, EventArgs e) {
            if (bmpPointers.Count > 0)
                nudPos.Value = bmpPointers.Pop();
            
            btnPrev.Enabled = bmpPointers.Count > 0;
        }

        private void btnClearHistory_Click(object sender, EventArgs e) {
            bmpPointers.Clear();
            flpBitmaps.Controls.Clear();
        }

        private void AddImageToPanel(Image i) {
            flpBitmaps.Controls.Add(CreatePanelImage(i));
        }

        private void AddControlsToPanel(List<Control> l) {
            flpBitmaps.Controls.AddRange(l.ToArray());
        }

        private static Control CreatePanelImage(Image i) {
            PictureBoxEx pb = new PictureBoxEx();
            pb.Width = pb.Height = 48;
            pb.SizeMode = PictureBoxSizeMode.Zoom;

            pb.Image = i;
            return pb;
        }

        private void cbFixedWidth_CheckedChanged(object sender, EventArgs e) {
            //nudWidth.Enabled = cbFixedWidth.Checked;

            //if (!cbFixedWidth.Checked)
            //    GenerateBitmap((long)nudPos.Value);
        }

        private void ByteAlign_CheckedChanged(object sender, EventArgs e) {
            if (rbTwoByte.Checked) PAD_TO = 2;
            else if (rbFourByte.Checked) PAD_TO = 4;

            GenerateBitmap((long)nudPos.Value);
        }

        private void btnNext20_Click(object sender, EventArgs e) {
            int numBitmaps = 50;
            List<Control> bmpCtrls = new List<Control>(numBitmaps);

            if (_dataFile != null) {
                for (int x = 0; x < numBitmaps; x++) {
                    bmpPointers.Push((long)nudPos.Value);
                    bmpCtrls.Add(CreatePanelImage(_currentImage));
                    GenerateBitmap(_dataFile.Position);
                }

                AddControlsToPanel(bmpCtrls);
                nudPos.Value = _dataFile.Position;
                ShowBitmap(_currentImage);
                btnPrev.Enabled = bmpPointers.Count > 0;
            }
        }
    }
}
