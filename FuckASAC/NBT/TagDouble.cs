using FuckASAC.Helper;
using System.IO;
using FuckASAC.NBT.IO;

namespace FuckASAC.NBT
{
    public class TagDouble : TagBase
    {
        private double _value;

        public double Value { get { return _value; } set { _value = value; } }

        public override TagType TagType
        {
            get { return TagType.Double; }
        }

        public TagDouble(string name) : base(name) { }

        public TagDouble(double value) : base(string.Empty) { _value = value; }

        public TagDouble(string name, double value)
            : base(name)
        {
            _value = value;
        }

        public TagDouble(TagDouble original)
            : this(original.Name, original.Value) { }

        internal override void WriteBinary(NBTBinaryWriter bw)
        {
            bw.Write(_value);
        }

        internal override void ReadBinary(NBTBinaryReader br)
        {
            _value = br.ReadDouble();
        }

        public override int GetHashCode()
        {
            return Name == null ? (int)_value : (Name.Length ^ (int)_value);
        }

        public override bool Equals(object obj)
        {
            TagDouble tag = obj as TagDouble;
            return tag == null ? false : tag.Value == Value && tag.Name == Name;
        }

        public override TagBase Clone()
        {
            return new TagDouble(this);
        }

        public static explicit operator double(TagDouble tag)
        {
            return tag.Value;
        }

    }
}
