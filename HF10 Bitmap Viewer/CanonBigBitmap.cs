using System;
using System.Collections.Generic;
using System.Text;

namespace HF10_Bitmap_Viewer {
    //public class CanonBigBitmap: CanonBitmap {
    //    private CanonBigBitmapHeader header;

    //    protected CanonBigBitmap() { }

    //    public CanonBigBitmap(byte[] data, long origin) {
    //        if (data.Length <= 0)
    //            throw new Exception("no data");

    //        if (data.Length < CanonBigBitmapHeader.SIZE)
    //            throw new Exception("too few data (no header)");

    //        byte[] headerData = new byte[CanonBitmapHeader.SIZE];
    //        Array.Copy(data, headerData, CanonBitmapHeader.SIZE);

    //        this.header = new CanonBigBitmapHeader(headerData, origin);

    //        byte[] imageData = new byte[data.Length - CanonBitmapHeader.SIZE];
    //        Array.Copy(data, CanonBitmapHeader.SIZE, imageData, 0, data.Length - CanonBitmapHeader.SIZE);

    //        this.data = imageData;

    //        if (this.header.Width <= 0)
    //            throw new Exception("invalid width (<=0)");

    //        this.width = this.header.Width;
    //        this.height = this.header.Height;
    //    }

    //    public CanonBigBitmap(CanonBigBitmapHeader header, byte[] data) {
    //        if (header == null)
    //            throw new NullReferenceException("header must not be null");

    //        if (data.Length <= 0)
    //            throw new Exception("no data");

    //        if (header.Width <= 0)
    //            throw new Exception("invalid width (<=0)");

    //        this.header = header;
    //        this.data = data;

    //        this.Width = header.Width;
    //        this.Height = header.Height;
    //    }

    //    public new CanonBigBitmapHeader Header {
    //        get { return header; }
    //    }
    //}

    public class CanonBigBitmapHeader : CanonHeader {

        static protected new int size = 4;

        public CanonBigBitmapHeader(byte[] bytes, long origin) {
            if (bytes.Length != SIZE)
                throw new InvalidHeaderException("invalid header size");

            this.v1 = bytes[1];
            this.v2 = bytes[3];

            this.origin = origin;
        }

        public int Width {
            get { return v1; }
        }

        public int Height {
            get { return v2; }
        }

        public static new int SIZE { get { return 4; } }
    }
}
