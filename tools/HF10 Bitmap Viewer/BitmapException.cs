using System;
using System.Collections.Generic;
using System.Text;

namespace HF10_Bitmap_Viewer {
    abstract class BitmapException: Exception {
        public BitmapException():base() {}
        public BitmapException(string message):base(message){}
        public BitmapException(string message, Exception innerException) : base(message, innerException) { }

    }
}
