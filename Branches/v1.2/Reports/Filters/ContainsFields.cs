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
    class ContainsFields : BaseFilter
    {
        public const string FIELDNAMES_PARAMETER = "FieldNames";

        private Dictionary<string, bool> _TemplateContainsFields = new Dictionary<string, bool>();

        public List<string> FieldNames
        {
            get
            {
                List<string> list = new List<string>();
                string s = base.getParameter(FIELDNAMES_PARAMETER);
                if (!string.IsNullOrEmpty(s))
                    list = s.Split(new char[] { ',' }).Select(x => x.ToLower()).ToList();
                return list;
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
                if (_TemplateContainsFields.Keys.Contains(item.TemplateID.ToString()))
                    return _TemplateContainsFields[item.TemplateID.ToString()];
                else
                {
                    bool bResult = true;
                    IEnumerable<string> itemfieldkeys = item.Template.Fields.Select<TemplateFieldItem,string>(f=>f.Key);

                    foreach (string f in this.FieldNames)
                    {
                        if (!itemfieldkeys.Contains(f))
                        {
                            bResult = false;
                            break;
                        }
                    }

                    _TemplateContainsFields.Add(item.TemplateID.ToString(), bResult);
                    return bResult;
                }
            }
            return false;
        }
    }
}
