using CorePoint.DomainObjects;
using CorePoint.DomainObjects.SC;
using System.Collections.Generic;
using System;

namespace ASR.DomainObjects
{
	[Template("System/ASR/Parameter")]
	public class ParameterItem : StandardTemplate
	{
		[Field("title")]
		public string Title
		{
			get;
			set;
		}

		[Field("type")]
		public string Type
		{
			get;
			set;
		}

		[Field("default value")]
		private string DefaultValue { get; set; }

		public string Value
		{
			get
			{
				string _replacedValue = DefaultValue;
				if (!string.IsNullOrEmpty(_replacedValue))
				{
					_replacedValue = _replacedValue.Replace("$sc_lastyear", DateTime.Today.AddYears(-1).ToString("yyyyMMddTHHmmss"));
					_replacedValue = _replacedValue.Replace("$sc_lastweek", DateTime.Today.AddDays(-7).ToString("yyyyMMddTHHmmss"));
					_replacedValue = _replacedValue.Replace("$sc_lastmonth", DateTime.Today.AddMonths(-1).ToString("yyyyMMddTHHmmss"));
					_replacedValue = _replacedValue.Replace("$sc_yesterday", DateTime.Today.AddDays(-1).ToString("yyyyMMddTHHmmss"));
					_replacedValue = _replacedValue.Replace("$sc_today", DateTime.Today.ToString("yyyyMMddTHHmmss"));
					_replacedValue = _replacedValue.Replace("$sc_now", DateTime.Now.ToString("yyyyMMddTHHmmss"));
					_replacedValue = _replacedValue.Replace("$sc_currentuser", Sitecore.Context.User == null ? string.Empty : Sitecore.Context.User.Name);
				}
				return _replacedValue;
			}
			set
			{
				DefaultValue = value;
			}
		}

		public IEnumerable<ValueItem> PossibleValues()
		{
			return this.Director.GetChildObjects<ValueItem>(this.Id);
		}

		//public string Value
		//{
		//    get;
		//    set;
		//}

	}
}
