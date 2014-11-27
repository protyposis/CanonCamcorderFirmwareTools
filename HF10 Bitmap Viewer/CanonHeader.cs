using System;
using System.Collections.Generic;
using System.Text;

namespace HF10_Bitmap_Viewer {
    public enum CanonType: byte {
        SmallSymbol,
        BigBitmap
    }

    public abstract class CanonHeader {

        protected long origin;

        protected int v1;
        protected int v2;

        protected static int size = 2;

        public long Origin {
            get { return origin; }
        }

        public int Value1 {
            get { return v1; }
        }

        public int Value2 {
            get { return v2; }
        }

        public static int SIZE { get { return size; } }
    }
}
