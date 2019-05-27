using System.IO;
using FuckASAC.NBT.IO;
namespace FuckASAC.NBT
{
    public class TagFloat : TagBase
    {
        private float _value;

        public float Value { get { return _value; } set { _value = value; } }

        public override TagType TagType
        {
            get { return TagType.Float; }
        }

        public TagFloat(string name) : base(name) { }

        public TagFloat(float value) : base(string.Empty) { _value = value; }

        public TagFloat(string name, float value)
            : base(name)
        {
            _value = value;
        }

        public TagFloat(TagFloat original)
            : this(original.Name, original.Value) { }

        internal override void WriteBinary(NBTBinaryWriter bw)
        {
            bw.Write(_value);
        }

        internal override void ReadBinary(NBTBinaryReader br)
        {
            _value = br.ReadSingle();
        }

        public override int GetHashCode()
        {
            return Name == null ? (int)_value : (Name.Length ^ (int)_value);
        }

        public override bool Equals(object obj)
        {
            TagFloat tag = obj as TagFloat;
            return tag == null ? false : tag.Value == Value && tag.Name == Name;
        }

        public override TagBase Clone()
        {
            return new TagFloat(this);
        }

        public static explicit operator float(TagFloat tag)
        {
            return tag.Value;
        }

    }
}
