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
        private CanonBitmap _currentImage;
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
            ofd.ReadOnlyChecked = true;
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

        private void printBitmapHeader(CanonHeader header) {
            string hInfo = "header hex / dec: {0:X2} {1:X2} / {0} {1}";
            lblBitmapHeader.Text = String.Format(hInfo, header.Value1, header.Value2);
        }

        private void GenerateBitmap(long pos) {
            lblStatus.Text = "";
            _currentImage = GetBitmap(pos);
        }

        private CanonBitmap GetBitmap(long pos) {
            CanonHeader header;
            CanonBitmap cb;

            if (_bitmapProvider == null)
                return null;
            //throw new Exception("no file loaded");

            try {
                if (cbSymbolsImages.Checked)
                    header = _bitmapProvider.readHeader(pos);
                else
                    header = _bitmapProvider.readBigHeader(pos);
                //if (header.Width * 2 != header.Unknown) Console.WriteLine("{0:x} {1} {2}", pos, header.Width, header.Unknown);
                printBitmapHeader(header);
                //Console.WriteLine("header: {0} {1}", header.Width, header.Unknown);

                if (cbFixedWidth.Checked)
                    cb = _bitmapProvider.readBitmap(header, (int)nudWidth.Value, (int)nudHeight.Value);
                else {
                    if (cbSymbolsImages.Checked)
                        cb = _bitmapProvider.readBitmap(header, header.Value1, (int)nudHeight.Value);
                    else
                        cb = _bitmapProvider.readBitmap(header, header.Value1, header.Value2);
                }

                NUDValueChange(nudWidth, cb.Width);
                NUDValueChange(nudHeight, cb.Height);

                return cb;

            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                lblStatus.Text = e.Message;
                //MessageBox.Show(this, e.StackTrace);
            }

            return null;
        }

        private void ShowBitmap(CanonBitmap i) {
            if(i != null)
                pbZoomed.Image = pbMediumZoom.Image = pbOriginal.Image = i.Pic;
        }

        private void btnNext_Click(object sender, EventArgs e) {
            if(_bitmapProvider != null) {
                bmpPointers.Push((long)nudPos.Value);
                AddImageToPanel(_currentImage);
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

        private void AddImageToPanel(CanonBitmap i) {
            Control c = CreatePanelImage(i);
            flpBitmaps.Controls.Add(c);
            flpBitmaps.ScrollControlIntoView(c);
        }

        private void CreateTooltip(Control c, CanonBitmap i) {
            toolTip1.SetToolTip(c,
                String.Format("0x{0:X8} w: {1} ws: {2}",
                i.Header.Origin, i.Header.Value1, i.Header.Value2));
        }

        private void AddControlsToPanel(List<Control> l) {
            flpBitmaps.Controls.AddRange(l.ToArray());
            flpBitmaps.ScrollControlIntoView(l[l.Count - 1]);
        }

        private Control CreatePanelImage(CanonBitmap i) {
            int border = 4;

            PictureBoxEx pb = new PictureBoxEx();
            pb.Width = pb.Height = CanonBitmapProvider.DEFAULT_HEIGHT + border;

            if (i != null) {
                //if(i.Header.Unknown > pb.Width)

                if (i.Header is CanonBitmapHeader)
                    pb.Width = i.Header.Value2 + border;
                else if (i.Header is CanonBigBitmapHeader) {
                    pb.Width = i.Header.Value1 + border;
                    pb.Height = i.Header.Value2 + border;
                }
                pb.SizeMode = PictureBoxSizeMode.CenterImage;
                CreateTooltip(pb, i);

                try {
                    if(i.Header is CanonBitmapHeader)
                        pb.Image = CanonBitmap.FixedSize(i.Pic, i.Header.Value2, i.Height);
                    else
                        pb.Image = i.Pic;
                }
                catch { }
            }
            return pb;
        }

        private void cbFixedWidth_CheckedChanged(object sender, EventArgs e) {
            nudWidth.Enabled = nudHeight.Enabled = cbFixedWidth.Checked;

            //if (!cbFixedWidth.Checked) {
            //    GenerateBitmap((long)nudPos.Value);
            //}
        }

        private void ByteAlign_CheckedChanged(object sender, EventArgs e) {
            if (rbOneByte.Checked) _bitmapProvider.ByteAlignment = ByteAlignment.Byte;
            else if (rbTwoByte.Checked) _bitmapProvider.ByteAlignment = ByteAlignment.Halfword;
            else if (rbFourByte.Checked) _bitmapProvider.ByteAlignment = ByteAlignment.Word;

            GenerateBitmap((long)nudPos.Value);
        }

        private void btnNext20_Click(object sender, EventArgs e) {
            int numBitmaps = 50;
            List<Control> bmpCtrls = new List<Control>(numBitmaps);

            if (_bitmapProvider != null) {
                for (int x = 0; x < numBitmaps; x++) {
                    bmpPointers.Push((long)nudPos.Value);
                    NUDValueChange(nudPos, _bitmapProvider.Position);
                    bmpCtrls.Add(CreatePanelImage(_currentImage));
                    GenerateBitmap(_bitmapProvider.Position);
                }
                AddControlsToPanel(bmpCtrls);
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

        private void button1_Click(object sender, EventArgs e) {
            ColorForm cf;
            if (lblFilename.Text != String.Empty)
                cf = new ColorForm(lblFilename.Text);
            else
                cf = new ColorForm();

            cf.Show();
        }

        //private void ExportBitmap(Control c) {
        //    SaveFileDialog sfd = new SaveFileDialog();
        //    sfd.Filter = "Bitmaps|*.bmp";
        //    sfd.AddExtension = true;
        //    if (sfd.ShowDialog(this) == DialogResult.OK) {
        //        int w = c.Width;   // Breite des Controls / der Form
        //        int h = c.Height;  // Höhe des Controls / der Form

        //        // Bitmap für das Abbild des Controls / der Form bereitstellen
        //        Bitmap bmp = new Bitmap(w, h);

        //        // Screenshot vornehmen und zurückgeben
        //        c.DrawToBitmap(bmp, Rectangle.FromLTRB(0, 0, w, h));
        //        bmp.Save(sfd.FileName);
        //    }
        //}

    }
}
