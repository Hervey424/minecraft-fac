namespace FAC.NBT.IO
{
    public class JsonOptions
    {
        private static JsonOptions _default = new JsonOptions();
        public static JsonOptions Default { get { return _default; } }

		private string _keyQuotation = "\"";
		public string KeyQuotation { get { return _keyQuotation; } set { _keyQuotation = value; } }

		private string _quotation = "\"";
        public string Quotation { get { return _quotation; } set { _quotation = value; } }

        private readonly string[] _digits = new string[] { "", "b", "s", "", "l", "f", "d", "", "", "", "", "", "" };

        private bool _escape = true;
        public bool Escape { get { return _escape; } set { _escape = value; } }

        public JsonOptions() { }

        internal string GetDigit(TagType type)
        {
            if ((int)type < _digits.Length && !string.IsNullOrEmpty(_digits[(int)type]))
                return _digits[(int)type];
            return string.Empty;
        }

        internal string GetKeyText(string name)
        {
            return string.Format("{0}{1}{0}:", _quotation, EscapeString(name, _keyQuotation));
        }

        internal string EscapeString(string str, string quote)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

			if(_escape)
				str = str.Replace("\\", "\\\\");

            if (!string.IsNullOrEmpty(quote) && str.Contains(quote))
            {
                str = str.Replace(quote, "\\" + quote);
            }

            return str;
        }
    }
}
