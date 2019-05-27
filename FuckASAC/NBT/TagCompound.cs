using FuckASAC.NBT.IO;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace FuckASAC.NBT
{
    public class TagCompound : TagBase, ICollection<TagBase>
    {
        private Dictionary<string, TagBase> _tagMap = new Dictionary<string, TagBase>();

        public Dictionary<string, TagBase> Value { get { return _tagMap; } set { _tagMap = value; } }

        public override TagType TagType
        {
            get { return TagType.Compound; }
        }

        public TagBase this[string name]
        {
            get { return _tagMap[name]; }
            set { _tagMap[name] = value; }
        }

        public ICollection Keys { get { return _tagMap.Keys; } }

        public ICollection Values { get { return _tagMap.Values; } }

        public TagCompound() : base(string.Empty) { }

        public TagCompound(string name) : base(name) { }

        public TagCompound(TagCompound original)
            : base(original.Name)
        {
            _tagMap = new Dictionary<string, TagBase>(original._tagMap.Count);
            foreach (string key in original.Keys)
            {
                TagBase ta = original[key].Clone();
                ta._parent = this;
                _tagMap.Add(key, ta);
            }
        }

        internal override void WriteBinary(NBTBinaryWriter bw)
        {
            foreach (TagBase value in _tagMap.Values)
            {
                WriteNamedTag(value, bw);
            }
            bw.Write((byte)0);
        }

        internal override void ReadBinary(NBTBinaryReader br)
        {
            _tagMap.Clear();
            TagBase tag;
            while ((tag = ReadNamedTag(br)).TagType != TagType.End)
            {
                tag._parent = this;
                _tagMap.Add(tag.Name, tag);
            }
        }

        public void Add(string name, TagCompound value)
        {
            value._name = name;
            Add(name, (TagBase)value);
        }

        public void Add(string key, TagBase value)
        {
            value._parent = this;
            value._name = key;
            _tagMap.Add(key, value);
        }

        #region MapInterfaces

        public bool ContainsKey(string key)
        {
            return _tagMap.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _tagMap.Remove(key);
        }

        public bool TryGetValue(string key, out TagBase value)
        {
            return _tagMap.TryGetValue(key, out value);
        }

        public void Clear()
        {
            _tagMap.Clear();
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
        #endregion

        #region CollectionInterfaces

        public void Add(TagBase item)
        {
            if (item != null && item.Name != null)
            {
                item._parent = this;
                _tagMap.Add(item.Name, item);
            }
        }

        public int Count
        {
            get { return _tagMap.Count; }
        }

        public bool Contains(TagBase item)
        {
            return _tagMap.ContainsValue(item);
        }

        public void CopyTo(TagBase[] array, int arrayIndex)
        {
            array = new TagBase[Count];
            int index = 0;
            foreach (TagBase value in Values)
            {
                array[index] = value;
                index++;
            }
        }

        public bool Remove(TagBase item)
        {
            bool r = _tagMap.Remove(item.Name);
            return r;
        }

        IEnumerator<TagBase> IEnumerable<TagBase>.GetEnumerator()
        {
            return (IEnumerator<TagBase>)Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tagMap.GetEnumerator();
        }

        #endregion

        public override int GetHashCode()
        {
            return Name == null ? _tagMap.Count : Name.Length ^ _tagMap.Count;
        }

        public override bool Equals(object obj)
        {
            TagCompound tag = obj as TagCompound;
            if (tag == null || tag.Count != Count) return false;
            foreach(string key in tag.Keys)
            {
                if (!ContainsKey(key) || !this[key].Equals(tag[key]))
                {
                    return false;
                }
            }
            return tag.Name == Name;
        }

        public override TagBase Clone()
        {
            return new TagCompound(this);
        }

        public override string ToString()
        {
            return NBTFile.ToJson(this);
        }

        public static explicit operator Dictionary<string, TagBase>(TagCompound tag)
        {
            return tag.Value;
        }

    }
}
