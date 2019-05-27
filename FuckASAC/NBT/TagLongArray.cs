using System.IO;
using System.Linq;
using FuckASAC.NBT.IO;

namespace FuckASAC.NBT
{
    public class TagLongArray : TagBase
    {
        private long[] _value;

        public long[] Value { get { return _value; } set { _value = value; } }

        public override TagType TagType
        {
            get { return TagType.LongArray; }
        }

        public TagLongArray(string name) : base(name) { }

        public TagLongArray(long[] value) : base(string.Empty) { _value = value; }

        public TagLongArray(string name, long[] value)
            : base(name)
        {
            _value = value;
        }

        public TagLongArray(TagLongArray original)
            :base(original.Name)
        {
            _value = new long[original.Value.Length];
            System.Array.Copy(original.Value, _value, _value.Length);
        }


        public long this[int index]
        {
            get { return _value[index]; }
            set { _value[index] = value; }
        }

        public int Length
        {
            get { return _value.Length; }
        }

        internal override void WriteBinary(NBTBinaryWriter bw)
        {
            bw.Write(_value.Length);
            for (int i = 0; i < _value.Length; i++)
            {
                bw.Write(_value[i]);
            }
        }

        internal override void ReadBinary(NBTBinaryReader br)
        {
            int count = br.ReadInt32();
            _value = new long[count];
            for (int i = 0; i < count; i++)
            {
                _value[i] = br.ReadInt64();
            }
        }

        public override int GetHashCode()
        {
            return Name == null ? _value.Length : Name.Length ^ _value.Length;
        }

        public override bool Equals(object obj)
        {
            TagLongArray tag = obj as TagLongArray;
            return tag == null ? false : tag.Value.SequenceEqual(Value) && tag.Name == Name;
        }

        public override TagBase Clone()
        {
            return new TagLongArray(this);
        }

        public static explicit operator long[](TagLongArray tag)
        {
            return tag.Value;
        }

    }
}
