using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASR.Interface;
using ASR.Reports.Items.Exceptions;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Workflows;

namespace ASR.Reports.Items
{
	public class ItemViewer : ASR.Interface.BaseViewer
	{
		public static string COLUMNS_PARAMETER = "columns";
		public static string HEADERS_PARAMETER = "headers";
        public static string MAX_LENGHT_PARAMETER = "maxlength";

        private int _maxLength = -1;
        public int MaxLength
        {
            get
            {
                if (_maxLength < 0)
                {
                    if (!int.TryParse(base.getParameter(MAX_LENGHT_PARAMETER), out _maxLength))
                    {
                        _maxLength = 100;
                    }
                }
                return _maxLength;
            }
        }
		public override void Display(DisplayElement d_element)
		{
			Item itemElement = d_element.Element as Item;
			if (itemElement == null)
			{
				return;
			}
			d_element.Value = itemElement.Uri.ToString();

			d_element.Header = itemElement.Name;

			for (int i = 0; i < Columns.Count; i++)
			{
				ASR.Interface.BaseColumn column = Columns[i];

				string text = column.GetColumnText(itemElement, MaxLength);

				if (string.IsNullOrEmpty(text))
				{
					d_element.AddColumn(column.Header, itemElement[column.Name]);
				}
				else
				{
					d_element.AddColumn(column.Header, text);
				}
			}
			d_element.Icon = itemElement.Appearance.Icon;
		}
	}
}
