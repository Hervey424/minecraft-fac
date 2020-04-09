using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FAC.NBT.IO
{
    class NBTJsonSerializerFizzy
    {
        private static Regex ListRegex = new Regex("\\[[-+\\d|,\\s]+\\]");
        private static Regex DecimalRegex = new Regex("^[-+]?([0-9]+\\.?|[0-9]*\\.[0-9]+)([eE][-+]?[0-9]+)?$");

        internal static Regex DoubleRegex = new Regex("^[-+]?([0-9]+\\.?|[0-9]*\\.[0-9]+)([eE][-+]?[0-9]+)?[d|D]$");
        internal static Regex SingleRegex = new Regex("^[-+]?([0-9]+\\.?|[0-9]*\\.[0-9]+)([eE][-+]?[0-9]+)?[f|F]$");
        internal static Regex ByteRegex = new Regex("^[-+]?[0-9]+[b|B]$");
        internal static Regex LongRegex = new Regex("^[-+]?[0-9]+[l|L]$");
        internal static Regex ShortRegex = new Regex("^[-+]?[0-9]+[s|S]$");
        internal static Regex IntRegex = new Regex("^[-+]?[0-9]+$");
        internal static Regex MinorityRegex = new Regex("^[-+]?[0-9]*\\.?[0-9]+$");

        internal static TagBase FromJson(string json)
        {
            json = json.Trim();

            if (!json.StartsWith("{"))
            {
                throw new NBTException("Invalid tag encountered, expected \'{\' as first char.");
            }
            else if (CheckQuoteBalance(json) != 1)
            {
                throw new NBTException("Encountered multiple top tags, only one expected");
            }
            else
            {
                return ValueExtract("tag", json);
            }
        }

        private static int CheckQuoteBalance(string json)
        {
            int count = 0;
            bool inQuote = false;
            Stack<char> charStack = new Stack<char>();

            for (int i = 0; i < json.Length; ++i)
            {
                char _c = json[i];

                if (_c == '"' || _c == '\'')
                {
                    if (IsEscaped(json, i))
                    {
                        if (!inQuote)
                        {
                            throw new NBTException("Illegal use of \\\": " + json);
                        }
                    }
                    else
                    {
                        inQuote = !inQuote;
                    }
                }
                else if (!inQuote)
                {
                    if (_c != '{' && _c != '[')
                    {
                        if (_c == '}' && (charStack.Count == 0 || (charStack.Pop()) != '{'))
                        {
                            throw new NBTException("Unbalanced curly brackets {}: " + json);
                        }

                        if (_c == ']' && (charStack.Count == 0 || (charStack.Pop()) != '['))
                        {
                            throw new NBTException("Unbalanced square brackets []: " + json);
                        }
                    }
                    else
                    {
                        if (charStack.Count == 0)
                            count++;
                        
                        charStack.Push(_c);
                    }
                }
            }

            if (inQuote)
            {
                throw new NBTException("Unbalanced quotation: " + json);
            }
            else if (charStack.Count != 0)
            {
                throw new NBTException("Unbalanced brackets: " + json);
            }
            else
            {
                if (count == 0 && !string.IsNullOrWhiteSpace(json))
                    return 1;
            }
            return count;
        }

        private static TagBase ValueExtract(string key, string json)
        {
            json = json.Trim();

            if (json.StartsWith("{"))
            {
                json = json.Substring(1, json.Length - 2);
                TagCompound compound = new TagCompound(key);

                while(json.Length > 0)
                {
                    string sentence = TakeSentence(json, true);

                    if (!string.IsNullOrEmpty(sentence))
                    {
                        compound.Add(ValueExtract(sentence, false));
                    }

                    if (json.Length < sentence.Length + 1)
                    {
                        break;
                    }

                    char nextKeyChar = json[sentence.Length];

                    if (nextKeyChar != ',' && nextKeyChar != '{' && nextKeyChar != '}' && nextKeyChar != '[' && nextKeyChar != ']')
                    {
                        throw new NBTException("Unexpected token \'" + nextKeyChar + "\' at: " + json.Substring(sentence.Length));
                    }

                    json = json.Substring(sentence.Length + 1);
                }

                return compound;
            }
            else if (json.StartsWith("[") && !ListRegex.IsMatch(json))
            {
                json = json.Substring(1, json.Length - 2);
                TagList list = new TagList(key);

                while(json.Length > 0)
                {
                    string sentence = TakeSentence(json, false);

                    if (!string.IsNullOrEmpty(sentence))
                    {
                        list.Add(ValueExtract(sentence, true));
                    }

                    if (json.Length < sentence.Length + 1)
                    {
                        break;
                    }

                    char nextKeyChar = json[sentence.Length];

                    if (nextKeyChar != ',' && nextKeyChar != '{' && nextKeyChar != '}' && nextKeyChar != '[' && nextKeyChar != ']')
                    {
                        throw new NBTException("Unexpected token \'" + nextKeyChar + "\' at: " + json.Substring(sentence.Length));
                    }

                    json = json.Substring(sentence.Length + 1);
                }

                return list;
            }
            else
            {
                return GetValue(key, json);
            }
        }

        private static TagBase GetValue(string key, string json)
        {
            try
            {
                if (DoubleRegex.IsMatch(json))
                {
                    return new TagDouble(double.Parse(json.Substring(0, json.Length - 1)));
                }

                if (SingleRegex.IsMatch(json))
                {
                    return new TagFloat(float.Parse(json.Substring(0, json.Length - 1)));
                }

                if (ByteRegex.IsMatch(json))
                {
                    return new TagByte(string.Empty, sbyte.Parse(json.Substring(0, json.Length - 1)));
                }

                if (LongRegex.IsMatch(json))
                {
                    return new TagLong(long.Parse(json.Substring(0, json.Length - 1)));
                }

                if (ShortRegex.IsMatch(json))
                {
                    return new TagShort(short.Parse(json.Substring(0, json.Length - 1)));
                }

                if (IntRegex.IsMatch(json))
                {
                    return new TagInt(int.Parse(json));
                }

                if (DecimalRegex.IsMatch(json))
                {
                    return new TagDouble(double.Parse(json));
                }

                if (json.ToLower() == "true" || json.ToLower() == "false")
                {
                    return new TagByte((byte)(bool.Parse(json) ? 1 : 0));
                }
            }
            catch (FormatException)
            {
                json = json.Replace("\\\\\"", "\"");
                return new TagString(string.Empty, json);
            }

            if (json.StartsWith("[") && json.EndsWith("]"))
            {
                string s = json.Substring(1, json.Length - 2);
                string[] values = s.Split(',');

                try
                {
                    int[] ary = new int[values.Length];

                    for (int i = 0; i < values.Length; ++i)
                    {
                        ary[i] = int.Parse(values[i].Trim());
                    }

                    return new TagIntArray(ary);
                }
                catch (FormatException)
                {
                    return new TagString(string.Empty, json);
                }
            }
            else
            {
                if (json.StartsWith("\"") && json.EndsWith("\""))
                {
                    json = json.Substring(1, json.Length - 2);
                }

                json = json.Replace("\\\\\"", "\"");
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < json.Length; i++)
                {
                    if (i < json.Length - 1 && json[i] == '\\' && json[i + 1] == '\\')
                    {
                        sb.Append('\\');
                        i++;
                    }
                    else
                    {
                        sb.Append(json[i]);
                    }
                }

                return new TagString(string.Empty, sb.ToString());
            }
        }

        private static TagBase ValueExtract(string sentence, bool isArray)
        {
            string _key = DivideKey(sentence, isArray);
            string _json = DevideValues(sentence, isArray);
            TagBase _tag = ValueExtract(_key, _json);
            _tag._name = _key;
            return _tag;
        }

        private static string TakeSentence(string json, bool isCompound)
        {
            int cIndex = JsonIndexOf(json, ':');
            int dIndex = JsonIndexOf(json, ',');

            if (isCompound)
            {
                if (cIndex == -1)
                {
                    throw new NBTException("Unable to locate name/value separator for string: " + json);
                }

                if (dIndex != -1 && dIndex < cIndex)
                {
                    throw new NBTException("Name error at: " + json);
                }
            }
            else if (cIndex == -1 || cIndex > dIndex)
            {
                cIndex = -1;
            }

            return SplitMain(json, cIndex);
        }

        private static string SplitMain(string json, int startIndex)
        {
            Stack<char> stack = new Stack<char>();
            
            bool inQuote = false;
            bool exist = false;
            bool inStr = false;
            int lastIndex = 0;

            for (int i = startIndex + 1; i < json.Length; i++)
            {
                char c = json[i];

                if (c == '"' || c == '\'')
                {
                    if (IsEscaped(json, i))
                    {
                        if (!inQuote)
                        {
                            throw new NBTException("Illegal use of \\\": " + json);
                        }
                    }
                    else
                    {
                        inQuote = !inQuote;

                        if (inQuote && !inStr)
                        {
                            exist = true;
                        }

                        if (!inQuote)
                        {
                            lastIndex = i;
                        }
                    }
                }
                else if (!inQuote)
                {
                    if (c != '{' && c != '[')
                    {
                        if (c == '}' && (stack.Count == 0 || (stack.Pop()) != '{'))
                        {
                            throw new NBTException("Unbalanced curly brackets {}: " + json);
                        }

                        if (c == ']' && (stack.Count == 0 || (stack.Pop()) != '['))
                        {
                            throw new NBTException("Unbalanced square brackets []: " + json);
                        }

                        if (c == ',' && stack.Count == 0)
                        {
                            return json.Substring(0, i);
                        }
                    }
                    else
                    {
                        stack.Push(c);
                    }
                }

                if (!char.IsWhiteSpace(c))
                {
                    if (!inQuote && exist && lastIndex != i)
                    {
                        return json.Substring(0, lastIndex + 1);
                    }

                    inStr = true;
                }
            }

            //return json.SubstringWithJava(0, j);
            return json.Substring(0, json.Length);
        }

        private static string DivideKey(string json, bool isArray)
        {
            if (isArray)
            {
                json = json.Trim();

                if (json.StartsWith("{") || json.StartsWith("["))
                {
                    return string.Empty;
                }
            }

            int index = JsonIndexOf(json, ':');

            if (index == -1)
            {
                if (isArray)
                {
                    return string.Empty;
                }
                else
                {
                    throw new NBTException("Unable to locate name/value separator for string: " + json);
                }
            }
            return json.Substring(0, index).Trim();
        }

        private static string DevideValues(string json, bool isArray)
        {
            if (isArray)
            {
                json = json.Trim();

                if (json.StartsWith("{") || json.StartsWith("["))
                {
                    return json;
                }
            }

            int i = JsonIndexOf(json, ':');

            if (i == -1)
            {
                if (isArray)
                {
                    return json;
                }
                else
                {
                    throw new NBTException("Unable to locate name/value separator for string: " + json);
                }
            }
            else
            {
                return json.Substring(i + 1).Trim();
            }
        }

        private static int JsonIndexOf(string json, char value)
        {
            bool inQuote = true;
            for (int i = 0; i < json.Length; i++)
            {
                char c = json[i];

                if (c == '"' || c == '\'')
                {
                    if (!IsEscaped(json, i))
                    {
                        inQuote = !inQuote;
                    }
                }
                else if (inQuote)
                {
                    if (c == value)
                    {
                        return i;
                    }

                    if (c == '{' || c == '[')
                    {
                        return -1;
                    }
                }
            }

            return -1;
        }

        private static bool IsEscaped(string json, int index)
        {
            return index > 0 && json[index - 1] == '\\' && !IsEscaped(json, index - 1);
        }

        internal static string ToJson(TagBase tag, JsonOptions ops)
        {
            if(tag.TagType == TagType.Compound)
            {
                return GetFromCompound((TagCompound)tag, ops);
            }
            else if(tag.TagType == TagType.List)
            {
                return GetFromList((TagList)tag, ops);
            }
            return string.Empty;
        }

        private static string GetFromCompound(TagCompound tag, JsonOptions ops)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            int index = 0;
            foreach (string key in tag.Keys)
            {
                sb.Append(ops.GetKeyText(key));
                sb.Append(GetJsonValue(tag[key], ops));

                if (tag.Count - 1 != index)
                {
                    sb.Append(',');
                }
                index++;
            }
            sb.Append('}');
            return sb.ToString();
        }

        private static string GetFromList(TagList tag, JsonOptions ops)
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;

            sb.Append('[');
            foreach (TagBase _tag in tag.Value)
            {
                sb.Append(GetJsonValue(_tag, ops));
                if (tag.Count - 1 != index)
                {
                    sb.Append(',');
                }
                index++;
            }
            sb.Append(']');
            return sb.ToString();
        }

        private static string GetJsonValue(TagBase tag, JsonOptions ops)
        {
            if (tag.TagType == TagType.Byte)
            {
                return ((TagByte)tag).Value + ops.GetDigit(TagType.Byte);
            }
            if (tag.TagType == TagType.Short)
            {
                return ((TagShort)tag).Value + ops.GetDigit(TagType.Short);
            }
            if (tag.TagType == TagType.Int)
            {
                return ((TagInt)tag).Value + ops.GetDigit(TagType.Int);
            }
            if (tag.TagType == TagType.Long)
            {
                return ((TagLong)tag).Value + ops.GetDigit(TagType.Long);
            }
            if (tag.TagType == TagType.Float)
            {
                return ((TagFloat)tag).Value + ops.GetDigit(TagType.Float);
            }
            if (tag.TagType == TagType.Double)
            {
                return ((TagDouble)tag).Value + ops.GetDigit(TagType.Double);
            }
            if (tag.TagType == TagType.String)
            {
                return ops.Quotation + ops.EscapeString(((TagString)tag).Value, ops.Quotation) + ops.Quotation;
            }
            if (tag.TagType == TagType.ByteArray)
            {
                byte[] ary = ((TagByteArray)tag).Value;
                StringBuilder sb = new StringBuilder();
                sb.Append('[');
                for (int i = 0; i < ary.Length; i++)
                {
                    sb.Append(ary[i]);
                    sb.Append(ops.GetDigit(TagType.Byte));
                    if (i != ary.Length - 1) sb.Append(',');
                }
                sb.Append(']');
                return sb.ToString();
            }
            if (tag.TagType == TagType.Compound)
            {
                return GetFromCompound((TagCompound)tag, ops);
            }
            if (tag.TagType == TagType.List)
            {
                return GetFromList((TagList)tag, ops);
            }
            if (tag.TagType == TagType.IntArray)
            {
                int[] ary = ((TagIntArray)tag).Value;
                StringBuilder sb = new StringBuilder();
                sb.Append('[');
                for (int i = 0; i < ary.Length; i++)
                {
                    sb.Append(ary[i]);
                    if (i != ary.Length - 1) sb.Append(',');
                }
                sb.Append(']');
                return sb.ToString();
            }
            return string.Empty;
        }

    }
}
