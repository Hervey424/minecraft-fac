using System.IO;
using FAC.NBT.IO;

namespace FAC.NBT
{
    public class TagString : TagBase
    {
        private string _value;

        public string Value { get { return _value; } set { _value = value; } }

        public override TagType TagType
        {
            get { return TagType.String; }
        }

        public TagString(string name) : base(name) { }

        public TagString(string name, string value)
            : base(name)
        {
            _value = value;
        }

        public TagString(TagString original)
            : this(original.Name, original.Value) { }

        internal override void WriteBinary(NBTBinaryWriter bw)
        {
            bw.Write(_value);
        }

        internal override void ReadBinary(NBTBinaryReader br)
        {
            _value = br.ReadString();
        }

        public override int GetHashCode()
        {
            return Name == null ? _value.Length : (Name.Length ^ _value.Length);
        }

        public override bool Equals(object obj)
        {
            TagString tag = obj as TagString;
            return tag == null ? false : tag.Value == Value && tag.Name == Name;
        }

        public override TagBase Clone()
        {
            return new TagString(this);
        }

        public static explicit operator string(TagString tag)
        {
            return tag.Value;
        }

    }
}
