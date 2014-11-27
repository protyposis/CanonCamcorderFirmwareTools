using System.Drawing;

namespace LCDDataInterpreter.Filter {
    class Sharpen : ConvolutionFilter, IFilter {
        private readonly int weight;

        public Sharpen() {
            weight = 9;
        }

        public Sharpen(int weight) {
            this.weight = weight;
        }

        public Bitmap Process(Bitmap input, bool returnAsCopy) {
            Bitmap output = returnAsCopy ? input.Clone() as Bitmap : input;
            sharpen(output, weight);
            return output;
        }

        /// <summary>
        /// http://www.codeproject.com/KB/GDI-plus/csharpfilters.aspx
        /// </summary>
        /// <param name="b"></param>
        /// <param name="nWeight"></param>
        /// <returns></returns>
        private static bool sharpen(Bitmap b, int nWeight /* default to 11*/ ) {
            ConvMatrix m = new ConvMatrix();
            m.SetAll(0);
            m.Pixel = nWeight;
            m.TopMid = m.MidLeft = m.MidRight = m.BottomMid = -2;
            m.Factor = nWeight - 8;

            return Conv3x3(b, m);
        }
    }
}
