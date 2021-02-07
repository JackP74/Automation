using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Automation
{
    class Recorder
    {
        private readonly List<Move> Items;
        private DateTime LastAction;
        private Keys CurrentKey;
        private Action.MoveSpeed CurrentMoveSpeed;

        public Recorder()
        {
            Items = new List<Move>();
        }

        public void Start(Keys OnKey, Action.MoveSpeed MoveSpeed)
        {
            this.LastAction = DateTime.Now;

            this.Items.Clear();

            this.CurrentKey = OnKey;
            this.CurrentMoveSpeed = MoveSpeed;
        }

        public void Stop()
        {
            this.Items.Clear();
        }

        private int GetWaitTime()
        {
            int NewWaitTime = Convert.ToInt32((DateTime.Now - LastAction).TotalMilliseconds);
            LastAction = DateTime.Now;
            return NewWaitTime;
        }

        public void Add(Keys Key)
        {
            Items.Add(new Move(GetWaitTime(), Key, Globals.Order));
        }

        public void Add(MouseData.Type ActionType, Point Pt, MouseButtons Btn)
        {
            Items.Add(new Move(GetWaitTime(), ActionType, Pt, Btn, Globals.Order));
        }

        public List<Action> GetItems()
        {
            List<Action> FinalList = new List<Action>();

            foreach (Move Item in Items)
            {
                switch (Item.Kind)
                {
                    case Move.Type.Mouse:
                        {
                            FinalList.Add(new Action(CurrentKey, Item.Order, 1, Item.WaitTime, Item.MouseInfo.Pt, Action.MoveCalcType.Absolute, CurrentMoveSpeed, Item.MouseInfo.Btn, Item.MouseInfo.Click));
                            break;
                        }

                    case Move.Type.Keyboard:
                        {
                            FinalList.Add(new Action(CurrentKey, Item.Order, 1, Item.WaitTime, Item.KeyInfo.Key));
                            break;
                        }

                    default:
                        {
                            continue;
                        }
                }
            }

            return FinalList;
        }
    }

    public class Move
    {
        public Type Kind;
        public KeyData KeyInfo;
        public MouseData MouseInfo;
        public int WaitTime;
        public int Order;

        public enum Type
        {
            Keyboard = 0,
            Mouse = 1
        }

        public Move(int WaitTime, Keys Key, int Order)
        {
            this.Kind = Type.Keyboard;
            this.KeyInfo = new KeyData(Key);
            this.WaitTime = WaitTime;
            this.Order = Order;
        }

        public Move(int WaitTime, MouseData.Type ActionType, Point Pt, MouseButtons Btn, int Order)
        {
            this.Kind = Type.Mouse;
            this.MouseInfo = new MouseData(ActionType, Pt, Btn);
            this.WaitTime = WaitTime;
            this.Order = Order;
        }
    }

    public class KeyData
    {
        public Keys Key;

        public KeyData(Keys Key)
        {
            this.Key = Key;
        }
    }

    public class MouseData
    {
        public Type Kind;
        public Point Pt;
        public MouseButtons Btn;
        public bool Click;

        public enum Type
        {
            Click,
            Move
        }

        public MouseData(Type Kind, Point Pt, MouseButtons Btn)
        {
            this.Kind = Kind;
            this.Pt = Pt;
            this.Btn = Btn;
            this.Click = (Kind == Type.Click);
        }
    }
}