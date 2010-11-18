using System;
using ASR.Interface;
using ASR.Reports.DisplayItems;
using Sitecore.Data.Items;
using Version = Sitecore.Data.Version;
using System.Linq;

namespace ASR.Reports.Filters
{
	class CreatedBetween : BaseFilter
	{
		public const string FROM_DATE_PARAMETER = "FromDate";
		public const string TO_DATE_PARAMETER = "ToDate";
	    public const string FIRST_VERSION = "UseFirstVersion";

		/// <summary>
		/// Gets from date.
		/// </summary>
		/// <value>From date.</value>
		public DateTime FromDate
		{
			get
			{
                if (_fromDate == DateTime.MinValue)
                {
                    string value = base.getParameter(FROM_DATE_PARAMETER);
                    _fromDate = Sitecore.DateUtil.ParseDateTime(value, DateTime.MinValue);
                }
			    return _fromDate;
			}
		}

	    private DateTime _fromDate = DateTime.MaxValue;

		/// <summary>
		/// Gets to date.
		/// </summary>
		/// <value>To date.</value>
		public DateTime ToDate
		{
			get
			{
                if(_toDate == DateTime.MinValue)
                {
                    string value = base.getParameter(TO_DATE_PARAMETER);
                    _toDate = Sitecore.DateUtil.ParseDateTime(value, DateTime.MaxValue);    
                }
			    return _toDate;
			}
		}

	    private DateTime _toDate = DateTime.MinValue;

        /// <summary>
        /// Whether to use the first version
        /// </summary>
        /// <value>Use first version.</value>
        public bool UseFirstVersion
        {
            get
            {        
                string value = base.getParameter(FIRST_VERSION);
                return value == "true";
            }
        }
        

		public override bool Filter(object element)
		{
			Item item = null;
			if (element is Item)
			{
				item = element as Item;
			}
			else if (element is ItemWorkflowEvent)
			{
				item = (element as ItemWorkflowEvent).Item;
			}
			if (item != null)
			{
                if (UseFirstVersion)
                {
                    var versions = item.Versions.GetVersionNumbers();
                    var minVersion = versions.Min(v => v.Number);
                    item = item.Database.GetItem(item.ID, item.Language, new Version(minVersion)); 
                }
				DateTime dateCreated = item.Statistics.Created;
				if (FromDate <= dateCreated && dateCreated < ToDate)
				{
					return true;
				}
			}
			return false;
		}
	}
}
