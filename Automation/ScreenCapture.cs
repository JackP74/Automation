using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Automation
{
    class ScreenCapture
    {
        public struct SIZE
        {
            public int cx;
            public int cy;
        }

        public const int SRCCOPY = 13369376;
        public const int SM_CXSCREEN = 0;
        public const int SM_CYSCREEN = 1;

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern IntPtr DeleteDC(IntPtr hDc);

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        public static extern IntPtr DeleteObject(IntPtr hDc);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt")]
        public static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, int RasterOp);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);

        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(int abc);

        [DllImport("user32.dll", EntryPoint = "GetWindowDC")]
        public static extern IntPtr GetWindowDC(Int32 ptr);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

        public static Bitmap GetPrimaryScreenImage()
        {
            SIZE size;
            IntPtr hDC = GetDC(GetDesktopWindow());
            IntPtr hMemDC = CreateCompatibleDC(hDC);
            size.cx = GetSystemMetrics(SM_CXSCREEN);
            size.cy = GetSystemMetrics(SM_CYSCREEN);
            IntPtr m_HBitmap = CreateCompatibleBitmap(hDC, size.cx, size.cy);

            if (m_HBitmap != IntPtr.Zero)
            {
                IntPtr hOld = SelectObject(hMemDC, m_HBitmap);
                BitBlt(hMemDC, 0, 0, size.cx, size.cy, hDC, 0, 0, SRCCOPY);
                SelectObject(hMemDC, hOld);
                DeleteDC(hMemDC);
                ReleaseDC(GetDesktopWindow(), hDC);
                return Image.FromHbitmap(m_HBitmap);
            }

            return new Bitmap(1, 1);
        }

        public static Bitmap CaptureScreen(int SX, int SY, int Width, int Height)
        {
            IntPtr HdcSrc = CreateDC("DISPLAY", null, null, IntPtr.Zero);
            IntPtr HdcDest = CreateCompatibleDC(HdcSrc);
            IntPtr HBitmap = CreateCompatibleBitmap(HdcSrc, Width, Height);

            SelectObject(HdcDest, HBitmap);

            Bitmap Bmp = new Bitmap(Width, Height);
            Graphics G = Graphics.FromImage(Bmp);
            IntPtr Hdc = G.GetHdc();

            BitBlt(HdcDest, 0, 0, Width, Height, Hdc, 0, 0, 0xFF0062);

            G.Dispose();
            Bmp.Dispose();

            Screen[] Screendata = Screen.AllScreens;
            int X, X1, Y, Y1;

            for (int I = 0; I <= Screendata.Length - 1; I++)
            {
                if (Screendata[I].Bounds.X > (SX + Width) || (Screendata[I].Bounds.X + Screendata[I].Bounds.Width) < SX || Screendata[I].Bounds.Y > (SY + Height) || (Screendata[I].Bounds.Y + Screendata[I].Bounds.Height) < SY)
                {
                }
                else
                {
                    if (SX < Screendata[I].Bounds.X)
                        X = Screendata[I].Bounds.X;
                    else
                        X = SX;

                    if ((SX + Width) > (Screendata[I].Bounds.X + Screendata[I].Bounds.Width))
                        X1 = Screendata[I].Bounds.X + Screendata[I].Bounds.Width;
                    else
                        X1 = SX + Width;

                    if (SY < Screendata[I].Bounds.Y)
                        Y = Screendata[I].Bounds.Y;
                    else
                        Y = SY;

                    if ((SY + Height) > (Screendata[I].Bounds.Y + Screendata[I].Bounds.Height))
                        Y1 = Screendata[I].Bounds.Y + Screendata[I].Bounds.Height;
                    else
                        Y1 = SY + Height;

                    BitBlt(HdcDest, X - SX, Y - SY, X1 - X, Y1 - Y, HdcSrc, X, Y, 0x40000000 | 0xCC0020);
                }
            }

            Image FinalImage = Image.FromHbitmap(HBitmap);

            DeleteDC(HdcSrc);
            DeleteDC(HdcDest);
            DeleteObject(HBitmap);

            return (Bitmap)FinalImage;
        }

        public static Bitmap GetScreenImage()
        {
            int MinWidth = 0;
            int MinHeight = 0;

            int MaxWidth = 0;
            int MaxHeight = 0;

            var NrOfScreens = Screen.AllScreens.Count();

            for (int i = 0; i <= NrOfScreens - 1; i++)
            {
                var CurrentScreen = Screen.AllScreens[i];

                if (MinWidth > CurrentScreen.Bounds.X)
                    MinWidth = CurrentScreen.Bounds.X;

                if (MinHeight > CurrentScreen.Bounds.Y)
                    MinHeight = CurrentScreen.Bounds.Y;

                if (MaxWidth < CurrentScreen.Bounds.X + CurrentScreen.Bounds.Width + Math.Abs(MinWidth))
                    MaxWidth = CurrentScreen.Bounds.X + CurrentScreen.Bounds.Width + Math.Abs(MinWidth);

                if (MaxHeight < CurrentScreen.Bounds.Y + CurrentScreen.Bounds.Height + Math.Abs(MinHeight))
                    MaxHeight = CurrentScreen.Bounds.Y + CurrentScreen.Bounds.Height + Math.Abs(MinHeight);
            }

            return CaptureScreen(MinWidth, MinHeight, MaxWidth, MaxHeight);
        }
    }
}
