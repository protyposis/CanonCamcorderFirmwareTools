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


        public CanonBitmap(byte[] data) {
            if (data.Length <= 0)
                throw new Exception("no data");

            if (data.Length < CanonBitmapHeader.SIZE)
                throw new Exception("too few data (no header)");

            byte[] headerData = new byte[CanonBitmapHeader.SIZE];
            for (int x = 0; x < headerData.Length; x++)
                headerData[x] = data[x];

            this.header = new CanonBitmapHeader(headerData);

            byte[] imageData = new byte[data.Length - CanonBitmapHeader.SIZE];
            for (int x = CanonBitmapHeader.SIZE; x < data.Length; x++)
                imageData[x - CanonBitmapHeader.SIZE] = data[x];

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

                Bitmap i = new Bitmap(Width + fillupBytesTo4, Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                i.Palette = GetGrayScalePalette();
                Console.WriteLine("create bitmap w{0} h{1}", i.Width, i.Height);

                BitmapData bd = i.LockBits(new Rectangle(0, 0, i.Width, i.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                bd.Stride = i.Width;
                bd.Width = Width;
                bd.Height = Height;

                Console.WriteLine("bitmapdata w{0} h{1} stride{2}", bd.Width, bd.Height, bd.Stride);

                IntPtr p = bd.Scan0;

                for (int x = 0; x < bd.Height; x++) {

                    if (bd.Width * x >= data.Length)
                        break;

                    Marshal.Copy(this.data, bd.Width * x, p, bd.Width);

                    if(x < (bd.Height - 1))
                        p = (IntPtr)((int)p + (x + 1) * bd.Stride);
                }



                //Marshal.Copy(this.data, 0, p, bytesToCopy);
                i.UnlockBits(bd);
              
                return i;
            }
        }

        public static ColorPalette GetGrayScalePalette() {
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
        private byte unknown1, unknown2;
        private byte width;
        private byte unknown3;

        public const byte SIZE = 4;

        public CanonBitmapHeader(byte[] bytes) {
            if (bytes.Length != SIZE)
                throw new Exception("invalid header size");

            this.unknown1 = bytes[0];
            this.unknown2 = bytes[1];
            this.width = bytes[2];
            this.unknown3 = bytes[3];
        }

        public byte Width {
            get { return width; }
            set { width = value; }
        }

        public byte Unknown1 {
            get { return unknown1; }
            set { unknown1 = value; }
        }

        public byte Unknown2 {
            get { return unknown2; }
            set { unknown2 = value; }
        }

        public byte Unknown3 {
            get { return unknown3; }
            set { unknown3 = value; }
        }
    }
}
