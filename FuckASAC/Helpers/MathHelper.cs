namespace FuckASAC.Helper
{
    public static class MathHelper
    {
        public static sbyte ConvertToSbyte(this byte b)
        {
            sbyte r = 0;
            if (b > 127)
                r = (sbyte)(b - 256);
            else
                r = (sbyte)b;
            return r;
        }

        public static byte ConvertToByte(this sbyte b)
        {
            byte r = 0;
            if (b < 0)
                r = (byte)(b + 256);
            else
                r = (byte)b;
            return r;
        }
    }
}
