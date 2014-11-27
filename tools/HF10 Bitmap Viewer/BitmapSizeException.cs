using System;
using System.Collections.Generic;
using System.Text;

namespace HF10_Bitmap_Viewer {
    class BitmapSizeException: BitmapException {
        public BitmapSizeException() : base() { }
        public BitmapSizeException(string message):base(message){}
        public BitmapSizeException(string message, Exception innerException):base(message, innerException){}
    }
}
