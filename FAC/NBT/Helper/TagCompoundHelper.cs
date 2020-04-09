using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FAC.NBT;

namespace FAC.NBT
{
    public static class TagCompoundHelper
    {
        public static void Add(this TagCompound tag, string name, bool value)
        {
            tag.Add(name, value ? (byte)1 : (byte)0);
        }

        public static void Add(this TagCompound tag, string name, byte value)
        {
            tag.Add(name, new TagByte(name, value));
        }

        public static void Add(this TagCompound tag, string name, sbyte value)
        {
            tag.Add(name, new TagByte(name, value));
        }

        public static void Add(this TagCompound tag, string name, short value)
        {
            tag.Add(name, new TagShort(name, value));
        }

        public static void Add(this TagCompound tag, string name, int value)
        {
            tag.Add(name, new TagInt(name, value));
        }

        public static void Add(this TagCompound tag, string name, long value)
        {
            tag.Add(name, new TagLong(name, value));
        }

        public static void Add(this TagCompound tag, string name, double value)
        {
            tag.Add(name, new TagDouble(name, value));
        }

        public static void Add(this TagCompound tag, string name, float value)
        {
            tag.Add(name, new TagFloat(name, value));
        }

        public static void Add(this TagCompound tag, string name, string value)
        {
            tag.Add(name, new TagString(name, value));
        }

        public static void Add(this TagCompound tag, string name, int[] value)
        {
            tag.Add(name, new TagIntArray(name, value));
        }

        public static void Add(this TagCompound tag, string name, byte[] value)
        {
            tag.Add(name, new TagByteArray(name, value));
        }

        public static bool GetBool(this TagCompound tag, string name)
        {
            return GetByte(tag, name) != 0;
        }

        public static byte GetByte(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? ((TagByte)tag[name]).Value : (byte)0;
        }

        public static short GetShort(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? ((TagShort)tag[name]).Value : (short)0;
        }

        public static int GetInt(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? ((TagInt)tag[name]).Value : 0;
        }

        public static long GetLong(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? ((TagLong)tag[name]).Value : 0;
        }

        public static double GetDouble(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? ((TagDouble)tag[name]).Value : 0d;
        }

        public static float GetFloat(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? ((TagFloat)tag[name]).Value : 0f;
        }

        public static string GetString(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? ((TagString)tag[name]).Value : string.Empty;
        }

        public static byte[] GetByteArray(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? ((TagByteArray)tag[name]).Value : new byte[0];
        }

        public static int[] GetIntArray(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? ((TagIntArray)tag[name]).Value : new int[0];
        }

		public static long[] GetLongArray(this TagCompound tag, string name)
		{
			return tag.ContainsKey(name) ? ((TagLongArray)tag[name]).Value : new long[0];
		}

		public static TagByte GetTagByte(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? tag[name] as TagByte : null;
        }

        public static TagShort GetTagShort(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? tag[name] as TagShort : null;
        }

        public static TagInt GetTagInt(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? tag[name] as TagInt : null;
        }

        public static TagLong GetTagLong(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? tag[name] as TagLong : null;
        }

        public static TagDouble GetTagDouble(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? tag[name] as TagDouble : null;
        }

        public static TagFloat GetTagFloat(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? tag[name] as TagFloat : null;
        }

        public static TagString GetTagString(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? tag[name] as TagString : null;
        }

        public static TagByteArray GetTagByteArray(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? tag[name] as TagByteArray : null;
        }

        public static TagIntArray GetTagIntArray(this TagCompound tag, string name)
        {
            return tag.ContainsKey(name) ? tag[name] as TagIntArray : null;
        }

		public static TagLongArray GetTagLongArray(this TagCompound tag, string name)
		{
			return tag.ContainsKey(name) ? tag[name] as TagLongArray : null;
		}

		public static bool ContainsKey(this TagCompound tag, string name, TagType type)
        {
            if (!tag.ContainsKey(name)) return false;
            return tag[name].TagType == type;
        }
    }
}
