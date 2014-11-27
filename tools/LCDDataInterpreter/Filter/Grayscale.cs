using System.Drawing;
using System.Drawing.Imaging;

namespace LCDDataInterpreter.Filter {
    class Grayscale: IFilter {

        public Bitmap Process(Bitmap input, bool returnAsCopy) {
            Bitmap output = returnAsCopy ? input.Clone() as Bitmap : input;
            GrayScale(output);
            return output;
        }

        /// <summary>
        /// taken from: http://www.codeproject.com/KB/GDI-plus/csharpgraphicfilters11.aspx
        /// </summary>
        /// <param name="b"></param>
        private static void GrayScale(Bitmap b) {
            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 3;

                byte red, green, blue;

                for (int y = 0; y < b.Height; ++y) {
                    for (int x = 0; x < b.Width; ++x) {
                        blue = p[0];
                        green = p[1];
                        red = p[2];

                        p[0] = p[1] = p[2] = (byte)(.299 * red + .587 * green + .114 * blue);

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);
        }
    }
}
