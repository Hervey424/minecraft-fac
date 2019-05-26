using FuckASAC.NBT.IO;
using System.Collections;
using System.Collections.Generic;

namespace FuckASAC.NBT
{
    public class TagList : TagBase, IEnumerable
    {
        private List<TagBase> _tagList = new List<TagBase>();

        public List<TagBase> Value { get { return _tagList; } set { _tagList = value; } }

        public override TagType TagType
        {
            get { return TagType.List; }
        }

        private TagType _listTagType;
        public TagType ListType
        {
            get { return _listTagType; }
            set { _listTagType = value; }
        }

        public TagBase this[int index]
        {
            get { return _tagList[index]; }
            set { _tagList[index] = value; }
        }

        public TagList() : base(string.Empty) { }

        public TagList(string name) : base(name) { }

        public TagList(string name, TagType tagType)
            : base(name)
        {
            _listTagType = tagType;
        }

        public TagList(TagList original)
            : base(original.Name)
        {
            _listTagType = original.ListType;
            for (int i = 0; i < original.Value.Count; i++)
            {
                TagBase _bs = original.Value[i].Clone();
                _bs._parent = this;
                Add(_bs);
            }
        }

        internal override void WriteBinary(NBTBinaryWriter bw)
        {
            if (_tagList.Count > 0)
            {
                _listTagType = _tagList[0].TagType;
            }
            else
            {
                _listTagType = TagType.Byte;
            }
            bw.Write((byte)_listTagType);
            bw.Write(_tagList.Count);
            foreach (TagBase nbt in _tagList)
            {
                nbt.WriteBinary(bw);
            }
        }

        internal override void ReadBinary(NBTBinaryReader br)
        {
            _tagList.Clear();
            _listTagType = (TagType)br.ReadByte();
            int count = br.ReadInt32();
            _tagList = new List<TagBase>();
            for (int i = 0; i < count; i++)
            {
                TagBase nbt = TagBase.CreateNewTag(_listTagType, null);
                nbt._parent = this;
                nbt.ReadBinary(br);
                _tagList.Add(nbt);
            }
        }


        public void Add(TagBase tag)
        {
            tag._parent = this;
            _tagList.Add(tag);
        }

        public void Remove(TagBase tag)
        {
            _tagList.Remove(tag);
        }

        public void RemoveAt(int index)
        {
            _tagList.RemoveAt(index);
        }

        public void Clear()
        {
            _tagList.Clear();
        }

        public void Insert(int index, TagBase tag)
        {
            if (tag == null)
                return;

            tag._parent = this;
            _tagList.Insert(index, tag);
        }

        public IEnumerator GetEnumerator()
        {
            return _tagList.GetEnumerator();
        }

        public int Count
        {
            get { return _tagList.Count; }
        }

        public override int GetHashCode()
        {
            return Name == null ? (int)_tagList.Count : Name.Length ^ (int)_tagList.Count;
        }

        public override bool Equals(object obj)
        {
            TagList tag = obj as TagList;
            if (tag == null || tag.Count != this.Count) return false;
            for (int i = 0; i < tag.Count; i++)
            {
                if (!tag[i].Equals(this[i]))
                {
                    return false;
                }
            }
            return tag.Name == Name;
        }

		public override string ToString()
		{
			return NBTFile.ToJson(this);
		}

		public override TagBase Clone()
        {
            return new TagList(this);
        }

        public static explicit operator List<TagBase>(TagList tag)
        {
            return tag.Value;
        }

    }
}
