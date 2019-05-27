using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuckASAC.Utils
{
    public static class ByteArrayExtension
    {
        public static byte[] Reverse(this byte[] b)
        {
            Array.Reverse(b);
            return b;
        }
    }
}
