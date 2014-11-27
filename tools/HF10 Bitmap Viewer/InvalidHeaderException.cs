using System;
using System.Collections.Generic;
using System.Text;

namespace HF10_Bitmap_Viewer {
    class InvalidHeaderException: BitmapException {
        public InvalidHeaderException():base() {}
        public InvalidHeaderException(string message):base(message){}
        public InvalidHeaderException(string message, Exception innerException):base(message, innerException){}
    }
}
