namespace Technolithic
{
    public static class NewBitmaskGenerator
    {
        public static int Get4BitBitmask(bool top, bool left, bool right, bool bottom)
        {
            int bitmask = 0;

            bitmask = top ? bitmask + 1 : bitmask;
            bitmask = left ? bitmask + 2 : bitmask;
            bitmask = right ? bitmask + 4 : bitmask;
            bitmask = bottom ? bitmask + 8 : bitmask;

            return bitmask;
        }
    }
}
