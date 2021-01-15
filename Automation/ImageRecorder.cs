using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Automation
{
    class ImageRecorder
    {
        private ImageSearchForm ImageForm;

        public ImageRecorder()
        {
        }

        public void Start()
        {
            CreateForm();
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
        }

        public void SetFormLoc(Point NewPoint)
        {
            if (ImageForm == null)
                return;

            ImageForm.Location = new Point(NewPoint.X - (ImageForm.Width / 2), NewPoint.Y - (ImageForm.Height / 2));
        }

        public void NewSize(int ToWidth, int ToHeight, Point NewPoint)
        {
            if (ToWidth == 0 && ToHeight == 0)
                return;

            if (ToWidth != 0)
                ImageForm.Width += ToWidth;
            if (ToHeight != 0)
                ImageForm.Height += ToHeight;

            SetFormLoc(NewPoint);

            ImageForm.Invalidate();
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
}
