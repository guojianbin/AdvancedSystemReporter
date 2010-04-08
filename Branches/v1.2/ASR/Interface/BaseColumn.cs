using ASR;
using ASR.DomainObjects;
using System;
using System.Text;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;


namespace ASR.Interface
{
    public abstract class BaseColumn : BaseReportObject
    {
        protected ColumnItem _columnitem;
        public string Name
        {
            get
            {
                return _columnitem.Name;
            }
        }
        public string Header
        {
            get
            {
                return _columnitem.Header.IfNullOrEmpty(_columnitem.DefaultHeader);
            }
        }

        public abstract string GetColumnText(Item itemElement, int iMaxLength);

        private static BaseColumn Create(string type)
        {
            return BaseReportObject.createObject(type) as BaseColumn;
        }

        internal static BaseColumn Create(ColumnItem ci)
        {
            Assert.ArgumentNotNull(ci, "ColumnItem");
            Assert.ArgumentNotNull(ci.FullType, "ColumnItem.FullType");
            BaseColumn c = BaseColumn.Create(ci.FullType) as BaseColumn;
            c._columnitem = ci;
            c.AddParameters(ci.Attributes);
            return c;
        }
    }
}
