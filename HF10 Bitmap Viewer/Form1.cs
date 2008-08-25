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

        private CanonBitmapProvider _bitmapProvider;
        private Image _currentImage;
        private EventHandler _valueChangedHandler;
        private Stack<long> bmpPointers;

        public Form1() {
            InitializeComponent();
            bmpPointers = new Stack<long>();

            _valueChangedHandler = new EventHandler(numericUpDown_ValueChanged);;
            nudPos.ValueChanged += _valueChangedHandler;
            nudWidth.ValueChanged += _valueChangedHandler;
            nudHeight.ValueChanged += _valueChangedHandler;
        }

        private void btnLoadFile_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog(this) == DialogResult.OK) {
                lblFilename.Text = ofd.FileName;
                _bitmapProvider = new CanonBitmapProvider(ofd.OpenFile());
                nudPos.Maximum = _bitmapProvider.Length;
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

            if (_bitmapProvider == null)
                return null;
            //throw new Exception("no file loaded");

            try {
                header = _bitmapProvider.readHeader(pos);
                printBitmapHeader(header);
                //Console.WriteLine("header: {0} {1}", header.Width, header.Unknown);

                if (cbFixedWidth.Checked)
                    cb = _bitmapProvider.readBitmap(header, (int)nudWidth.Value, (int)nudHeight.Value);
                else
                    cb = _bitmapProvider.readBitmap(header, header.Width, (int)nudHeight.Value);

                NUDValueChange(nudWidth, cb.Width);
                NUDValueChange(nudHeight, cb.Height);

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
            if(_bitmapProvider != null) {
                bmpPointers.Push((long)nudPos.Value);
                AddImageToPanel(pbOriginal.Image);
                nudPos.Value = _bitmapProvider.Position;
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
            nudWidth.Enabled = nudHeight.Enabled = cbFixedWidth.Checked;

            //if (!cbFixedWidth.Checked) {
            //    GenerateBitmap((long)nudPos.Value);
            //}
        }

        private void ByteAlign_CheckedChanged(object sender, EventArgs e) {
            if (rbTwoByte.Checked) _bitmapProvider.ByteAlignment = ByteAlignment.Halfword;
            else if (rbFourByte.Checked) _bitmapProvider.ByteAlignment = ByteAlignment.Word;

            GenerateBitmap((long)nudPos.Value);
        }

        private void btnNext20_Click(object sender, EventArgs e) {
            int numBitmaps = 50;
            List<Control> bmpCtrls = new List<Control>(numBitmaps);

            if (_bitmapProvider != null) {
                for (int x = 0; x < numBitmaps; x++) {
                    bmpPointers.Push(_bitmapProvider.Position);
                    bmpCtrls.Add(CreatePanelImage(_currentImage));
                    GenerateBitmap(_bitmapProvider.Position);
                }

                AddControlsToPanel(bmpCtrls);
                nudPos.Value = _bitmapProvider.Position;
                ShowBitmap(_currentImage);
                btnPrev.Enabled = bmpPointers.Count > 0;
            }
        }

        /// <summary>
        /// Changes the value of a NumericUpDown control without
        /// calling the value changed handler.
        /// </summary>
        /// <param name="nud"></param>
        /// <param name="value"></param>
        private void NUDValueChange(NumericUpDown nud, decimal value) {
            nud.ValueChanged -= _valueChangedHandler;
            nud.Value = value;
            nud.ValueChanged += _valueChangedHandler;
        }
    }
}
