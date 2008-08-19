using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace HF10_Bitmap_Viewer {
    public class PictureBoxEx: PictureBox {
        protected override void OnPaint(PaintEventArgs pe) {
            pe.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            base.OnPaint(pe);
        }
    }
}
