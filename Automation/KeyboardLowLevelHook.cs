using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Automation
{
    class KeyboardLowLevelHook
    {
        private readonly IntPtr KeyboardHandle;
        private bool disposed;
        private readonly KeyboardHookDelegate callback;
        private delegate IntPtr KeyboardHookDelegate(int Code, IntPtr wParam, IntPtr lParam);
        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public KBDLLHOOKSTRUCTFlags flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [Flags()]
        public enum KBDLLHOOKSTRUCTFlags : uint
        {
            LLKHF_EXTENDED = 0x1,
            LLKHF_INJECTED = 0x10,
            LLKHF_ALTDOWN = 0x20,
            LLKHF_UP = 0x80
        }

        public KeyboardLowLevelHook() : base()
        {
            disposed = false;

            callback = new KeyboardHookDelegate(KeyboardCallback);
            KeyboardHandle = SetWindowsHookEx(13, callback, IntPtr.Zero, 0);

            if (KeyboardHandle == IntPtr.Zero)
            {
                int Win32Error = Marshal.GetLastWin32Error();
                throw new Win32Exception(Win32Error, "Failed to create keboard hook! (" + Win32Error + ")");
            }
        }

        public virtual void Dispose()
        {
            if (disposed == true)
                throw new ObjectDisposedException("KeyboardHook");

            disposed = true;
            UnhookWindowsHookEx(KeyboardHandle);
        }

        ~KeyboardLowLevelHook()
        {
            // Me.Finalize()
            if (disposed == false)
                Dispose();
        }

        private IntPtr KeyboardCallback(int Code, IntPtr wParam, IntPtr lParam)
        {
            if (Code == 0)
            {
                KBDLLHOOKSTRUCT KeboardEventInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                Keys key = (Keys)Convert.ToInt32(KeboardEventInfo.vkCode);

                if ((GetAsyncKeyState(18) & 32768) != 0)
                    key |= Keys.Alt;

                if ((GetAsyncKeyState(17) & 32768) != 0)
                    key |= Keys.Control;

                if ((GetAsyncKeyState(16) & 32768) != 0)
                    key |= Keys.Shift;

                KeyEventArgs keyEventArg = new KeyEventArgs(key);

                if ((int)wParam == 256 | (int)wParam == 260)
                    KeyDown?.Invoke(this, keyEventArg);
                else if ((int)wParam == 257 | (int)wParam == 261)
                    KeyUp?.Invoke(this, keyEventArg);

                if (keyEventArg.Handled)
                    return new IntPtr(1);
            }

            return CallNextHookEx(KeyboardHandle, Code, wParam, lParam);
        }
    }
}
