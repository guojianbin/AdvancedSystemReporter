﻿using System;
using ASR.Reports.DisplayItems;
using Sitecore.Data.Items;
using Sitecore.Common;

namespace ASR.Reports.Filters
{
	public class PublishedBetween : ASR.Interface.BaseFilter
	{
		public const string FROM_DATE_PARAMETER = "FromDate";
		public const string TO_DATE_PARAMETER = "ToDate";

		/// <summary>
		/// Gets from date.
		/// </summary>
		/// <value>From date.</value>
		public DateTime FromDate
		{
			get
			{
				string value = base.getParameter(FROM_DATE_PARAMETER);
				return Sitecore.DateUtil.ParseDateTime(value, DateTime.MinValue);
			}
		}

		/// <summary>
		/// Gets to date.
		/// </summary>
		/// <value>To date.</value>
		public DateTime ToDate
		{
			get
			{
				string value = base.getParameter(TO_DATE_PARAMETER);
				return Sitecore.DateUtil.ParseDateTime(value, DateTime.MaxValue);
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
				DateTime publishDate = item.Publishing.PublishDate;
				DateTime validFromDate = item.Publishing.ValidFrom;
				if ((FromDate <= publishDate && publishDate < ToDate) || (FromDate <= validFromDate && validFromDate < ToDate))
				{
					return true;
				}
			}
			return false;
		}
	}
}
