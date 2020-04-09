using System.IO;
using FAC.NBT.IO;
using FAC.NBT.Helper;

namespace FAC.NBT
{
    public class TagByte : TagBase
    {
        private byte _value;

        public byte Value { get { return _value; } set { _value = value; } }

        public sbyte SignedValue
        {
            get { return _value.ConvertToSbyte(); }
            set { _value = value.ConvertToByte(); }
        }

        public override TagType TagType
        {
            get { return TagType.Byte; }
        }

        public TagByte(string name) : base(name) { }

        public TagByte(byte value) : base(string.Empty) { _value = value; }

        public TagByte(string name, byte value)
            : base(name)
        {
            _value = value;
        }

        public TagByte(string name, sbyte value)
            : base(name)
        {
            _value = value.ConvertToByte();
        }

        public TagByte(TagByte original)
            : this(original.Name, original.Value) { }

        internal override void WriteBinary(NBTBinaryWriter bw)
        {
            bw.Write(_value);
        }

        internal override void ReadBinary(NBTBinaryReader br)
        {
            _value = br.ReadByte();
        }

        public override int GetHashCode()
        {
            return Name == null ? _value : Name.Length ^ _value;
        }

        public override bool Equals(object obj)
        {
            TagByte tag = obj as TagByte;
            return tag == null ? false : tag.Value == Value && tag.Name == Name;
        }

        public override TagBase Clone()
        {
            return new TagByte(this);
        }

        public static explicit operator byte(TagByte tag)
        {
            return tag.Value;
        }

        public static explicit operator sbyte(TagByte tag)
        {
            return tag.SignedValue;
        }

    }
}
