using System;
using System.Text.RegularExpressions;
using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
	class RegexFilter : ASR.Interface.BaseFilter
	{
		public const string REGEX_PARAMETER = "Regex";

		/// <summary>
		/// Gets or sets the regular expression used to filter items by their name.
		/// </summary>
		/// <value>The regex.</value>
		public string RegexString { get; set; }

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
				return Regex.IsMatch(item.Name);
			}
			return false;
		}
	}
}
