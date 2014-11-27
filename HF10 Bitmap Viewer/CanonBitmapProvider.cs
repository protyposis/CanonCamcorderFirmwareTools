using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace HF10_Bitmap_Viewer {
    enum ByteAlignment:byte {
        Word = 4,
        Halfword = 2,
        Byte = 1
    }

    class CanonBitmapProvider {
        public static int DEFAULT_HEIGHT = 18;
        
        private static int MAX_BUFFER = 1024 * 1024 * 10;
        
        private ByteAlignment _byteAlign = ByteAlignment.Word;
        private Stream _dataFile;

        public CanonBitmapProvider(Stream file) {
            _dataFile = new BufferedStream(file, 
                file.Length > MAX_BUFFER ? MAX_BUFFER : (int)file.Length);
        }

        /// <summary>
        /// Read header from current position
        /// </summary>
        /// <returns></returns>
        public CanonBitmapHeader readHeader() {
            byte[] headerData = new byte[CanonBitmapHeader.SIZE];

            if(_dataFile.Read(headerData, 0, CanonBitmapHeader.SIZE) != CanonBitmapHeader.SIZE)
                throw new InvalidHeaderException("could not read entire header (EOF?)");

            return new CanonBitmapHeader(headerData, _dataFile.Position - CanonBitmapHeader.SIZE);
        }

        /// <summary>
        /// Read big header from current position
        /// </summary>
        /// <returns></returns>
        public CanonBigBitmapHeader readBigHeader() {
            byte[] headerData = new byte[CanonBigBitmapHeader.SIZE];

            if (_dataFile.Read(headerData, 0, CanonBigBitmapHeader.SIZE) != CanonBigBitmapHeader.SIZE)
                throw new InvalidHeaderException("could not read entire header (EOF?)");

            return new CanonBigBitmapHeader(headerData, _dataFile.Position - CanonBigBitmapHeader.SIZE);
        }

        /// <summary>
        /// Read header from passed position
        /// </summary>
        /// <param name="pos">position to read the header from</param>
        /// <returns></returns>
        public CanonBitmapHeader readHeader(long pos) {
            // skip if we are already at the right position
            if (_dataFile.Position != pos && _dataFile.Seek(pos, SeekOrigin.Begin) != pos)
                throw new EndOfStreamException("cannot jump to the specified position (EOF?)");

            return readHeader();
        }

        /// <summary>
        /// Read big header from passed position
        /// </summary>
        /// <param name="pos">position to read the header from</param>
        /// <returns></returns>
        public CanonBigBitmapHeader readBigHeader(long pos) {
            // skip if we are already at the right position
            if (_dataFile.Position != pos && _dataFile.Seek(pos, SeekOrigin.Begin) != pos)
                throw new EndOfStreamException("cannot jump to the specified position (EOF?)");

            return readBigHeader();
        }

        /// <summary>
        /// Read the bitmap on the current position and adds the passed header.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public CanonBitmap readBitmap(CanonHeader header) {
            if(header is CanonBitmapHeader)
                return readBitmap(header, header.Value1, DEFAULT_HEIGHT);
            else if (header is CanonBigBitmapHeader)
                return readBitmap(header, header.Value1, header.Value2);
            
            // should not happen
            return null;
        }

        public CanonBitmap readBitmap(CanonHeader header, int width, int height) {
            CanonBitmap bmp;

            if (width <= 0 || height <= 0)
                throw new BitmapSizeException("width and height must be > 0");

            int size = width * height;
            int padding = (size + CanonHeader.SIZE) % (byte)_byteAlign == 0 ? 0 : (byte)_byteAlign - (size + CanonBitmapHeader.SIZE) % (byte)_byteAlign;
            
            //Console.WriteLine("trying to read {0} bytes, {0}%{1}={2} -> padding {3} bytes (pad_to {4})",
            //    size, CanonBitmapHeader.SIZE, (size + CanonBitmapHeader.SIZE) % (byte)_byteAlign, padding, _byteAlign);

            byte[] data = new byte[size + padding];

            if(_dataFile.Read(data, 0, size + padding) != (size + padding))
                throw new EndOfStreamException("cannot read the whole bitmap data (EOF?)");

            bmp = new CanonBitmap(header, data);
            bmp.Width = width;
            bmp.Height = height;

            return bmp;
        }

        public long Position {
            get { return _dataFile.Position; }
        }

        public long Length {
            get { return _dataFile.Length; }
        }

        public ByteAlignment ByteAlignment {
            get { return _byteAlign; }
            set { _byteAlign = value; }
        }
    }
}
