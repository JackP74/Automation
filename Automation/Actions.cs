using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Automation
{
    [Serializable()]
    public class Actions
    {
        public List<Action> Items;

        public Actions()
        {
            Items = new List<Action>();
        }

        public int GetOrder(Keys Key)
        {
            try
            {
                if (Items.Count() == 0)
                    return 0;

                return Items.Where(x => x.OnKey == Key).Select(x => x.Order).Max() + 1;
            }
            catch
            {
                return 0;
            }
        }
    }

    public class Action
    {
        private int localLoopCount = 1;
        private int localWaitTime = 100;

        public Keys OnKey { get; set; }

        public int Order { get; set; }

        public Type ActionType { get; set; }

        public int LoopCount
        {
            set
            {
                int NewValue = value;

                if ((NewValue > 10000))
                    NewValue = 10000;
                else if (NewValue < 1)
                    NewValue = 1;

                localLoopCount = NewValue;
            }
            get
            {
                return localLoopCount;
            }
        }

        public int WaitTime
        {
            set
            {
                int NewValue = value;

                if ((NewValue > 100000))
                    NewValue = 100000;
                else if (NewValue < 1)
                    NewValue = 1;

                localWaitTime = NewValue;
            }
            get
            {
                return localWaitTime;
            }
        }

        public Point MovePoint { get; set; }

        public MoveCalcType MoveType { get; set; }

        public MoveSpeed MouseSpeed { get; set; }

        public MouseButtons MouseBtn { get; set; }

        public bool AutoClick { get; set; }

        public Keys ActionKey { get; set; }

        public string ImagePath { get; set; }

        public Location ImageLoc { get; set; }

        public enum Type
        {
            None = 0,
            Mouse = 1,
            KeyPress = 2,
            ImageSearch = 3
        }

        public enum MoveCalcType
        {
            Relative = 0,
            Absolute = 1
        }

        public enum MoveSpeed
        {
            Instant = 0,
            LineSlow = 1,
            LineFast = 2,
            CurveSlow = 3,
            CurveFast = 4
        }

        public enum Location
        {
            LeftTop = 0,
            LeftMiddle = 1,
            LeftBottom = 2,
            MiddleTop = 3,
            Middle = 4,
            MiddleBottom = 5,
            RightTop = 6,
            RightMiddle = 7,
            RightBottom = 8
        }

        public Action()
        {
            this.Order = -1;
            this.ActionType = Type.None;

            this.MovePoint = new Point(0, 0);

            this.ActionKey = Keys.Escape;

            this.ImageLoc = Location.LeftTop;
        }

        public Action(Keys OnKey, int Order, int LoopCount, int WaitTime, Point MovePoint, MoveCalcType MoveType, MoveSpeed MouseSpeed, MouseButtons MouseBtn, bool AutoClick)
        {
            this.OnKey = OnKey;
            this.Order = Order;

            this.LoopCount = LoopCount;
            this.WaitTime = WaitTime;

            this.MovePoint = MovePoint;

            this.MoveType = MoveType;
            this.MouseSpeed = MouseSpeed;
            this.MouseBtn = MouseBtn;
            this.AutoClick = AutoClick;

            this.ActionKey = Keys.Escape;
            this.ActionType = Type.Mouse;

            this.ImageLoc = Location.LeftTop;
        }

        public Action(Keys OnKey, int Order, int LoopCount, int WaitTime, Keys ActionKey)
        {
            this.OnKey = OnKey;
            this.Order = Order;

            this.LoopCount = LoopCount;
            this.WaitTime = WaitTime;

            this.MovePoint = new Point(0, 0);

            this.ActionKey = ActionKey;
            this.ActionType = Type.KeyPress;

            this.ImageLoc = Location.LeftTop;
        }

        public Action(Keys OnKey, int Order, int LoopCount, int WaitTime, string ImagePath, Location ImageLoc, MoveSpeed MouseSpeed, bool AutoClick)
        {
            this.OnKey = OnKey;
            this.Order = Order;

            this.LoopCount = LoopCount;
            this.WaitTime = WaitTime;

            this.MovePoint = new Point(0, 0);
            this.MouseSpeed = MouseSpeed;
            this.AutoClick = AutoClick;

            this.ActionType = Type.ImageSearch;

            this.ImagePath = ImagePath;
            this.ImageLoc = ImageLoc;
        }
    }

}
