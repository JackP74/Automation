namespace Automation
{
    public static class Globals
    {
        private static int LastOrder;

        public static int Order
        {
            set
            {
                LastOrder = value;
            }
            get
            {
                LastOrder++;
                return LastOrder - 1;
            }
        }

        public static bool ForceStop = false;
    }
}
