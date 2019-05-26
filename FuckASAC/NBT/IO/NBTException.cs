using System;

namespace FuckASAC.NBT.IO
{
    public class NBTException : Exception
    {
        public NBTException(string error)
            : base(error) { }

        public NBTException(string error, string json, int index)
    : base(error) { }
    }
}
