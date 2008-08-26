using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing.Drawing2D;

namespace HF10_Bitmap_Viewer {
    public class CanonBitmap {
        protected int width;
        protected int height;

        protected byte[] data;

        private CanonHeader header;
        protected static ColorPalette palette = GetGrayScalePalette();

        private Image img;

        protected CanonBitmap() { }

        public CanonBitmap(byte[] data, long origin) {
            if (data.Length <= 0)
                throw new Exception("no data");

            if (data.Length < CanonBitmapHeader.SIZE)
                throw new Exception("too few data (no header)");

            byte[] headerData = new byte[CanonBitmapHeader.SIZE];
            Array.Copy(data, headerData, CanonBitmapHeader.SIZE);

            this.header = new CanonBitmapHeader(headerData, origin);

            byte[] imageData = new byte[data.Length - CanonBitmapHeader.SIZE];
            Array.Copy(data, CanonBitmapHeader.SIZE, imageData, 0, data.Length - CanonBitmapHeader.SIZE);

            this.data = imageData;

            if (this.header.Value1 <= 0)
                throw new Exception("invalid width (<=0)");

            this.width = this.header.Value1;
            this.height = (byte)(this.data.Length / this.width);
        }

        public CanonBitmap(CanonHeader header, byte[] data) {
            if (header == null)
                throw new NullReferenceException("header must not be null");

            if (data.Length <= 0)
                throw new Exception("no data");

            if (header.Value1 <= 0)
                throw new Exception("invalid width (<=0)");

            this.header = header;
            this.data = data;

            this.Width = header.Value1;
            this.Height = (byte)(data.Length / this.Width);
        }

        public int Width {
            get { return width; }
            set { width = value; }
        }

        public int Height {
            get { return height; }
            set { height = value; }
        }

        public CanonHeader Header {
            get { return header; }
        }

        public byte[] Data {
            get { return data; }
        }

        public Image Pic {
            get {
                if (img != null)
                    return img;
                //Console.WriteLine(
                //    "generating pic - w: {0}px h: {1}px data: {2}bytes (w*h={3}bytes)",
                //    width, height, data.Length, width * height);

                int bytesToCopy = (width * height < data.Length ? width * height : data.Length);
                int fillupBytesTo4 = 4 - (Width % 4); // every scanline must be dividable by 4 (msdn docs!!?)
                if (fillupBytesTo4 == 4) fillupBytesTo4 = 0;

                Bitmap i = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                i.Palette = palette;
                //Console.WriteLine("create bitmap w{0} h{1}", i.Width, i.Height);

                BitmapData bd = i.LockBits(new Rectangle(0, 0, i.Width, i.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                bd.Stride = i.Width + fillupBytesTo4;
                bd.Width = width;
                bd.Height = height;

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
                img = i;

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

            //entries[0xFF] = Color.Yellow;

            return monoPalette;
        }

        /// <summary>
        /// http://www.codeproject.com/KB/GDI-plus/imageresize.aspx
        /// modified by me
        /// </summary>
        /// <param name="imgPhoto"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static Image FixedSize(Image imgPhoto, int Width, int Height) {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            int destWidth = Width;
            int destHeight = Height;

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Black);
            grPhoto.InterpolationMode =
                    InterpolationMode.NearestNeighbor;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }
    }

    public class CanonBitmapHeader: CanonHeader {

        public CanonBitmapHeader(byte[] bytes, long origin) {
            if (bytes.Length != SIZE)
                throw new InvalidHeaderException("invalid header size");

            this.v1 = bytes[0];
            this.v2 = bytes[1];

            this.origin = origin;
        }

        public int Width {
            get { return v1; }
        }

        public int ScaledWidth {
            get { return v2; }
        }
    }
}
