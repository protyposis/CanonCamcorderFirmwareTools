using System.Drawing;
using System.Drawing.Imaging;

namespace LCDDataInterpreter.Features {
    class Histogram: Feature {
        public int[] bins;

        public Histogram() {
            bins = new int[256];
        }

        public override void Extract(Bitmap b) {
            var bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), 
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 3;

                for (int y = 0; y < b.Height; ++y) {
                    for (int x = 0; x < b.Width; ++x) {

                        bins[truncate24to8bit(p[2], p[1], p[0])]++;

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);
        }

        private static byte truncate24to8bit(byte r, byte g, byte b) {
            byte divR, divG, divB;
            divR = divG = (0xFF + 1) / 8;   // truncate R, G to 3 bits
            divB = (0xFF + 1) / 4;          // truncate B to 2 bits
            return (byte)(b / divB | (g / divG) << 2 | (r / divR) << 5);
        }
    }
}
