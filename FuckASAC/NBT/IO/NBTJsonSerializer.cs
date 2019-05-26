using System;
using System.Collections.Generic;
using System.Text;

namespace FuckASAC.NBT.IO
{
    internal class NBTJsonSerializer
    {
        private readonly string _jsonText;
        private int _jsonCursor;

        public static TagCompound FromJson(string jsonstring)
        {
            return (new NBTJsonSerializer(jsonstring)).GetNBT();
        }

        private TagCompound GetNBT()
        {
            TagCompound TagCompound = StartAnalyze();
            SkipWhiteSpace();

            if (InTextLength())
            {
                ++_jsonCursor;
                throw GenerateException("Trailing data found");
            }
            else
            {
                return TagCompound;
            }
        }

        private NBTJsonSerializer(string jsonStr)
        {
            _jsonText = jsonStr;
        }

        private string ExtractKey()
        {
            SkipWhiteSpace();

            if (!InTextLength())
            {
                throw GenerateException("Expected key");
            }
            else
            {
                return JsonAt() == '"' ? CatchString() : CatchUnquotedString();
            }
        }

        private NBTException GenerateException(string str)
        {
            return new NBTException(str, _jsonText, _jsonCursor);
        }

        private TagBase CatchValues()
        {
            SkipWhiteSpace();

            if (JsonAt() == '"')
            {
                return new TagString("", CatchString());
            }
            else
            {
                string s = CatchUnquotedString();

                if (string.IsNullOrEmpty(s))
                {
                    throw GenerateException("Expected value");
                }
                else
                {
                    return GetNumberTag(s);
                }
            }
        }

        private TagBase GetNumberTag(string target)
        {
            try
            {
                if (NBTJsonSerializerFizzy.SingleRegex.IsMatch(target))
                {
                    return new TagFloat(float.Parse(target.Substring(0, target.Length - 1)));
                }

                if (NBTJsonSerializerFizzy.ByteRegex.IsMatch(target))
                {
                    return new TagByte(byte.Parse(target.Substring(0, target.Length - 1)));
                }

                if (NBTJsonSerializerFizzy.LongRegex.IsMatch(target))
                {
                    return new TagLong(long.Parse(target.Substring(0, target.Length - 1)));
                }

                if (NBTJsonSerializerFizzy.ShortRegex.IsMatch(target))
                {
                    return new TagShort(short.Parse(target.Substring(0, target.Length - 1)));
                }

                if (NBTJsonSerializerFizzy.IntRegex.IsMatch(target))
                {
                    return new TagInt(int.Parse(target));
                }

                if (NBTJsonSerializerFizzy.DoubleRegex.IsMatch(target))
                {
                    return new TagDouble(double.Parse(target.Substring(0, target.Length - 1)));
                }

                if (NBTJsonSerializerFizzy.MinorityRegex.IsMatch(target))
                {
                    return new TagDouble(double.Parse(target));
                }

                if ("true".Equals(target.ToLower()))
                {
                    return new TagByte((byte)1);
                }

                if ("false".Equals(target.ToLower()))
                {
                    return new TagByte((byte)0);
                }
            }
            catch (FormatException)
            {
            
            }

            return new TagString(string.Empty, target);
        }

        private string CatchString()
        {
            int index = ++_jsonCursor;
            StringBuilder sb = null;
            bool flag = false;

            while (InTextLength())
            {
                char c0 = NextJsonAt();

                if (flag)
                {
                    if (c0 != '\\' && c0 != '"')
                    {
                        throw GenerateException("Invalid escape of '" + c0 + "'");
                    }

                    flag = false;
                }
                else
                {
                    if (c0 == '\\')
                    {
                        flag = true;

                        if (sb == null)
                        {
                            //sb = new StringBuilder(_jsonText.SubstringWithJava(index, _jsonCursor - 1));
                            sb = new StringBuilder(_jsonText.Substring(index, _jsonCursor - index - 1));

                        }

                        continue;
                    }

                    if (c0 == '"')
                    {
                        //return sb == null ? _jsonText.SubstringWithJava(index, _jsonCursor - 1) : sb.ToString();
                        return sb == null ? _jsonText.Substring(index, _jsonCursor - index - 1) : sb.ToString();

                    }
                }

                if (sb != null)
                {
                    sb.Append(c0);
                }
            }

            throw GenerateException("Missing termination quote");
        }

        private string CatchUnquotedString()
        {
            int i;

            for (i = _jsonCursor; InTextLength() && IsEnableChar(JsonAt()); ++_jsonCursor) { }

            //return _jsonText.SubstringWithJava(i, _jsonCursor);
            return _jsonText.Substring(i, _jsonCursor - i);

        }

        private TagBase ExtractValue()
        {
            SkipWhiteSpace();

            if (!InTextLength())
            {
                throw GenerateException("Expected value");
            }
            else
            {
                char c = JsonAt();

                if (c == '{')
                {
                    return StartAnalyze();
                }
                else
                {
                    return c == '[' ? CatchListedValues() : CatchValues();
                }
            }
        }

        private TagBase CatchListedValues()
        {
            return InTextLength(2) && JsonAt(1) != '"' && JsonAt(2) == ';' ? GetArrayedTag() : GetListedTag();
        }

        private TagCompound StartAnalyze()
        {
            CheckCharacter('{');
            TagCompound compound = new TagCompound();
            SkipWhiteSpace();

            while (InTextLength() && JsonAt() != '}')
            {
                string key = ExtractKey();

                if (string.IsNullOrEmpty(key))
                {
                    throw GenerateException("Expected non-empty key");
                }

                CheckCharacter(':');
                compound.Add(key, ExtractValue());

                if (!HasArrayNext())
                {
                    break;
                }

                if (!InTextLength())
                {
                    throw GenerateException("Expected key");
                }
            }

            CheckCharacter('}');
            return compound;
        }

        private TagBase GetListedTag()
        {
            CheckCharacter('[');
            SkipWhiteSpace();

            if (!InTextLength())
            {
                throw GenerateException("Expected value");
            }
            else
            {
                TagList nbttaglist = new TagList();
                TagType baseTagType = TagType.End;

                while (JsonAt() != ']')
                {
                    TagBase TagBase = ExtractValue();
                    TagType tagtype = TagBase.TagType;

                    if (baseTagType <= 0)
                    {
                        baseTagType = tagtype;
                    }
                    else if (tagtype != baseTagType)
                    {
                        throw GenerateException("Unable to insert " + tagtype.ToString() + " into ListTag of type " + baseTagType.ToString());
                    }

                    nbttaglist.Add(TagBase);

                    if (!HasArrayNext())
                    {
                        break;
                    }

                    if (!InTextLength())
                    {
                        throw GenerateException("Expected value");
                    }
                }

                CheckCharacter(']');
                return nbttaglist;
            }
        }

        private TagBase GetArrayedTag()
        {
            CheckCharacter('[');
            char c0 = NextJsonAt();
            NextJsonAt();
            SkipWhiteSpace();

            if (!InTextLength())
            {
                throw GenerateException("Expected value");
            }
            else if (c0 == 'B')
            {
                return new TagByteArray(CatchArrayItems<byte>(TagType.ByteArray, TagType.Byte, e =>
                {
                    TagByte tag = e as TagByte;
                    return e != null ? tag.Value : (byte)0;
                }));
            }
            else if (c0 == 'L')
            {
                return new TagLongArray(CatchArrayItems<long>(TagType.LongArray, TagType.Long, e =>
                {
                    TagLong tag = e as TagLong;
                    return e != null ? tag.Value : 0;
                }));
            }
            else if (c0 == 'I')
            {
                return new TagIntArray(CatchArrayItems<int>(TagType.IntArray, TagType.Int, e =>
                {
                    TagInt tag = e as TagInt;
                    return e != null ? tag.Value : 0;
                }));
            }
            else
            {
                throw GenerateException("Invalid array type '" + c0 + "' found");
            }
        }

        private T[] CatchArrayItems<T>(TagType arrayedTagType, TagType targetTagType, Func<TagBase, T> func) where T : IConvertible
        {
            List<T> list = new List<T>();

            while (true)
            {
                if (JsonAt() != ']')
                {
                    TagBase TagBase = ExtractValue();
                    TagType targetType = TagBase.TagType;

                    if (targetType != targetTagType)
                    {
                        throw GenerateException("Unable to insert " + targetType.ToString() + " into " + arrayedTagType.ToString());
                    }

                    list.Add(func(TagBase));
                    if (HasArrayNext())
                    {
                        if (!InTextLength())
                        {
                            throw GenerateException("Expected value");
                        }

                        continue;
                    }
                }

                CheckCharacter(']');
                return list.ToArray();
            }
        }

        private void SkipWhiteSpace()
        {
            while (InTextLength() && char.IsWhiteSpace(JsonAt()))
            {
                ++_jsonCursor;
            }
        }

        private bool HasArrayNext()
        {
            SkipWhiteSpace();

            if (InTextLength() && JsonAt() == ',')
            {
                ++_jsonCursor;
                SkipWhiteSpace();
                return true;
            }
            return false;
        }

        private void CheckCharacter(char mozi)
        {
            SkipWhiteSpace();
            bool flag = InTextLength();

            if (flag && JsonAt() == mozi)
            {
                ++_jsonCursor;
            }
            else
            {
                throw new NBTException(string.Format("Expected '{0}' but got '{1}'", mozi, (flag ? JsonAt().ToString() : "<EOF>")), _jsonText, _jsonCursor + 1);

            }
        }

        private bool IsEnableChar(char c)
        {
            return c >= '0' && c <= '9' || c >= 'A' && c <= 'Z' || c >= 'a' && c <= 'z' || c == '_' || c == '-' || c == '.' || c == '+';
        }

        private bool InTextLength(int offset = 0)
        {
            return _jsonCursor + offset < _jsonText.Length;
        }

        private char JsonAt(int offset = 0)
        {
            return _jsonText[_jsonCursor + offset];
        }

        private char NextJsonAt()
        {
            return _jsonText[_jsonCursor++];
        }
    }
}
