namespace Automation
{
    public static class RecordingGlobals
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
    }
}
