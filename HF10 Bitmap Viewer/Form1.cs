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

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {

        }

        private void btnLoadFile_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            if (ofd.ShowDialog(this) == DialogResult.OK) {
                lblFilename.Text = ofd.FileName;
                _dataFile = ofd.OpenFile();
            }
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e) {
            String decInfo = "pos: {0:g} / w: {1:g}px / h: {2:g}px";
            lblDecimalValues.Text = String.Format(decInfo, 
                nudPos.Value, nudWidth.Value, nudHeight.Value);

            GenerateBitmap();
        }

        private void printBitmapHeader(CanonBitmapHeader header) {
            string hInfo = "{0:x} {1:x} {2:x} {3:x} / {0} {1} {2} {3}";
            lblBitmapHeader.Text = String.Format(hInfo, header.Unknown1,
                header.Unknown2, header.Width, header.Unknown3);
        }

        private void GenerateBitmap() {
            CanonBitmapHeader header;
            CanonBitmap cb;
            //try {

            if (_dataFile == null)
                return;
                //throw new Exception("no file loaded");

            try {
                byte[] headerData = new byte[4];

                _dataFile.Seek((long)nudPos.Value, SeekOrigin.Begin);
                _dataFile.Read(headerData, 0, CanonBitmapHeader.SIZE);

                header = new CanonBitmapHeader(headerData);
                printBitmapHeader(header);
            
                nudWidth.Value = header.Width;

                int size = (int)(header.Width * nudHeight.Value);
                byte[] data = new byte[size];

                _dataFile.Read(data, 0, size);
                
                
                try {
                    cb = new CanonBitmap(header, data);
                    cb.Height = (byte)nudHeight.Value;
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                    return;
                }

                try {
                    pbZoomed.Image = pbMediumZoom.Image = pbOriginal.Image = cb.Pic;
                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                    return;
                }
                
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
