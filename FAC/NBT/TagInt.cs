using System.IO;
using FAC.NBT.IO;

namespace FAC.NBT
{
    public class TagInt : TagBase
    {
        private int _value;

        public int Value { get { return _value; } set { _value = value; } }

        public override TagType TagType
        {
            get { return TagType.Int; }
        }

        public TagInt(string name) : base(name) { }

        public TagInt(int value) : base(string.Empty) { _value = value; }

        public TagInt(string name, int value)
            : base(name)
        {
            _value = value;
        }

        public TagInt(TagInt original)
            : this(original.Name, original.Value) { }

        internal override void WriteBinary(NBTBinaryWriter bw)
        {
            bw.Write(_value);
        }

        internal override void ReadBinary(NBTBinaryReader br)
        {
            _value = br.ReadInt32();
        }

        public override int GetHashCode()
        {
            return Name == null ? _value : Name.Length ^ _value;
        }

        public override bool Equals(object obj)
        {
            TagInt tag = obj as TagInt;
            return tag == null ? false : tag.Value == Value && tag.Name == Name;
        }

        public override TagBase Clone()
        {
            return new TagInt(this);
        }

        public static explicit operator int(TagInt tag)
        {
            return tag.Value;
        }

    }
}
