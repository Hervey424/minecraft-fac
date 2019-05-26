using FuckASAC.Helper;
using System.IO;
using System.Linq;
using FuckASAC.NBT.IO;

namespace FuckASAC.NBT
{
    public class TagIntArray : TagBase
    {
        private int[] _value;

        public int[] Value { get { return _value; } set { _value = value; } }

        public override TagType TagType
        {
            get { return TagType.IntArray; }
        }

        public TagIntArray(string name) : base(name) { }

        public TagIntArray(int[] value) : base(string.Empty) { _value = value; }

        public TagIntArray(string name, int[] value)
            : base(name)
        {
            _value = value;
        }

        public TagIntArray(TagIntArray original)
            :base(original.Name)
        {
            _value = new int[original.Value.Length];
            System.Array.Copy(original.Value, _value, _value.Length);
        }


        public int this[int index]
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
            _value = new int[count];
            for (int i = 0; i < count; i++)
            {
                _value[i] = br.ReadInt32();
            }
        }

        public override int GetHashCode()
        {
            return Name == null ? _value.Length : Name.Length ^ _value.Length;
        }

        public override bool Equals(object obj)
        {
            TagIntArray tag = obj as TagIntArray;
            return tag == null ? false : tag.Value.SequenceEqual(Value) && tag.Name == Name;
        }

        public override TagBase Clone()
        {
            return new TagIntArray(this);
        }

        public static explicit operator int[](TagIntArray tag)
        {
            return tag.Value;
        }

    }
}
