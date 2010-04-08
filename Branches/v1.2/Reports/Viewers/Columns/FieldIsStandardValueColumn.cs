using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ASR.Reports.Viewers.Columns
{
    class FieldIsStandardValueColumn : ASR.Interface.BaseColumn
    {
        public const string FIELD_NAME_PARAMETER = "fieldname";

        public string FieldName
        {
            get
            {
                return base.getParameter(FIELD_NAME_PARAMETER);
            }
        }

        public override string GetColumnText(Item itemElement, int iMaxLength)
        {
            string fieldname = this.FieldName;

            Field f = itemElement.Fields[fieldname];
            if (f == null)
                return "n/a";
            else
            {
                if (itemElement.Fields[fieldname].ContainsStandardValue || !itemElement.Fields[fieldname].HasValue)
                    return "N";
                else
                    return "Y";
            }
        }
    }
}
