using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace LCDDataInterpreter {
    class RegionSelectPictureBox: PictureBox {

        private int numberOfRegions = 0;

        public bool RegionSelectionAlwaysActive { get; set; }
        public bool RegionSelectionActive { get; set; }
        public int NumberOfRegions { get { return numberOfRegions; } set { numberOfRegions = value; this.Invalidate(); } }
        public Bitmap[] SelectedRegions { get { return GenerateSelectedRegionsBitmaps(); } }

        public RegionSelectPictureBox():base() {
            RegionSelectionAlwaysActive = true;
            RegionSelectionActive = false;
            numberOfRegions = 1;
        }

        private Point selectionStart, selectionEnd;
        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            Debug.WriteLine("OnMouseDown");
            if (e.Button == MouseButtons.Left && (RegionSelectionAlwaysActive || RegionSelectionActive)) {
                Debug.WriteLine("OnMouseDown LEFT");
                selectionStart = e.Location;
            }
        }
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left && (RegionSelectionAlwaysActive || RegionSelectionActive)) {
                selectionEnd = e.Location;
                this.Invalidate();
            }
        }
        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            Debug.WriteLine("OnMouseUp");
            if (e.Button == MouseButtons.Left && (RegionSelectionAlwaysActive || RegionSelectionActive)) {
                Debug.WriteLine("OnMouseUp LEFT");
                selectionEnd = e.Location;
                this.Invalidate();
                RegionSelectionActive = false;
            }
        }

        protected override void OnPaint(PaintEventArgs pe) {
            base.OnPaint(pe);
            PaintSelectionRectangle(pe.Graphics);
        }

        private void PaintSelectionRectangle(Graphics g) {
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            float width = (float)selectionEnd.X - selectionStart.X;
            float height = (float)selectionEnd.Y - selectionStart.Y;
            Pen p = new Pen(Color.Red, 1);
            g.DrawRectangle(p, selectionStart.X, selectionStart.Y, width, height);

            float sectionWidth = width / numberOfRegions;
            for (int x = 0; x < numberOfRegions; x++) {
                g.DrawLine(p, selectionStart.X + sectionWidth * x, selectionStart.Y, selectionStart.X + sectionWidth * x, selectionStart.Y + height);
            }
        }

        private Bitmap[] GenerateSelectedRegionsBitmaps() {
            try {
                Bitmap[] bitmaps = new Bitmap[numberOfRegions];

                float width = (float) selectionEnd.X - selectionStart.X;
                float height = (float) selectionEnd.Y - selectionStart.Y;
                float sectionWidth = width/numberOfRegions;

                Bitmap image = this.Image as Bitmap;
                for (int x = 0; x < numberOfRegions; x++) {
                    bitmaps[x] = Copy(image, new Rectangle(
                                                 (int) (selectionStart.X + x*sectionWidth),
                                                 selectionStart.Y,
                                                 (int) (sectionWidth),
                                                 (int) (height)));
                }

                return bitmaps;
            } catch {
                
            }

            return new Bitmap[0];
        }

        public new Image Image {
            set {
                if(value != null && value is Bitmap) ((Bitmap)value).SetResolution(96, 96);
                base.Image = value;
            }
            get {
                return base.Image;
            }
        }

        /// <summary>
        /// Copied from MSDN
        /// http://msdn.microsoft.com/en-us/library/aa457087.aspx
        /// </summary>
        /// <param name="srcBitmap"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        static public Bitmap Copy(Bitmap srcBitmap, Rectangle section) {
            // Create the new bitmap and associated graphics object
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            Graphics g = Graphics.FromImage(bmp);

            // Draw the specified section of the source bitmap to the new one
            g.DrawImage(srcBitmap, 0, 0, section, GraphicsUnit.Pixel);

            // Clean up
            g.Dispose();

            // Return the bitmap
            return bmp;
        }

    }
}
