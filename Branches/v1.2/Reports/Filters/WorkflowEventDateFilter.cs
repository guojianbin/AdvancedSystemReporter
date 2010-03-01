using System;
using ASR.Interface;
using ASR.Reports.DisplayItems;

namespace ASR.Reports.Filters
{
	class WorkflowEventDateFilter : BaseFilter
	{
		internal ItemWorkflowEvent Event
		{
			get;
			set;
		}

		#region Parameters
		public DateTime FromDate
		{
			get
			{
				string value = base.getParameter("FromDate");
				return Sitecore.DateUtil.ParseDateTime(value, DateTime.MinValue);
			}
		}
		public DateTime ToDate
		{
			get
			{
				string value = base.getParameter("ToDate");
				return Sitecore.DateUtil.ParseDateTime(value, DateTime.MaxValue);
			}
		}
		#endregion

		public override bool Filter(object element)
		{
			if (element is ItemWorkflowEvent)
			{
				Event = element as ItemWorkflowEvent;
				return (FromDate <= Event.Date && Event.Date < ToDate);
			}
			return false;
		}
	}
}
