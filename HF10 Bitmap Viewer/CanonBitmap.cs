using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace HF10_Bitmap_Viewer {
    public class CanonBitmap {
        private byte width;
        private byte height;

        private byte[] data;

        private CanonBitmapHeader header;
        private static ColorPalette palette = GetGrayScalePalette();


        public CanonBitmap(byte[] data) {
            if (data.Length <= 0)
                throw new Exception("no data");

            if (data.Length < CanonBitmapHeader.SIZE)
                throw new Exception("too few data (no header)");

            byte[] headerData = new byte[CanonBitmapHeader.SIZE];
            Array.Copy(data, headerData, CanonBitmapHeader.SIZE);

            this.header = new CanonBitmapHeader(headerData);

            byte[] imageData = new byte[data.Length - CanonBitmapHeader.SIZE];
            Array.Copy(data, CanonBitmapHeader.SIZE, imageData, 0, data.Length - CanonBitmapHeader.SIZE);

            this.data = imageData;

            if (this.header.Width <= 0)
                throw new Exception("invalid width (<=0)");

            this.width = this.header.Width;
            this.height = (byte)(this.data.Length / this.width);
        }

        public CanonBitmap(CanonBitmapHeader header, byte[] data) {
            if (header == null)
                throw new NullReferenceException("header must not be null");

            if (data.Length <= 0)
                throw new Exception("no data");

            if (header.Width <= 0)
                throw new Exception("invalid width (<=0)");

            this.header = header;
            this.data = data;

            this.Width = header.Width;
            this.Height = (byte)(data.Length / this.Width);
        }

        public byte Width {
            get { return width; }
            set { width = value; }
        }

        public byte Height {
            get { return height; }
            set { height = value; }
        }

        public CanonBitmapHeader Header {
            get { return header; }
        }

        public byte[] Data {
            get { return data; }
        }

        public Image Pic {
            get {
                Console.WriteLine(
                    "generating pic - w: {0}px h: {1}px data: {2}bytes (w*h={3}bytes)",
                    Width, Height, data.Length, Width * Height);

                int bytesToCopy = (Width * Height < data.Length ? Width * Height : data.Length);
                int fillupBytesTo4 = 4 - (Width % 4); // every scanline must be dividable by 4 (msdn docs!!?)
                if (fillupBytesTo4 == 4) fillupBytesTo4 = 0;

                Bitmap i = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                i.Palette = palette;
                //Console.WriteLine("create bitmap w{0} h{1}", i.Width, i.Height);

                BitmapData bd = i.LockBits(new Rectangle(0, 0, i.Width, i.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                bd.Stride = i.Width + fillupBytesTo4;
                bd.Width = Width;
                bd.Height = Height;

                //Console.WriteLine("bitmapdata w{0} h{1} stride{2}", bd.Width, bd.Height, bd.Stride);

                IntPtr p = bd.Scan0;

                int initPos = p.ToInt32();

                for (int x = 0; x < bd.Height; x++) {

                    if (bd.Width * x >= data.Length)
                        break;

                    /*Console.WriteLine(
                        "writing data from pos {0} to pos {1} length {2}",
                        bd.Width * x, p.ToInt32() - initPos, bd.Width);*/
                    Marshal.Copy(this.data, bd.Width * x, p, bd.Width);

                    if(x < (bd.Height - 1))
                        p = (IntPtr)((int)p + bd.Stride);
                }

                //Marshal.Copy(this.data, 0, p, bytesToCopy);
                i.UnlockBits(bd);

                //Console.WriteLine("bitmap created: w:{0} h:{1}", i.Width, i.Height);
              
                return i;
            }
        }

        private static ColorPalette GetGrayScalePalette() {
            Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);

            ColorPalette monoPalette = bmp.Palette;

            Color[] entries = monoPalette.Entries;

            for (int i = 0; i < 256; i++) {
                entries[i] = Color.FromArgb(i, i, i);
            }

            return monoPalette;
        }
    }

    public class CanonBitmapHeader {
        private byte width;
        private byte unknown;

        public const byte SIZE = 2;

        public CanonBitmapHeader(byte[] bytes) {
            if (bytes.Length != SIZE)
                throw new Exception("invalid header size");

            this.width = bytes[0];
            this.unknown = bytes[1];
        }

        public byte Width {
            get { return width; }
            set { width = value; }
        }

        public byte Unknown {
            get { return unknown; }
            set { unknown = value; }
        }
    }
}
