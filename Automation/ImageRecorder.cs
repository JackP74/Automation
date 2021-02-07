using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;

namespace Automation
{
    class ImageRecorder
    {
        private readonly List<ImageData> Items;
        private DateTime LastAction;
        private Keys CurrentKey;
        private Action.MoveSpeed CurrentMoveSpeed;
        private Action.Location CurrentImgLoc;

        private readonly Random Rnd = new Random(Guid.NewGuid().GetHashCode());

        private ImageSearchForm ImageForm;
        private bool ShowingForm = false;

        public bool ShowForm
        {
            set
            {
                ShowingForm = value;

                if (ImageForm != null && value)
                    ImageForm.Show();
                else if (ImageForm != null)
                    ImageForm.Hide();
            }
            get
            {
                return ShowingForm;
            }
        }

        public ImageRecorder()
        {
            Items = new List<ImageData>();
        }
        
        public void Start(Keys OnKey, Action.MoveSpeed MoveSpeed, Action.Location ImgLoc)
        {
            this.LastAction = DateTime.Now;

            this.Items.Clear();

            this.CurrentKey = OnKey;
            this.CurrentMoveSpeed = MoveSpeed;
            this.CurrentImgLoc = ImgLoc;

            CreateForm();
        }

        public void Stop()
        {
            this.Items.Clear();
            ImageForm.Close();
            ImageForm.Dispose();
        }

        public void CreateForm()
        {
            ImageForm = new ImageSearchForm()
            {
                Text = "Image Searh Form",
                ShowInTaskbar = false,
                FormBorderStyle = FormBorderStyle.None,
                BorderColor = Color.Red,
                BorderWidth = 2
            };

            ImageForm.Show();
            ShowForm = false;
        }

        public void SetFormLoc(Point NewPoint)
        {
            if (ImageForm == null)
                return;

            ImageForm.Location = new Point(NewPoint.X - (ImageForm.Width / 2), NewPoint.Y - (ImageForm.Height / 2));
        }

        public void NewSize(int ToWidth, int ToHeight, Point NewPoint)
        {
            if (ImageForm == null)
                return;

            if (ToWidth == 0 && ToHeight == 0)
                return;

            if (ToWidth != 0)
                ImageForm.Width += ToWidth;
            if (ToHeight != 0)
                ImageForm.Height += ToHeight;

            SetFormLoc(NewPoint);

            ImageForm.Invalidate();
        }

        public int GetWaitTime()
        {
            int NewWaitTime = Convert.ToInt32((DateTime.Now - LastAction).TotalMilliseconds);
            LastAction = DateTime.Now;
            return NewWaitTime;
        }

        public void NewRecord(MouseButtons Btn)
        {
            if (ImageForm == null)
                return;

            ShowForm = false;

            Bitmap NewImg = ScreenCapture.CaptureScreen(ImageForm.Location.X, ImageForm.Location.Y, ImageForm.Width, ImageForm.Height);
            Items.Add(new ImageData(GetWaitTime(), NewImg, Btn, Globals.Order));

            ShowForm = true;
        }

        public string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Rnd.Next(s.Length)]).ToArray());
        }

        public List<Action> GetItems()
        {
            List<Action> FinalList = new List<Action>();

            string ImgDir = Path.Combine(Application.StartupPath, "Images");

            if (!Directory.Exists(ImgDir))
                Directory.CreateDirectory(ImgDir);

            foreach (ImageData Item in Items)
            {
                string newImgPath = Path.Combine(ImgDir, $"Img-key{CurrentKey}-order{Item.Order}-{RandomString(20)}.png");
                Item.Img.Save(newImgPath, ImageFormat.Png);

                FinalList.Add(new Action(CurrentKey, Item.Order, 1, Item.WaitTime, newImgPath, CurrentImgLoc, CurrentMoveSpeed, true));
            }

            return FinalList;
        }
    }

    public class ImageSearchForm : Form
    {
        private GraphicsPath BorderPath;
        private Pen BorderPen;
        private Color CBorderColor = Color.Black;
        private float CBorderWidth = 1;
        private int BorderOffSet = 0;

        public Color BorderColor
        {
            set
            {
                CBorderColor = value;
                BorderPen = new Pen(value, BorderWidth);

                Invalidate();
            }

            get
            {
                return CBorderColor;
            }
        }

        public float BorderWidth
        {
            set
            {
                CBorderWidth = value;
                BorderPen = new Pen(BorderColor, value);
                BorderOffSet = Convert.ToInt32(BorderWidth / 2);

                Invalidate();
            }

            get
            {
                return CBorderWidth;
            }
        }

        public ImageSearchForm()
        {
            SetStyle((ControlStyles)139270, true);
            DoubleBuffered = true;

            this.TransparencyKey = Color.Fuchsia;
            this.TopMost = true;
            this.MinimumSize = new Size(10, 10);
            this.MaximumSize = new Size(400, 400);

            BorderPath = new GraphicsPath();
            BorderPath.AddRectangle(new Rectangle(BorderOffSet, BorderOffSet, this.Width - BorderOffSet - 1, this.Height - BorderOffSet - 2));

            BorderPen = new Pen(BorderColor, BorderWidth);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Graphics G = e.Graphics;

            G.Clear(Color.Fuchsia);
            G.DrawPath(BorderPen, BorderPath);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            BorderPath.Dispose();
            BorderPath = new GraphicsPath();
            BorderPath.AddRectangle(new Rectangle(BorderOffSet, BorderOffSet, this.Width - BorderOffSet - 1, this.Height - BorderOffSet - 2));
        }
    }

    public class ImageData
    {
        public Bitmap Img;
        public MouseButtons Btn;
        public int WaitTime;
        public int Order;
        
        public ImageData(int WaitTime, Bitmap Img, MouseButtons Btn, int Order)
        {
            this.WaitTime = WaitTime;

            this.Img = Img;
            this.Btn = Btn;

            this.Order = Order;
        }
    }
}
