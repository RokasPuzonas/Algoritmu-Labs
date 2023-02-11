using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Lab1
{
    internal struct Color
    {
        public byte r = 0, g = 0, b = 0;

        public Color(byte r, byte g, byte b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public Color(Color color)
        {
            this.r = color.r;
            this.g = color.g;
            this.b = color.b;
        }

        public override string ToString()
        {
            return $"Color({r}, {g}, {b})";
        }
    }

    internal class BMPImage
    {
        public uint width, height;
        public Color[] data;

        public BMPImage(uint width, uint height)
        {
            Debug.Assert(width > 0, "Width must be at least 1");
            Debug.Assert(height > 0, "Height must be at least 1");

            this.width = width;
            this.height = height;

            this.data = new Color[width * height];
        }

        public void SetPixel(int x, int y, int r, int g, int b)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return;
            data[width * y + x].r = (byte)r;
            data[width * y + x].g = (byte)g;
            data[width * y + x].b = (byte)b;
        }

        public Color GetPixel(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return new Color(0, 0, 0);
            return new Color(data[width * y + x]);
        }

        public void SetPixel(int x, int y, Color color)
        {
            SetPixel(x, y, color.r, color.g, color.b);
        }

        public void Fill(Color color)
        {
            Rectangle(0, 0, (int)width, (int)height, color);
        }

        public void Rectangle(int x, int y, int width, int height, Color color)
        {
            for (int dy = 0; dy < height; dy++)
            {
                for (int dx = 0; dx < width; dx++)
                {
                    SetPixel(x + dx, y + dy, color);
                }
            }
        }

        private static void swap(ref int a, ref int b)
        {
            int c = a;
            a = b;
            b = c;
        }

        public void Line(int x1, int y1, int x2, int y2, Color color)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;

            if (dx == 0 && dy == 0)
            {
                SetPixel(x1, y1, color);
                return;
            }

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                if (x1 > x2)
                {
                    swap(ref x1, ref x2);
                    swap(ref y1, ref y2);
                }

                for (int x = x1; x <= x2; ++x)
                {
                    int y = dy * (x - x1) / dx + y1;
                    SetPixel(x, y, color);
                }
            } else
            {
                if (y1 > y2)
                {
                    swap(ref x1, ref x2);
                    swap(ref y1, ref y2);
                }

                for (int y = y1; y <= y2; ++y)
                {
                    int x = dx * (y - y1) / dy + x1;
                    SetPixel(x, y, color);
                }
            }
        }

        private static void SortPointsByX(ref int x1, ref int y1, ref int x2, ref int y2, ref int x3, ref int y3)
        {
            if (x1 > x2)
            {
                swap(ref x1, ref x2);
                swap(ref y1, ref y2);
            }

            if (x1 > x3)
            {
                swap(ref x1, ref x3);
                swap(ref y1, ref y3);
            }

            if (x2 > x3)
            {
                swap(ref x2, ref x3);
                swap(ref y2, ref y3);
            }
        }

        public void Triangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
        {
            SortPointsByX(ref x1, ref y1, ref x2, ref y2, ref x3, ref y3);

            if (x2 - x1 == 0) return;
            if (x3 - x1 == 0) return;
            if (x3 - x2 == 0) return;
            if (x3 - x1 == 0) return;

            for (int x = x1; x <= x2; x++)
            {
                int yEdge1 = (y2 - y1) * (x - x1) / (x2 - x1) + y1;
                int yEdge2 = (y3 - y1) * (x - x1) / (x3 - x1) + y1;
                for (int y = Math.Min(yEdge1, yEdge2); y < Math.Max(yEdge1, yEdge2); y++)
                {
                    SetPixel(x, y, color);
                }
            }

            for (int x = x2; x <= x3; x++)
            {
                int yEdge1 = (y3 - y2) * (x - x2) / (x3 - x2) + y2;
                int yEdge2 = (y3 - y1) * (x - x1) / (x3 - x1) + y1;
                for (int y = Math.Min(yEdge1, yEdge2); y < Math.Max(yEdge1, yEdge2); y++)
                {
                    SetPixel(x, y, color);
                }
            }
        }

        public void Circle(int x, int y, int radius, Color color)
        {
            for (int dy = -(int)radius; dy < radius; dy++)
            {
                for (int dx = -(int)radius; dx < radius; dx++)
                {
                    if (dx*dx + dy*dy < radius*radius)
                    {
                        SetPixel(x + dx, y + dy, color);
                    }
                }
            }
        }

        private static void WriteBytes(FileStream f, params byte[] bytes)
        {
            f.Write(bytes);
        }

        private static void WriteUInt16(FileStream f, UInt16 number)
        {
            WriteBytes(f,
                (byte)((number >> 8 * 0) & 0xFF),
                (byte)((number >> 8 * 1) & 0xFF)
            );
        }
        private static void WriteUInt32(FileStream f, UInt32 number)
        {
            WriteBytes(f,
                (byte)((number >> 8 * 0) & 0xFF),
                (byte)((number >> 8 * 1) & 0xFF),
                (byte)((number >> 8 * 2) & 0xFF),
                (byte)((number >> 8 * 3) & 0xFF)
            );
        }

        private static ushort DetermineBPP(ICollection<Color> colors)
        {
            int count = colors.Count;
            if (count <= 2)
            {
                return 1;
            } else if (count <= 16)
            {
                return 4;
            } else if (count <= 256)
            {
                return 8;
            }
            return 24;
        }

        private static uint GetScanlineSize(uint width, uint bpp)
        {
            return ((width * bpp + 31) / 32 * 32) / 8u;
        }

        private byte[] EncodePixelData(uint bpp, List<Color> usedColors)
        {
            uint scanline = GetScanlineSize(width, bpp);
            byte[] pixelData = new byte[height * scanline];
            if (bpp == 1)
            {
                for (int y = 0; y < height; y++)
                {
                    int idx = 0;
                    for (int x = 0; x < width; x += 8)
                    {
                        byte pix1 = (byte)(usedColors.IndexOf(GetPixel(x + 0, y)) & 1);
                        byte pix2 = (byte)(usedColors.IndexOf(GetPixel(x + 1, y)) & 1);
                        byte pix3 = (byte)(usedColors.IndexOf(GetPixel(x + 2, y)) & 1);
                        byte pix4 = (byte)(usedColors.IndexOf(GetPixel(x + 3, y)) & 1);
                        byte pix5 = (byte)(usedColors.IndexOf(GetPixel(x + 4, y)) & 1);
                        byte pix6 = (byte)(usedColors.IndexOf(GetPixel(x + 5, y)) & 1);
                        byte pix7 = (byte)(usedColors.IndexOf(GetPixel(x + 6, y)) & 1);
                        byte pix8 = (byte)(usedColors.IndexOf(GetPixel(x + 7, y)) & 1);
                        pixelData[scanline * y + idx] = (byte)(pix1 << 7 | pix2 << 6 | pix3 << 5 | pix4 << 4 | pix5 << 3 | pix6 << 2 | pix7 << 1 | pix8 << 0);
                        idx++;
                    }
                }
            }
            else if (bpp == 4)
            {
                for (int y = 0; y < height; y++)
                {
                    int idx = 0;
                    for (int x = 0; x < width; x += 2)
                    {
                        byte pix1 = (byte)(usedColors.IndexOf(GetPixel(x + 0, y)) & 0xF);
                        byte pix2 = (byte)(usedColors.IndexOf(GetPixel(x + 1, y)) & 0xF);
                        pixelData[scanline * y + idx] = (byte)(pix1 << 4 | pix2 << 0);
                        idx++;
                    }
                }
            }
            else if (bpp == 8)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixelData[scanline * y + x] = (byte)(usedColors.IndexOf(GetPixel(x, y)) & 0xFF);
                    }
                }
            }
            else if (bpp == 24)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color color = GetPixel(x, y);
                        pixelData[scanline * y + 3 * x + 0] = color.b;
                        pixelData[scanline * y + 3 * x + 1] = color.g;
                        pixelData[scanline * y + 3 * x + 2] = color.r;
                    }
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            return pixelData;
        }

        public void Write(string filename)
        {
            List<Color> usedColors = new List<Color>();
            for (int i = 0; i < width*height; i++)
            {
                if (!usedColors.Contains(data[i]))
                {
                    usedColors.Add(data[i]);
                }
            }

            ushort bpp = DetermineBPP(usedColors);
            byte[] pixelData = EncodePixelData(bpp, usedColors);

            using (FileStream f = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                // Write header
                WriteBytes(f, 0x42, 0x4d); // signature
                WriteUInt32(f, 0);         // file size
                WriteUInt32(f, 0);         // reserved
                WriteUInt32(f, 0);         // data offset

                // Write info header
                WriteUInt32(f, 40);        // info header size, always 40
                WriteUInt32(f, width);     // width
                WriteUInt32(f, height);    // height
                WriteUInt16(f, 1);         // number of planes
                WriteUInt16(f, bpp);       // bits per pixel
                WriteUInt32(f, 0);         // compression
                WriteUInt32(f, 0);         // size of compressed image
                WriteUInt32(f, 0);         // x pixel per meter
                WriteUInt32(f, 0);         // y pixel per meter
                WriteUInt32(f, 0);         // colors used
                WriteUInt32(f, 0);         // important colors, 0 = all

                if (bpp <= 8)
                {
                    foreach (Color color in usedColors)
                    {
                        WriteBytes(f, color.b, color.g, color.r, 0);
                    }
                    for (int i = 0; i < Math.Pow(2, bpp) - usedColors.Count; i++)
                    {
                        WriteBytes(f, 0, 0, 0, 0);
                    }
                }

                // Write pixel data
                f.Write(pixelData);
            }
        }
    }
}
