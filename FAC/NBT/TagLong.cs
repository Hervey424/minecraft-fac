using System.IO;
using FAC.NBT.IO;
namespace FAC.NBT
{
    public class TagLong : TagBase
    {
        private long _value;

        public long Value { get { return _value; } set { _value = value; } }

        public override TagType TagType
        {
            get { return TagType.Long; }
        }

        public TagLong(string name) : base(name) { }

        public TagLong(long value) : base(string.Empty) { _value = value; }

        public TagLong(string name, long value)
            : base(name)
        {
            _value = value;
        }

        public TagLong(TagLong original)
            : this(original.Name, original.Value) { }

        internal override void WriteBinary(NBTBinaryWriter bw)
        {
            bw.Write(_value);
        }

        internal override void ReadBinary(NBTBinaryReader br)
        {
            _value = br.ReadInt64();
        }

        public override int GetHashCode()
        {
            return Name == null ? (int)_value : (int)(Name.Length ^ _value);
        }

        public override bool Equals(object obj)
        {
            TagLong tag = obj as TagLong;
            return tag == null ? false : tag.Value == Value && tag.Name == Name;
        }

        public override TagBase Clone()
        {
            return new TagLong(this);
        }

        public static explicit operator long(TagLong tag)
        {
            return tag.Value;
        }

    }
}
