using System.Drawing;
using System.Drawing.Imaging;

namespace LCDDataInterpreter.Filter {
    class Inverter: IFilter {
        public Bitmap Process(Bitmap input, bool returnAsCopy) {
            Bitmap output = returnAsCopy ? input.Clone() as Bitmap : input;
            Invert(output);
            return output;
        }

        /// <summary>
        /// taken from: http://www.codeproject.com/KB/GDI-plus/csharpgraphicfilters11.aspx
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static void Invert(Bitmap b) {
            // GDI+ still lies to us - the return format is BGR, NOT RGB. 
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            unsafe {
                byte* p = (byte*)(void*)Scan0;
                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width * 3;
                for (int y = 0; y < b.Height; ++y) {
                    for (int x = 0; x < nWidth; ++x) {
                        p[0] = (byte)(255 - p[0]);
                        ++p;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);
        }
    }
}
