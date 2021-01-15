using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Automation
{
    class MouseLowLevelHook : IDisposable
    {
        private readonly IntPtr MouseHandle;
        private bool disposed;
        private readonly LowLevelMouseProc callback;
        private delegate IntPtr LowLevelMouseProc(int code, IntPtr wParam, IntPtr lParam);
        public event EventHandler<HandledMouseEventArgs> MouseDown;
        public event EventHandler<HandledMouseEventArgs> MouseUp;
        public event EventHandler<HandledMouseEventArgs> MouseMove;
        public event EventHandler<HandledMouseEventArgs> MouseWheel;

        public const int LBUTTONDOWN = 0x0201;
        public const int LBUTTONUP = 0x0202;
        public const int MBUTTONDOWN = 0x0207;
        public const int MBUTTONUP = 0x0208;
        public const int RBUTTONDOWN = 0x0204;
        public const int RBUTTONUP = 0x0205;
        public const int XBUTTONDOWN = 0x020B;
        public const int XBUTTONUP = 0x020C;
        public const int MOUSEWHEEL = 0x020A;
        public const int MOUSEHWHEEL = 0x020E;
        public const int MOUSEMOVE = 0x0200;

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public int mouseData; // be careful, this must be ints, not uints (was wrong before I changed it...). regards, cmew.
            public int flags;
            public int time;
            public UIntPtr dwExtraInfo;
        }

        public MouseLowLevelHook() : base()
        {
            disposed = false;

            callback = new LowLevelMouseProc(HookCallback);
            MouseHandle = SetWindowsHookEx(14, callback, IntPtr.Zero, 0);

            if (MouseHandle == IntPtr.Zero)
            {
                int Win32Error = Marshal.GetLastWin32Error();
                throw new Win32Exception(Win32Error, "Failed to create mouse hook! (" + Win32Error + ")");
            }
        }

        public void Dispose()
        {
            if (disposed == true)
                throw new ObjectDisposedException("MouseHook");

            disposed = true;
            UnhookWindowsHookEx(MouseHandle);
        }

        ~MouseLowLevelHook()
        {
            // Me.Finalize()
            if (disposed == false)
                Dispose();
        }

        private IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code == 0)
            {
                MSLLHOOKSTRUCT MouseEventInfo = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                HandledMouseEventArgs MouseEvent = null;

                switch ((int)wParam)
                {
                    case LBUTTONDOWN:
                        {
                            MouseEvent = new HandledMouseEventArgs(MouseButtons.Left, 1, MouseEventInfo.pt.X, MouseEventInfo.pt.Y, 0, false);
                            MouseDown?.Invoke(this, MouseEvent);
                            break;
                        }

                    case LBUTTONUP:
                        {
                            MouseEvent = new HandledMouseEventArgs(MouseButtons.Left, 1, MouseEventInfo.pt.X, MouseEventInfo.pt.Y, 0, false);
                            MouseUp?.Invoke(this, MouseEvent);
                            break;
                        }

                    case MBUTTONDOWN:
                        {
                            MouseEvent = new HandledMouseEventArgs(MouseButtons.Middle, 1, MouseEventInfo.pt.X, MouseEventInfo.pt.Y, 0, false);
                            MouseDown?.Invoke(this, MouseEvent);
                            break;
                        }

                    case MBUTTONUP:
                        {
                            MouseEvent = new HandledMouseEventArgs(MouseButtons.Middle, 1, MouseEventInfo.pt.X, MouseEventInfo.pt.Y, 0, false);
                            MouseUp?.Invoke(this, MouseEvent);
                            break;
                        }

                    case RBUTTONDOWN:
                        {
                            MouseEvent = new HandledMouseEventArgs(MouseButtons.Right, 1, MouseEventInfo.pt.X, MouseEventInfo.pt.Y, 0, false);
                            MouseDown?.Invoke(this, MouseEvent);
                            break;
                        }

                    case RBUTTONUP:
                        {
                            MouseEvent = new HandledMouseEventArgs(MouseButtons.Right, 1, MouseEventInfo.pt.X, MouseEventInfo.pt.Y, 0, false);
                            MouseUp?.Invoke(this, MouseEvent);
                            break;
                        }

                    case XBUTTONDOWN:
                        {
                            MouseButtons XBtn = (MouseEventInfo.mouseData - 1) / (double)ushort.MaxValue == 1 ? MouseButtons.XButton1 : MouseButtons.XButton2;
                            MouseEvent = new HandledMouseEventArgs(XBtn, 1, MouseEventInfo.pt.X, MouseEventInfo.pt.Y, 0, false);
                            MouseUp?.Invoke(this, MouseEvent);
                            break;
                        }

                    case XBUTTONUP:
                        {
                            MouseButtons XBtn = (MouseEventInfo.mouseData - 1) / (double)ushort.MaxValue == 1 ? MouseButtons.XButton1 : MouseButtons.XButton2;
                            MouseEvent = new HandledMouseEventArgs(XBtn, 1, MouseEventInfo.pt.X, MouseEventInfo.pt.Y, 0, false);
                            MouseUp?.Invoke(this, MouseEvent);
                            break;
                        }

                    case MOUSEWHEEL:
                        {
                            int Delta = MouseEventInfo.mouseData > 0 ? 1 : -1;
                            MouseEvent = new HandledMouseEventArgs(MouseButtons.None, 0, MouseEventInfo.pt.X, MouseEventInfo.pt.Y, Delta, false);
                            MouseWheel?.Invoke(this, MouseEvent);
                            break;
                        }

                    case MOUSEHWHEEL:
                        {
                            int Delta = MouseEventInfo.mouseData > 0 ? 1 : -1;
                            MouseEvent = new HandledMouseEventArgs(MouseButtons.None, 0, MouseEventInfo.pt.X, MouseEventInfo.pt.Y, Delta, false);
                            MouseWheel?.Invoke(this, MouseEvent);
                            break;
                        }

                    case MOUSEMOVE:
                        {
                            MouseEvent = new HandledMouseEventArgs(MouseButtons.None, 0, MouseEventInfo.pt.X, MouseEventInfo.pt.Y, 0, false);
                            MouseMove?.Invoke(this, MouseEvent);
                            break;
                        }
                }

                if (MouseEvent != null && MouseEvent.Handled)
                    return new IntPtr(1);
            }

            return CallNextHookEx(MouseHandle, code, wParam, lParam);
        }
    }
}
