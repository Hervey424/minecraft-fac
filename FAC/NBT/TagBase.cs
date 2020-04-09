using System.IO;
using FAC.NBT.IO;

namespace FAC.NBT
{
    public abstract class TagBase
    {
        public const string NBTVersion = "Anvil";

        internal TagBase _parent;

        public TagBase Parent
        {
            get { return _parent; }
        }

        internal string _name = string.Empty;

        public string Name
        {
            get { return _name; }
        }

        public abstract TagType TagType
        {
            get;
        }

        public TagBase(string name)
        {
            if (!string.IsNullOrEmpty(name))
                _name = name;
        }

        internal abstract void WriteBinary(NBTBinaryWriter bw);

        internal abstract void ReadBinary(NBTBinaryReader br);

        public abstract TagBase Clone();

		public bool Rename(string newName)
        {
            if (_parent != null)
            {
                TagCompound compound = _parent as TagCompound;

                if (compound != null && !compound.ContainsKey(newName))
                {
                    TagBase tag = this;
                    tag._name = newName;
                    compound.Add(newName, tag);
                    compound.Remove(Name);
                    return true;
                }

                TagList tagList = _parent as TagList;
                if (tagList != null)
                {
                    _name = newName;
                    return true;
                }
            }
            return false;
        }

        public string GetPath()
        {
            string result = "";
            TagBase tag = this;
            while(true)
            {
                if (tag != null && tag._parent != null)
                {
                    result = "/" + tag._parent.Name + result;
                    tag = tag._parent;
                }
                else
                {
                    break;
                }
            }
            return result;
        }

        public static TagBase CreateNewTag(TagType type, string name)
        {
            if (name == null) name = string.Empty;
            if (type == TagType.End) return new TagEnd();
            if (type == TagType.Byte) return new TagByte(name);
            if (type == TagType.Short) return new TagShort(name);
            if (type == TagType.Int) return new TagInt(name);
            if (type == TagType.Long) return new TagLong(name);
            if (type == TagType.Float) return new TagFloat(name);
            if (type == TagType.Double) return new TagDouble(name);
            if (type == TagType.ByteArray) return new TagByteArray(name);
            if (type == TagType.String) return new TagString(name);
            if (type == TagType.List) return new TagList(name);
            if (type == TagType.Compound) return new TagCompound(name);
            if (type == TagType.IntArray) return new TagIntArray(name);
            if (type == TagType.LongArray) return new TagLongArray(name);

            return null;
        }

        public static TagBase ReadNamedTag(NBTBinaryReader br)
        {
            byte type = br.ReadByte();
            if (type == 0)
            {
                return new TagEnd();
            }
            else
            {
                string n = br.ReadString();
                TagBase nbt = CreateNewTag((TagType)type, n);
                nbt.ReadBinary(br);
                return nbt;
            }
        }

        public static void WriteNamedTag(TagBase tag, NBTBinaryWriter bw)
        {
            byte tagId = (byte)tag.TagType;
            bw.Write(tagId);
            if (tagId != 0)
            {
                bw.Write(tag.Name);
                tag.WriteBinary(bw);
            }
        }

    }
}
