using System.Drawing;
using System.Drawing.Imaging;

namespace LCDDataInterpreter.Filter {
    class Contrast:IFilter {

        private readonly sbyte contrastFactor;

        public Contrast() {
            contrastFactor = 1;
        }

        public Contrast(sbyte factor) {
            contrastFactor = factor;
        }

        public Bitmap Process(Bitmap input, bool returnAsCopy) {
            Bitmap output = returnAsCopy ? input.Clone() as Bitmap : input;
            contrast(output, contrastFactor);
            return output;
        }

        /// <summary>
        /// taken from: http://www.codeproject.com/KB/GDI-plus/csharpgraphicfilters11.aspx
        /// </summary>
        /// <param name="b"></param>
        /// <param name="nContrast"></param>
        /// <returns></returns>
        private static bool contrast(Bitmap b, sbyte nContrast) {
            if (nContrast < -100) return false;
            if (nContrast > 100) return false;

            double pixel = 0, contrast = (100.0 + nContrast) / 100.0;

            contrast *= contrast;

            int red, green, blue;

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 3;

                for (int y = 0; y < b.Height; ++y) {
                    for (int x = 0; x < b.Width; ++x) {
                        blue = p[0];
                        green = p[1];
                        red = p[2];

                        pixel = red / 255.0;
                        pixel -= 0.5;
                        pixel *= contrast;
                        pixel += 0.5;
                        pixel *= 255;
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;
                        p[2] = (byte)pixel;

                        pixel = green / 255.0;
                        pixel -= 0.5;
                        pixel *= contrast;
                        pixel += 0.5;
                        pixel *= 255;
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;
                        p[1] = (byte)pixel;

                        pixel = blue / 255.0;
                        pixel -= 0.5;
                        pixel *= contrast;
                        pixel += 0.5;
                        pixel *= 255;
                        if (pixel < 0) pixel = 0;
                        if (pixel > 255) pixel = 255;
                        p[0] = (byte)pixel;

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);

            return true;
        }
    }
}
