using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using ASR.Interface;
using ASR.Reports.DisplayItems;


namespace ASR.Reports.Filters
{
    class FieldIsStandardValue : BaseFilter
    {
        public const string FIELD_NAME_PARAMETER = "FieldName";

        public string FieldName
        {
            get
            {
                return base.getParameter(FIELD_NAME_PARAMETER);
            }
        }

        public override bool Filter(object element)
        {
            Item item = null;
            switch (element.GetType().FullName)
            {
                case "ASR.Reports.DisplayItems.ItemWorkflowEvent":
                    item = element is ItemWorkflowEvent ? (element as ItemWorkflowEvent).Item : null;
                    break;
                case "ASR.Reports.DisplayItems.MediaUsageItem":
                    item = element is MediaUsageItem ? (element as MediaUsageItem).Item : null;
                    break;
                default:
                    item = element as Item;
                    break;
            }

            if (item != null)
            {
                Field field = item.Fields[FieldName];
                if (field != null && field.ContainsStandardValue)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
