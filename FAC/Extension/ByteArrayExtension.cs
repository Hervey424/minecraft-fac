using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FAC.Utils
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
