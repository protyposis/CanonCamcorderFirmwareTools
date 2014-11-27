using System.Drawing;

namespace LCDDataInterpreter.Features {
    abstract class Feature {

        public abstract void Extract(Bitmap image);
    }
}
