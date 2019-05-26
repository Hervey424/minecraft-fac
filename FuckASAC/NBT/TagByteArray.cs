using FuckASAC.Helper;
using System.IO;
using System.Linq;
using FuckASAC.NBT.IO;
namespace FuckASAC.NBT
{
    public class TagByteArray : TagBase
    {
        private byte[] _value;

        public byte[] Value { get { return _value; } set { _value = value; } }

        public override TagType TagType
        {
            get { return TagType.ByteArray; }
        }

        public sbyte[] SignedValue
        {
            get { return GetValueAsSbyte(); }
            set { GetValueAsSbyte(value); }
        }

        public TagByteArray(string name) : base(name) { }

        public TagByteArray(byte[] value) : base(string.Empty) { _value = value; }

        public TagByteArray(string name, byte[] value)
            : base(name)
        {
            _value = value;
        }

        public TagByteArray(TagByteArray original)
            : base(original.Name)
        {
            _value = new byte[original.Value.Length];
            System.Array.Copy(original.Value, _value, _value.Length);
        }

        public byte this[int index]
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
            bw.Write(_value);
        }

        internal override void ReadBinary(NBTBinaryReader br)
        {
            int count = br.ReadInt32();
            _value = new byte[count];
            _value = br.ReadBytes(count, false);
        }

        public override int GetHashCode()
        {
            return Name == null ? _value.Length : Name.Length ^ _value.Length;
        }

        public override bool Equals(object obj)
        {
            TagByteArray tag = obj as TagByteArray;
            return tag == null ? false : tag.Value.SequenceEqual(Value) && tag.Name == Name;
        }

        public override TagBase Clone()
        {
            return new TagByteArray(this);
        }

        private sbyte[] GetValueAsSbyte()
        {
            sbyte[] newArray = new sbyte[this.Length];
            for (int i = 0; i < this.Length; i++)
            {
                newArray[i] = this[i].ConvertToSbyte();
            }
            return newArray;
        }

        private void GetValueAsSbyte(sbyte[] data)
        {
            byte[] newArray = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                newArray[i] = data[i].ConvertToByte();
            }
            this._value = newArray;
        }

        public static explicit operator byte[](TagByteArray tag)
        {
            return tag.Value;
        }

        public static explicit operator sbyte[] (TagByteArray tag)
        {
            return tag.GetValueAsSbyte();
        }
    }
}
