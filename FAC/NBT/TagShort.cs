using System.IO;
using FAC.NBT.IO;

namespace FAC.NBT
{
    public class TagShort : TagBase
    {
        private short _value;

        public short Value { get { return _value; } set { _value = value; } }

        public override TagType TagType
        {
            get { return TagType.Short; }
        }

        public TagShort(string name) : base(name) { }

        public TagShort(short value) : base(string.Empty) { _value = value; }

        public TagShort(string name, short value)
            : base(name)
        {
            _value = value;
        }


        public TagShort(TagShort original)
            : this(original.Name, original.Value) { }

        internal override void WriteBinary(NBTBinaryWriter bw)
        {
            bw.Write(_value);
        }

        internal override void ReadBinary(NBTBinaryReader br)
        {
            _value = br.ReadInt16();
        }

        public override int GetHashCode()
        {
            return Name == null ? _value : Name.Length ^ _value;
        }

        public override bool Equals(object obj)
        {
            TagShort tag = obj as TagShort;
            return tag == null ? false : tag.Value == Value && tag.Name == Name;
        }

        public override TagBase Clone()
        {
            return new TagShort(this);
        }

        public static explicit operator short(TagShort tag)
        {
            return tag.Value;
        }

    }
}
