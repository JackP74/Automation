using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Automation
{
    public static class MouseHelper
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [Flags]
        public enum MouseEventFlags : uint
        {
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            MOVE = 0x0001,
            ABSOLUTE = 0x8000,
            RIGHTDOWN = 0x0008,
            RIGHTUP = 0x0010,
            WHEEL = 0x0800,
            XDOWN = 0x0080,
            XUP = 0x0100
        }

        public enum MouseEventDataXButtons : uint
        {
            XBUTTON1 = 0x0001,
            XBUTTON2 = 0x0002
        }

        public static void ClickMouse(MouseButtons Btn)
        {
            switch (Btn)
            {
                case MouseButtons.Left:
                    {
                        mouse_event((uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, 0);
                        mouse_event((uint)MouseEventFlags.LEFTUP, 0, 0, 0, 0);
                        break;
                    }

                case MouseButtons.Right:
                    {
                        mouse_event((uint)MouseEventFlags.RIGHTDOWN, 0, 0, 0, 0);
                        mouse_event((uint)MouseEventFlags.RIGHTUP, 0, 0, 0, 0);
                        break;
                    }

                case MouseButtons.Middle:
                    {
                        mouse_event((uint)MouseEventFlags.MIDDLEDOWN, 0, 0, 0, 0);
                        mouse_event((uint)MouseEventFlags.MIDDLEUP, 0, 0, 0, 0);
                        break;
                    }

                case MouseButtons.XButton1:
                    {
                        mouse_event((uint)MouseEventFlags.XDOWN, 0, 0, (uint)MouseEventDataXButtons.XBUTTON1, 0);
                        mouse_event((uint)MouseEventFlags.XUP, 0, 0, (uint)MouseEventDataXButtons.XBUTTON1, 0);
                        break;
                    }

                case MouseButtons.XButton2:
                    {
                        mouse_event((uint)MouseEventFlags.XDOWN, 0, 0, (uint)MouseEventDataXButtons.XBUTTON2, 0);
                        mouse_event((uint)MouseEventFlags.XUP, 0, 0, (uint)MouseEventDataXButtons.XBUTTON2, 0);
                        break;
                    }

                default:
                    return;
            }
        }

        public static void MoveMouse(int X, int Y, Action.MoveSpeed Speed)
        {
            switch (Speed)
            {
                case Action.MoveSpeed.Instant:
                    {
                        SetCursorPos(X, Y);
                        break;
                    }

                case Action.MoveSpeed.LineSlow:
                    {
                        MoveLine(X, Y, 25, 25);
                        break;
                    }

                case Action.MoveSpeed.LineFast:
                    {
                        MoveLine(X, Y, 5, 25);
                        break;
                    }

                case Action.MoveSpeed.CurveSlow:
                    {
                        SmoothMotion(X, Y, 25, 2);
                        break;
                    }

                case Action.MoveSpeed.CurveFast:
                    {
                        SmoothMotion(X, Y, 5, 2);
                        break;
                    }

                default:
                    return;
            }
        }

        private static void MoveLine(int X, int Y, int Sleep, int PointCount)
        {
            float CurTimeMs = 0;
            float TargetTimeMs = 1000;

            GetCursorPos(out POINT CPoint);

            float cX = CPoint.X;
            float cY = CPoint.Y;

            float oX = cX;
            float oY = cY;

            while (CurTimeMs < TargetTimeMs)
            {
                CurTimeMs += TargetTimeMs / PointCount;

                cX = UpdateLocation(oX, X, CurTimeMs / TargetTimeMs);
                cY = UpdateLocation(oY, Y, CurTimeMs / TargetTimeMs);

                SetCursorPos((int)cX, (int)cY);

                if (cX == oX && cY == oY)
                    break;

                Thread.Sleep(Sleep);

                if (Globals.ForceStop)
                    return;
            }

            SetCursorPos(X, Y);
        }

        private static void SmoothMotion(int X, int Y, int Sleep, int Speed)
        {
            GetCursorPos(out POINT OrgPt);

            float halfx = (X + OrgPt.X) / 2;
            float halfy = (Y + OrgPt.Y) / 2;

            float ptX = OrgPt.X + Sign(X - halfx);
            float ptY = OrgPt.Y + Sign(Y - halfy);

            while (Math.Abs(X - ptX) > 3 | Math.Abs(Y - ptY) > 3)
            {
                if (PtBetween(Convert.ToInt64(ptX), OrgPt.X, Convert.ToInt64(halfx), X))
                    ptX += ((X - ptX) / Speed);
                else
                    ptX += ((ptX - OrgPt.X) / Speed);

                if (PtBetween(Convert.ToInt64(ptY), OrgPt.Y, Convert.ToInt64(halfy), Y))
                    ptY += ((Y - ptY) / Speed);
                else
                    ptY += ((ptY - OrgPt.Y) / Speed);

                if (ptX == OrgPt.X && ptY == OrgPt.Y)
                    break;

                SetCursorPos((int)ptX, (int)ptY);
                Thread.Sleep(Sleep);

                if (Globals.ForceStop)
                    return;
            }

            SetCursorPos(X, Y);
        }

        private static float UpdateLocation(float Origin, float Destination, float Time)
        {
            return (Origin + (Destination - Origin) * Time);
        }

        private static long Sign(float X)
        {
            return X == 0 ? 0 : Convert.ToInt64(X / Math.Abs(X));
        }

        private static bool PtBetween(long X, long x0, long x1, long x2)
        {
            return x0 < x2 ? X > x1 : X < x1;
        }

        public static Point? ImageLocation(ref Bitmap Source, ref Bitmap bmp)
        {
            if (Source == null || bmp == null)
                return null;

            if (Source.Width == bmp.Width && Source.Height == bmp.Height)
            {
                if (Source.GetPixel(0, 0) == bmp.GetPixel(0, 0))
                    return new Point(0, 0);
                else
                    return null;
            }
            else if (Source.Width < bmp.Width || Source.Height < bmp.Height)
                return null;

            Rectangle sr = new Rectangle(0, 0, Source.Width, Source.Height);
            Rectangle br = new Rectangle(0, 0, bmp.Width, bmp.Height);

            BitmapData srcLock = Source.LockBits(sr, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData bmpLock = bmp.LockBits(br, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            int sStride = srcLock.Stride;
            int bStride = bmpLock.Stride;

            int srcSz = sStride * Source.Height;
            int bmpSz = bStride * bmp.Height;

            byte[] srcBuff = new byte[srcSz + 1];
            byte[] bmpBuff = new byte[bmpSz + 1];

            Marshal.Copy(srcLock.Scan0, srcBuff, 0, srcSz);
            Marshal.Copy(bmpLock.Scan0, bmpBuff, 0, bmpSz);
            bmp.UnlockBits(bmpLock);
            Source.UnlockBits(srcLock);

            int x, y, x2, y2, sx, sy, bx, by, sw, sh, bw, bh;
            byte r, g, b;
            Point? p = null;

            bw = bmp.Width;
            bh = bmp.Height;

            sw = Source.Width - bw;
            sh = Source.Height - bh;

            for (y = 0; y <= sh; y++)
            {
                sy = y * sStride;

                for (x = 0; x <= sw; x++)
                {
                    sx = sy + x * 3;
                    r = srcBuff[sx + 2];
                    g = srcBuff[sx + 1];
                    b = srcBuff[sx];

                    if (r == bmpBuff[2] && g == bmpBuff[1] && b == bmpBuff[0])
                    {
                        p = new Point(x, y);

                        for (y2 = 0; y2 <= bh - 1; y2++)
                        {
                            by = y2 * bStride;

                            for (x2 = 0; x2 <= bw - 1; x2++)
                            {
                                bx = by + x2 * 3;
                                sy = (y + y2) * sStride;
                                sx = sy + (x + x2) * 3;
                                r = srcBuff[sx + 2];
                                g = srcBuff[sx + 1];
                                b = srcBuff[sx];

                                if (!(r == bmpBuff[bx + 2] && g == bmpBuff[bx + 1] && b == bmpBuff[bx]))
                                {
                                    p = null;
                                    sy = y * sStride;
                                    break;
                                }
                            }

                            if (p == null)
                                break;
                        }
                    }

                    if (p != null)
                        break;
                }

                if (p != null)
                    break;
            }

            return p;
        }

        public static Point? GetImagePoint(Point? OrgPoint, Size ImgSize, Action.Location LocType)
        {
            if (!OrgPoint.HasValue)
                return null;

            switch (LocType)
            {
                case Action.Location.LeftTop:
                    return OrgPoint;

                case Action.Location.LeftMiddle:
                    return new Point(OrgPoint.Value.X, OrgPoint.Value.Y + (ImgSize.Height / 2));

                case Action.Location.LeftBottom:
                    return new Point(OrgPoint.Value.X, OrgPoint.Value.Y + ImgSize.Height);

                case Action.Location.MiddleTop:
                    return new Point(OrgPoint.Value.X + (ImgSize.Width / 2), OrgPoint.Value.Y);

                case Action.Location.Middle:
                    return new Point(OrgPoint.Value.X + (ImgSize.Width / 2), OrgPoint.Value.Y + (ImgSize.Height / 2));

                case Action.Location.MiddleBottom:
                    return new Point(OrgPoint.Value.X + (ImgSize.Width / 2), OrgPoint.Value.Y + ImgSize.Height);

                case Action.Location.RightTop:
                    return new Point(OrgPoint.Value.X + ImgSize.Width, OrgPoint.Value.Y);

                case Action.Location.RightMiddle:
                    return new Point(OrgPoint.Value.X + ImgSize.Width, OrgPoint.Value.Y + (ImgSize.Height / 2));

                case Action.Location.RightBottom:
                    return new Point(OrgPoint.Value.X + ImgSize.Width, OrgPoint.Value.Y + ImgSize.Height);

                default:
                    return OrgPoint;
            }
        }
    }
}
