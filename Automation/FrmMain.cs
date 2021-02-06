using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;

using MessageCustomHandler;

namespace Automation
{
    public partial class FrmMain : Form
    {
        #region "Variables"
        private readonly KeyboardLowLevelHook KeyBoardHook = new KeyboardLowLevelHook();
        private readonly MouseLowLevelHook MouseHook = new MouseLowLevelHook();

        private bool ActionInProgress = false;
        private bool ForceStop = false;
        private bool SetTxtPosition = false;
        private bool Listen = true;

        private Actions ActionList = new Actions();
        private readonly XmlSerializer MoveXML = new XmlSerializer(typeof(Actions));
        private readonly Action TempMoveAction = new Action();

        private readonly string SavePath = Path.Combine(Application.StartupPath, "list.xml");
        private readonly Random Rnd = new Random(Guid.NewGuid().GetHashCode());

        private bool Record = false;
        private bool ShiftDown = false;
        private readonly Recorder Recording = new Recorder();
        private readonly ImageRecorder ImageBaseRecorder = new ImageRecorder();
        #endregion

        #region "Win32"
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();
        #endregion

        #region "Enums"
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
        #endregion

        #region "Functions"
        public FrmMain()
        {
            InitializeComponent();

            KeyBoardHook.KeyDown += KeyBoardHook_KeyDown;
            KeyBoardHook.KeyUp += KeyBoardHook_KeyUp;

            MouseHook.MouseMove += MouseHook_MouseMove;
            MouseHook.MouseUp += MouseHook_MouseUp;
            MouseHook.MouseWheel += MouseHook_MouseWheel;
        }

        private void StartActions(List<Action> ActionList)
        {
            foreach (Action CurrentAction in ActionList)
            {
                if (ForceStop)
                    break;

                switch (CurrentAction.ActionType)
                {
                    case Action.Type.Mouse:
                        {
                            for (int I = 1; I <= CurrentAction.LoopCount; I++)
                            {
                                Thread.Sleep(CurrentAction.WaitTime);

                                int NewX = CurrentAction.MovePoint.X;
                                int NewY = CurrentAction.MovePoint.Y;

                                if (CurrentAction.MoveType == Action.MoveCalcType.Relative)
                                {
                                    GetCursorPos(out POINT CurrentPoint);

                                    NewX = CurrentPoint.X + NewX;
                                    NewY = CurrentPoint.Y + NewY;
                                }

                                MoveMouse(NewX, NewY, CurrentAction.MouseSpeed);

                                if (CurrentAction.AutoClick)
                                    ClickMouse(CurrentAction.MouseBtn);

                                if (ForceStop)
                                    break;
                            }

                            break;
                        }

                    case Action.Type.KeyPress:
                        {
                            for (int I = 1; I <= CurrentAction.LoopCount; I++)
                            {
                                Thread.Sleep(CurrentAction.WaitTime);

                                SendKeys.SendWait(KeyToString(CurrentAction.ActionKey));

                                if (ForceStop)
                                    break;
                            }

                            break;
                        }

                    case Action.Type.ImageSearch:
                        {
                            for (int I = 1; I <= CurrentAction.LoopCount; I++)
                            {
                                Thread.Sleep(CurrentAction.WaitTime);

                                Bitmap Screenshot = ScreenCapture.GetScreenImage();
                                Bitmap ImgSource = (Bitmap)Image.FromFile(CurrentAction.ImagePath);
                                Point? TopLeftPoint = ImageLocation(ref Screenshot, ref ImgSource);

                                if (!TopLeftPoint.HasValue)
                                {
                                    while (!TopLeftPoint.HasValue)
                                    {
                                        Thread.Sleep(500);

                                        Screenshot = ScreenCapture.GetScreenImage();
                                        TopLeftPoint = ImageLocation(ref Screenshot, ref ImgSource);

                                        if (ForceStop)
                                            break;
                                    }
                                }

                                if (ForceStop)
                                    break;

                                int MinX = 0;
                                int MinY = 0;

                                var NrOfScreens = Screen.AllScreens.Count();

                                for (int i = 0; i <= NrOfScreens - 1; i++)
                                {
                                    var CurrentScreen = Screen.AllScreens[i];

                                    if (MinX > CurrentScreen.Bounds.X)
                                        MinX = CurrentScreen.Bounds.X;

                                    if (MinY > CurrentScreen.Bounds.Y)
                                        MinY = CurrentScreen.Bounds.Y;
                                }

                                int OffsetX = Math.Abs(0 - MinX);
                                int OffsetY = Math.Abs(0 - MinY);

                                int RealX = TopLeftPoint.Value.X - OffsetX;
                                int RealY = TopLeftPoint.Value.Y - OffsetY;

                                TopLeftPoint = new Point(RealX, RealY);

                                Point? FinalPoint = GetImagePoint(TopLeftPoint, ImgSource.Size, CurrentAction.ImageLoc);

                                if (!FinalPoint.HasValue)
                                    continue;

                                MoveMouse(FinalPoint.Value.X, FinalPoint.Value.Y, CurrentAction.MouseSpeed);

                                Screenshot.Dispose();
                                ImgSource.Dispose();

                                if (ForceStop)
                                    break;
                            }

                            break;
                        }

                    default:
                        {
                            continue;
                        }
                }
            }
        }

        private void SaveList()
        {
            try
            {
                FileStream StreamOut = new FileStream(SavePath, FileMode.Create, FileAccess.Write, FileShare.None);
                MoveXML.Serialize(StreamOut, ActionList);

                StreamOut.Dispose();
            }
            catch (Exception ex)
            {
                CMBox.Show("Error", "Couldn't save, Error: " + ex.Message, Style.Error, Buttons.OK, ex.ToString());
            }
            
        }

        private void LoadList()
        {
            try
            {
                if (!File.Exists(SavePath))
                    return;

                FileStream StreamIn = new FileStream(SavePath, FileMode.Open, FileAccess.Read, FileShare.None);
                ActionList = (Actions)MoveXML.Deserialize(StreamIn);

                RefreshList();

                StreamIn.Dispose();
            }
            catch (Exception ex)
            {
                CMBox.Show("Error", "Couldn't load, Error: " + ex.Message, Style.Error, Buttons.OK, ex.ToString());
            }
        }

        private void RefreshList()
        {
            ListMain.Items.Clear();
            ActionList.Items = ActionList.Items.OrderBy(x => x.OnKey).ThenBy(x => x.Order).ToList();

            foreach (Action Itm in ActionList.Items)
            {
                switch (Itm.ActionType)
                {
                    case Action.Type.Mouse:
                        {
                            ListMain.Items.Add($"{Itm.OnKey}").SubItems.AddRange(new[] {
                                $"{Itm.Order}", "Mouse", $"P: {Itm.MovePoint.X}x{Itm.MovePoint.Y} M: {Itm.MouseBtn}",
                                $"{Itm.LoopCount}", $"{Itm.WaitTime}", $"T: {Itm.MoveType} S: {Itm.MouseSpeed} AC: {Itm.AutoClick}" 
                            });
                            break;
                        }

                    case Action.Type.KeyPress:
                        {
                            ListMain.Items.Add($"{Itm.OnKey}").SubItems.AddRange(new[] {
                                $"{Itm.Order}", "Key Press", $"Key: {Itm.ActionKey}", $"{Itm.LoopCount}", $"{Itm.WaitTime}", "-"
                            });
                            break;
                        }

                    case Action.Type.ImageSearch:
                        {
                            ListMain.Items.Add($"{Itm.OnKey}").SubItems.AddRange(new[] {
                                $"{Itm.Order}", "Image Search", $"Image: {GetName(Itm.ImagePath)}", $"{Itm.LoopCount}",
                                $"{Itm.WaitTime}", $"L: {Itm.ImageLoc} S: {Itm.MouseSpeed} AC: {Itm.AutoClick}"
                            });
                            break;
                        }

                    default:
                        {
                            continue;
                        }
                }
            }
        }

        public void AddRecording(Keys NewKey)
        {
            if (!Record)
                return;

            if (NewKey == Keys.Pause)
            {
                BtnRecord.Text = "Record";

                Record = false;
                Listen = true;
                List<Action> NewItems = new List<Action>();

                NewItems.AddRange(Recording.GetItems());
                NewItems.AddRange(ImageBaseRecorder.GetItems());
                NewItems = NewItems.OrderBy(x => x.Order).ToList();

                Recording.Stop();
                ImageBaseRecorder.Stop();

                ActionList.Items.AddRange(NewItems);
                SaveList();
                RefreshList();

                GC.Collect();
                GC.WaitForPendingFinalizers();

                return;
            }

            Recording.Add(NewKey);
        }

        public void AddRecording(MouseData.Type NewKind, Point NewPoint, MouseButtons NewBtn)
        {
            if (!Record)
                return;

            Recording.Add(NewKind, NewPoint, NewBtn);
        }

        public void AddRecording(MouseButtons NewBtn)
        {
            if (!Record)
                return;

            ImageBaseRecorder.NewRecord(NewBtn);
        }

        private string GetName(string FullPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FullPath))
                    return string.Empty;

                return new FileInfo(FullPath).Name;
            }
            catch
            {
                return FullPath;
            }
        }

        private void StartThread(ThreadStart NewStart)
        {
            Thread NewThread = new Thread(NewStart) { IsBackground = true };
            NewThread.SetApartmentState(ApartmentState.STA);
            NewThread.Start();
        }

        private void ClickMouse(MouseButtons Btn)
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
                    {
                        return;
                    }
            }
        }

        private void MoveMouse(int X, int Y, Action.MoveSpeed Speed)
        {
            switch (Speed)
            {
                case Action.MoveSpeed.Instant:
                    {
                        SetCursorPos(X, Y);
                        break;
                    }

                case  Action.MoveSpeed.LineSlow:
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
                    {
                        return;
                    }
            }
        }

        private void MoveLine(int X, int Y, int Sleep, int PointCount)
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

                if (ForceStop)
                    return;
            }

            SetCursorPos(X, Y);
        }

        private void SmoothMotion(int X, int Y, int Sleep, int Speed)
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

                if (ForceStop)
                    return;
            }

            SetCursorPos(X, Y);
        }

        private float UpdateLocation(float Origin, float Destination, float Time)
        {
            return (Origin + (Destination - Origin) * Time);
        }

        private long Sign(Single X)
        {
            return X == 0 ? 0 : Convert.ToInt64(X / Math.Abs(X));
        }

        private bool PtBetween(long X, long x0, long x1, long x2)
        {
            return x0 < x2 ? X > x1 : X < x1;
        }

        private string KeyToString(Keys Key)
        {
            switch (Key)
            {
                case Keys.Add:
                    {
                        return "+";
                    }

                case Keys.Decimal:
                    {
                        return ".";
                    }

                case Keys.Divide:
                    {
                        return "/";
                    }

                case Keys.Multiply:
                    {
                        return "*";
                    }

                case Keys.OemBackslash:
                    {
                        return @"\";
                    }

                case Keys.OemCloseBrackets:
                    {
                        return "]";
                    }

                case Keys.OemMinus:
                    {
                        return "-";
                    }

                case Keys.OemOpenBrackets:
                    {
                        return "[";
                    }

                case Keys.OemPeriod:
                    {
                        return ".";
                    }

                case Keys.OemPipe:
                    {
                        return "|";
                    }

                case Keys.OemQuestion:
                    {
                        return "/";
                    }

                case Keys.OemQuotes:
                    {
                        return "\"";
                    }

                case Keys.OemSemicolon:
                    {
                        return ";";
                    }

                case Keys.Oemcomma:
                    {
                        return ",";
                    }

                case Keys.Oemplus:
                    {
                        return "+";
                    }

                case Keys.Oemtilde:
                    {
                        return "`";
                    }

                case Keys.Separator:
                    {
                        return "-";
                    }

                case Keys.Subtract:
                    {
                        return "-";
                    }

                case Keys.D0:
                    {
                        return "0";
                    }

                case Keys.D1:
                    {
                        return "1";
                    }

                case Keys.D2:
                    {
                        return "2";
                    }

                case Keys.D3:
                    {
                        return "3";
                    }

                case Keys.D4:
                    {
                        return "4";
                    }

                case Keys.D5:
                    {
                        return "5";
                    }

                case Keys.D6:
                    {
                        return "6";
                    }

                case Keys.D7:
                    {
                        return "7";
                    }

                case Keys.D8:
                    {
                        return "8";
                    }

                case Keys.D9:
                    {
                        return "9";
                    }

                case Keys.NumPad0:
                    {
                        return "0";
                    }

                case Keys.NumPad1:
                    {
                        return "1";
                    }

                case Keys.NumPad2:
                    {
                        return "2";
                    }

                case Keys.NumPad3:
                    {
                        return "3";
                    }

                case Keys.NumPad4:
                    {
                        return "4";
                    }

                case Keys.NumPad5:
                    {
                        return "5";
                    }

                case Keys.NumPad6:
                    {
                        return "6";
                    }

                case Keys.NumPad7:
                    {
                        return "7";
                    }

                case Keys.NumPad8:
                    {
                        return "8";
                    }

                case Keys.NumPad9:
                    {
                        return "9";
                    }

                case Keys.Space:
                    {
                        return " ";
                    }

                case Keys.Return:
                    {
                        return "{ENTER}";
                    }

                default:
                    {
                        return Key.ToString().ToLower();
                    }
            }
        }

        private int RandomNumber(int Min, int Max)
        {
            return Rnd.Next(Min, Max + 1);
        }

        private Point? ImageLocation(ref Bitmap Source, ref Bitmap bmp)
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

                            if (p == null )
                                break;
                        }
                    }

                    if (p != null )
                        break;
                }

                if (p != null)
                    break;
            }

            return p;
        }

        private Point? GetImagePoint(Point? OrgPoint, Size ImgSize, Action.Location LocType)
        {
            if (!OrgPoint.HasValue)
                return null;

            switch (LocType)
            {
                case Action.Location.LeftTop:
                    {
                        return OrgPoint;
                    }

                case Action.Location.LeftMiddle:
                    {
                        return new Point(OrgPoint.Value.X, OrgPoint.Value.Y + (ImgSize.Height / 2));
                    }

                case Action.Location.LeftBottom:
                    {
                        return new Point(OrgPoint.Value.X, OrgPoint.Value.Y + ImgSize.Height);
                    }

                case Action.Location.MiddleTop:
                    {
                        return new Point(OrgPoint.Value.X + (ImgSize.Width / 2), OrgPoint.Value.Y);
                    }

                case Action.Location.Middle:
                    {
                        return new Point(OrgPoint.Value.X + (ImgSize.Width / 2), OrgPoint.Value.Y + (ImgSize.Height / 2));
                    }

                case Action.Location.MiddleBottom:
                    {
                        return new Point(OrgPoint.Value.X + (ImgSize.Width / 2), OrgPoint.Value.Y + ImgSize.Height);
                    }

                case Action.Location.RightTop:
                    {
                        return new Point(OrgPoint.Value.X + ImgSize.Width, OrgPoint.Value.Y);
                    }

                case Action.Location.RightMiddle:
                    {
                        return new Point(OrgPoint.Value.X + ImgSize.Width, OrgPoint.Value.Y + (ImgSize.Height / 2));
                    }

                case Action.Location.RightBottom:
                    {
                        return new Point(OrgPoint.Value.X + ImgSize.Width, OrgPoint.Value.Y + ImgSize.Height);
                    }

                default:
                    {
                        return OrgPoint;
                    }
            }
        }
        #endregion

        #region "Handles"
        private void FrmMain_Load(object sender, EventArgs e)
        {
            LoadList();

            TempMoveAction.OnKey = Keys.Escape;
            TempMoveAction.MoveType = Action.MoveCalcType.Relative;
            TempMoveAction.MovePoint = new Point(0, 0);
            TempMoveAction.AutoClick = false;

            if (ListActionType.Items.Count > 0)
                ListActionType.SelectedIndex = 0;

            if (ListMoveType.Items.Count > 0)
                ListMoveType.SelectedIndex = 0;

            if (ListMoveSpeed.Items.Count > 0)
                ListMoveSpeed.SelectedIndex = 0;

            if (ListImageLoc.Items.Count > 0)
                ListImageLoc.SelectedIndex = 0;
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            KeyBoardHook.Dispose();
            MouseHook.Dispose();
        }

        private void KeyBoardHook_KeyDown(object sender, KeyEventArgs e)
        {
            // For recording
            if (Record && e.KeyCode == Keys.LShiftKey)
                ShiftDown = true;

            if (Record && e.KeyCode == Keys.LMenu)
            {
                ImageBaseRecorder.ShowForm = true;
            }
        }

        private void KeyBoardHook_KeyUp(object sender, KeyEventArgs e)
        {
            // For recording
            if (e.KeyCode == Keys.LShiftKey)
            {
                ShiftDown = false;
                return;
            }

            if (Record && e.KeyCode == Keys.LMenu)
            {
                ImageBaseRecorder.ShowForm = false;
                return;
            }

            // WinKey ignored
            if (e.KeyCode == Keys.LWin || e.KeyCode == Keys.RWin)
                return;

            // End is force stop
            if (e.KeyCode == Keys.End)
            {
                ForceStop = true;
                return;
            }

            // Print screen for saving the screen
            if (e.KeyCode == Keys.PrintScreen)
            {
                Bitmap Screenshot = ScreenCapture.GetScreenImage();
                Screenshot.Save(Path.Combine(Application.StartupPath, "screen.png"), ImageFormat.Png);

                Screenshot.Dispose();

                return;
            }

            // Set key for new action
            if (BtnOnKey.Text == "Cancel" && TxtOnKey.Text == "Set key..." && GetActiveWindow() == this.Handle)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        {
                            TxtOnKey.Text = TempMoveAction.OnKey == Keys.Escape ? string.Empty : TempMoveAction.OnKey.ToString();
                            BtnOnKey.Text = "Set";
                            break;
                        }

                    default:
                        {
                            TempMoveAction.OnKey = e.KeyCode;
                            TxtOnKey.Text = TempMoveAction.OnKey == Keys.Escape ? string.Empty : TempMoveAction.OnKey.ToString();
                            BtnOnKey.Text = "Set";
                            break;
                        }
                }
                return;
            }

            // Set what key to send
            if (BtnNewKey.Text == "Cancel" && TxtNewAction.Text == "Set key..." && GetActiveWindow() == this.Handle)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        {
                            TxtNewAction.Text = TempMoveAction.ActionKey == Keys.Escape ? string.Empty : TempMoveAction.ActionKey.ToString();
                            BtnNewKey.Text = "Set";
                            break;
                        }

                    default:
                        {
                            TempMoveAction.ActionKey = e.KeyCode;
                            TxtNewAction.Text = TempMoveAction.ActionKey == Keys.Escape ? string.Empty : TempMoveAction.ActionKey.ToString();
                            BtnNewKey.Text = "Set";
                            break;
                        }
                }
                return;
            }

            // Start action
            if (Listen && GetActiveWindow() != this.Handle)
            {
                if (ActionInProgress)
                    return;

                List<Action> CurrentList = ActionList.Items.Where(X => X.OnKey == e.KeyCode).ToList();

                if (CurrentList.Count < 1)
                    return;

                if (ChkLoopKey.Checked)
                {
                    StartThread(() =>
                    {
                        ForceStop = false;
                        ActionInProgress = true;

                        while (!ForceStop)
                            StartActions(CurrentList);

                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        ActionInProgress = false;
                        ForceStop = false;
                    });
                }
                else
                {
                    StartThread(() =>
                    {
                        ForceStop = false;
                        ActionInProgress = true;

                        StartActions(CurrentList);

                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        ActionInProgress = false;
                        ForceStop = false;
                    });
                }
                return;
            }

            // Recording actions
            if (Record)
            {
                AddRecording(e.KeyCode);
                return;
            }
        }

        private void MouseHook_MouseMove(object sender, HandledMouseEventArgs e)
        {
            if (SetTxtPosition)
            {
                TxtNewAction.Text = $"{e.X} {e.Y}";
                return;
            }

            if (Record)
                ImageBaseRecorder.SetFormLoc(e.Location);
        }

        private void MouseHook_MouseUp(object sender, HandledMouseEventArgs e)
        {
            if (ImageBaseRecorder.ShowForm)
            {
                AddRecording(e.Button);
            }
            else
            {
                AddRecording(MouseData.Type.Click, e.Location, e.Button);
            }
        }

        private void MouseHook_MouseWheel(object sender, HandledMouseEventArgs e)
        {
            int NewWidth = 0;
            int NewHeight = 0;

            if (e.Delta < 0)
            {
                if (ShiftDown == true)
                    NewWidth = -5;
                else
                    NewHeight = -5;
            }
            else if (e.Delta > 0)
            {
                if (ShiftDown == true)
                    NewWidth = 5;
                else
                    NewHeight = 5;
            }

            ImageBaseRecorder.NewSize(NewWidth, NewHeight, e.Location);
        }

        private void ContextListMainDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (ListMain.SelectedItems.Count <= 0)
                    return;

                List<string> KeyList = ListMain.SelectedItems.OfType<ListViewItem>().Select((X) => $"{X.SubItems[0].Text}:{X.SubItems[1].Text}").ToList();
                ActionList.Items = ActionList.Items.Where(X => !KeyList.Contains($"{X.OnKey}:{X.Order}")).ToList();

                RefreshList();
                SaveList();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                CMBox.Show("Error", "Couldn't delete, Error: " + ex.Message, Style.Error, Buttons.OK, ex.ToString());
            }
        }

        private void ListActionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetTxtPosition = false;

            switch (ListActionType.SelectedIndex)
            {
                case 0:
                    {
                        BtnNewKey.Visible = false;
                        ChkClick.Visible = true;
                        BtnImageBrw.Visible = false;

                        TempMoveAction.ActionType = Action.Type.Mouse;
                        LabelMoveType.Visible = true;
                        LabelMoveSpeed.Visible = true;
                        LabelImageLoc.Visible = false;
                        ListMoveType.Visible = true;
                        ListMoveSpeed.Visible = true;
                        ListImageLoc.Visible = false;

                        TxtNewAction.Text = TempMoveAction.MovePoint.X + " " + TempMoveAction.MovePoint.Y;
                        TxtNewAction.ReadOnly = false;
                        TxtNewAction.BackColor = SystemColors.Window;

                        BtnNewKey.Text = "Set";
                        break;
                    }

                case 1:
                    {
                        ChkClick.Visible = false;
                        BtnNewKey.Visible = true;
                        BtnImageBrw.Visible = false;

                        TempMoveAction.ActionType = Action.Type.KeyPress;
                        LabelMoveType.Visible = false;
                        LabelMoveSpeed.Visible = false;
                        LabelImageLoc.Visible = false;
                        ListMoveType.Visible = false;
                        ListMoveSpeed.Visible = false;
                        ListImageLoc.Visible = false;

                        TxtNewAction.Text = TempMoveAction.ActionKey == Keys.Escape ? string.Empty : TempMoveAction.ActionKey.ToString();
                        TxtNewAction.ReadOnly = true;
                        TxtNewAction.BackColor = SystemColors.Window;

                        BtnNewKey.Text = "Set";
                        break;
                    }

                case 2:
                    {
                        ChkClick.Visible = false;
                        BtnNewKey.Visible = false;
                        BtnImageBrw.Visible = true;

                        TempMoveAction.ActionType = Action.Type.ImageSearch;
                        LabelMoveType.Visible = false;
                        LabelMoveSpeed.Visible = true;
                        LabelImageLoc.Visible = true;
                        ListMoveType.Visible = false;
                        ListMoveSpeed.Visible = true;
                        ListImageLoc.Visible = true;

                        TxtNewAction.Text = GetName(TempMoveAction.ImagePath);
                        TxtNewAction.ReadOnly = true;
                        TxtNewAction.BackColor = SystemColors.Window;

                        BtnNewKey.Text = "Set";
                        break;
                    }

                default:
                    {
                        ListActionType.SelectedIndex = 0;
                        break;
                    }
            }
        }

        private void BtnOnKey_Click(object sender, EventArgs e)
        {
            if (BtnOnKey.Text == "Set")
            {
                if (BtnNewKey.Text != "Set")
                {
                    TxtNewAction.Text = TempMoveAction.ActionKey == Keys.Escape ? string.Empty : TempMoveAction.ActionKey.ToString();
                    BtnNewKey.Text = "Set";
                }

                TxtOnKey.Text = "Set key...";
                BtnOnKey.Text = "Cancel";
                TxtOnKey.Focus();
            }
            else
            {
                TxtOnKey.Text = TempMoveAction.OnKey == Keys.Escape ? string.Empty : TempMoveAction.OnKey.ToString();
                BtnOnKey.Text = "Set";
            }
        }

        private void ChkClick_CheckedChanged(object sender, EventArgs e)
        {
            TempMoveAction.AutoClick = ChkClick.Checked;
        }

        private void BtnNewKey_Click(object sender, EventArgs e)
        {
            if (BtnNewKey.Text == "Set")
            {
                if (BtnOnKey.Text != "Set")
                {
                    TxtOnKey.Text = TempMoveAction.OnKey == Keys.Escape ? string.Empty : TempMoveAction.OnKey.ToString();
                    BtnOnKey.Text = "Set";
                }

                TxtNewAction.Text = "Set key...";
                BtnNewKey.Text = "Cancel";
                TxtNewAction.Focus();
            }
            else
            {
                TxtNewAction.Text = TempMoveAction.ActionKey == Keys.Escape ? string.Empty : TempMoveAction.ActionKey.ToString();
                BtnNewKey.Text = "Set";
            }
        }

        private void BtnImageBrw_Click(object sender, EventArgs e)
        {
            OpenFileDialog NewImage = new OpenFileDialog()
            {
                Title = "Select Image...",
                Multiselect = false,
                CheckFileExists = true,
                SupportMultiDottedExtensions = false,
                Filter = "Image File|*.png;*.bmp;*.jpg;*.jpeg"
            };

            if (NewImage.ShowDialog() == DialogResult.OK)
            {
                TempMoveAction.ImagePath = NewImage.FileName;
                TxtNewAction.Text = GetName(NewImage.FileName);
            }
        }

        private void TxtNewAction_KeyDown(object sender, KeyEventArgs e)
        {
            if (!TxtNewAction.ReadOnly && e.KeyCode == Keys.Menu)
                SetTxtPosition = true;
        }

        private void TxtNewAction_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu)
                SetTxtPosition = false;
        }

        private void TxtNewAction_KeyPress(object sender, KeyPressEventArgs e)
        {
            // only digits
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ' ') && (e.KeyChar != '-'))
            {
                e.Handled = true;
                SystemSounds.Beep.Play();
            }
            // no more than one space
            else if (e.KeyChar == ' ')
            {
                if (TxtNewAction.Text.IndexOf(" ") > -1)
                {
                    e.Handled = true;
                    SystemSounds.Beep.Play();
                }
            }
            else if (e.KeyChar == '-')
            {
                // no more than 2 minuses
                if (TxtNewAction.Text.Count(x => x == '-') >= 2) 
                {
                    e.Handled = true;
                    SystemSounds.Beep.Play();
                }
                // minus as first or after space
                else if (TxtNewAction.SelectionStart > 0 && TxtNewAction.Text.Substring(TxtNewAction.SelectionStart - 1, 1) != " ")
                {
                    e.Handled = true;
                    SystemSounds.Beep.Play();
                }
                // no minus minus
                else if (TxtNewAction.SelectionStart == 0 && TxtNewAction.Text.Length > 0 && TxtNewAction.Text.Substring(TxtNewAction.SelectionStart, 1) == "-")
                {
                    e.Handled = true;
                    SystemSounds.Beep.Play();
                }
            }
        }

        private void TxtNewAction_TextChanged(object sender, EventArgs e)
        {
            if (TempMoveAction.ActionType != Action.Type.Mouse)
                return;

            string[] Inputs = TxtNewAction.Text.Split(' ');

            if (Inputs.Count() == 2 && !string.IsNullOrWhiteSpace(Inputs[0]) && !string.IsNullOrWhiteSpace(Inputs[1]))
            {
                int NewX = Convert.ToInt32(Inputs[0] == "-" ? "0" : Inputs[0]);
                int NewY = Convert.ToInt32(Inputs[1] == "-" ? "0" : Inputs[1]);

                TempMoveAction.MovePoint = new Point(NewX, NewY);
            }
        }

        private void NumericLoopCount_ValueChanged(object sender, EventArgs e)
        {
            TempMoveAction.LoopCount = (int)NumericLoopCount.Value;
        }

        private void NumericWaitTime_ValueChanged(object sender, EventArgs e)
        {
            TempMoveAction.WaitTime = (int)NumericWaitTime.Value;
        }

        private void ListMoveType_SelectedIndexChanged(object sender, EventArgs e)
        {
            TempMoveAction.MoveType = (Action.MoveCalcType)ListMoveType.SelectedIndex;
        }

        private void ListMoveSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            TempMoveAction.MouseSpeed = (Action.MoveSpeed)ListMoveSpeed.SelectedIndex;
        }

        private void ListImageLoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            TempMoveAction.ImageLoc = (Action.Location)ListImageLoc.SelectedIndex;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtOnKey.Text))
            {
                CMBox.Show("Warning", "No key set", Style.Warning, Buttons.OK);
                return;
            }

            if ((string.IsNullOrWhiteSpace(TxtNewAction.Text)))
            {
                CMBox.Show("Warning", "No action set", Style.Warning, Buttons.OK);
                return;
            }

            try
            {
                Action NewAction;

                switch (TempMoveAction.ActionType)
                {
                    case Action.Type.Mouse:
                        {
                            NewAction = new Action(TempMoveAction.OnKey, ActionList.GetOrder(TempMoveAction.OnKey), TempMoveAction.LoopCount, TempMoveAction.WaitTime, TempMoveAction.MovePoint, TempMoveAction.MoveType, TempMoveAction.MouseSpeed, MouseButtons.Left, TempMoveAction.AutoClick);
                            break;
                        }

                    case Action.Type.KeyPress:
                        {
                            NewAction = new Action(TempMoveAction.OnKey, ActionList.GetOrder(TempMoveAction.OnKey), TempMoveAction.LoopCount, TempMoveAction.WaitTime, TempMoveAction.ActionKey);
                            break;
                        }

                    case Action.Type.ImageSearch:
                        {
                            NewAction = new Action(TempMoveAction.OnKey, ActionList.GetOrder(TempMoveAction.OnKey), TempMoveAction.LoopCount, TempMoveAction.WaitTime, TempMoveAction.ImagePath, TempMoveAction.ImageLoc, TempMoveAction.MouseSpeed, TempMoveAction.AutoClick);
                            break;
                        }

                    default:
                        {
                            return;
                        }
                }

                ActionList.Items.Add(NewAction);
                RefreshList();

                TempMoveAction.OnKey = Keys.Escape;
                TxtOnKey.Text = string.Empty;

                SaveList();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                CMBox.Show("Error", "Couldn't add, Error: " + ex.Message, Style.Error, Buttons.OK, ex.ToString());
            }
        }

        private void BtnRecord_Click(object sender, EventArgs e)
        {
            if (Record == false)
            {
                BtnRecord.Text = "Cancel";

                RecordingGlobals.Order = ActionList.GetOrder(TempMoveAction.OnKey);

                Recording.Start(TempMoveAction.OnKey, TempMoveAction.MouseSpeed);
                ImageBaseRecorder.Start(TempMoveAction.OnKey, TempMoveAction.MouseSpeed, TempMoveAction.ImageLoc);

                Listen = false;
                Record = true;
            }
            else if (Record == true)
            {
                BtnRecord.Text = "Record";

                Record = false;
                Listen = true;

                Recording.Stop();
                ImageBaseRecorder.Stop();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            Listen = !Listen;
            BtnStartStop.Text = Listen ? "Stop" : "Start";
        }
        #endregion
    }
}