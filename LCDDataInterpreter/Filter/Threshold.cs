using System.Drawing;
using System.Drawing.Imaging;

namespace LCDDataInterpreter.Filter {
    class Threshold:IFilter {

        private readonly byte thresholdValue;

        public Threshold() {
            thresholdValue = 128;
        }

        public Threshold(byte threshold) {
            thresholdValue = threshold;
        }

        public Bitmap Process(Bitmap input, bool returnAsCopy) {
            Bitmap output = returnAsCopy ? input.Clone() as Bitmap : input;
            threshold(output, thresholdValue);
            return output;
        }

        private static void threshold(Bitmap b, byte thresholdValue) {
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 3;

                for (int y = 0; y < b.Height; ++y) {
                    for (int x = 0; x < b.Width; ++x) {

                        if(p[0] > thresholdValue && p[1] > thresholdValue && p[2]> thresholdValue) {
                            p[0] = p[1] = p[2] = 0; 
                        } else {
                            p[0] = p[1] = p[2] = 0xFF; 
                        }

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);
        }
    }
}
