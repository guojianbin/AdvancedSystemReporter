using System;
using System.Text.RegularExpressions;
using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
	class RegexFieldFilter : ASR.Interface.BaseFilter
	{
		public const string REGEX_PARAMETER = "Regex";
        public const string FIELD_PARAMETER = "Field";

        private string _field;
        private string Field
        {
            get
            {
                if (_field == null)
                {
                    _field = base.getParameter(FIELD_PARAMETER);                    
                }
                return _field;
            }
        }
		

		private Regex _regex;
		private Regex Regex
		{
			get
			{
				if (_regex == null)
				{
					string value = base.getParameter(REGEX_PARAMETER);
					if (!String.IsNullOrEmpty(value))
					{
						_regex = new Regex(value);
					}
				}
				return _regex;
			}
		}

		public override bool Filter(object element)
		{
			Item item = element as Item;
			if (item != null)
			{
                if (string.IsNullOrEmpty(Field))
                {
                    return Regex.IsMatch(item.Name);
                }
                else
                {
                    return Regex.IsMatch(item[Field]);
                }
			}
			return false;
		}
	}
}
