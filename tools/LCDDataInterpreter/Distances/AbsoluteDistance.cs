using System;
using System.Drawing;

namespace LCDDataInterpreter.Distances {
    class AbsoluteDistance: Distance {
        public override void calculateDistance(PatternDefinition d, Bitmap b) {
            Color c1, c2;
            long dist = 0;

            // resize b2 to match b1
            b = new Bitmap(b, d.Pattern.Size);

            for (int x = 0; x < d.Pattern.Width; x++) {
                for (int y = 0; y < d.Pattern.Height; y++) {
                    c1 = d.Pattern.GetPixel(x, y);
                    c2 = b.GetPixel(x, y);
                    dist += absD(c1.R, c2.R) + absD(c1.G, c2.G) + absD(c1.B, c2.B);
                }
            }
            Value = dist;
        }

        private static int absD(int a, int b) {
            return Math.Abs(a - b);
        }

        public override string ToString() {
            return "d=" + Value;
        }
    }
}
