using System.Linq;
using ASR.Interface;
using Sitecore.Data.Items;

namespace ASR.Reports.Viewers
{
    public class AliasViewer : BaseViewer
    {
        private static string AliasTemplateID;

        static AliasViewer()
        {
            AliasTemplateID = "{54BCFFB7-8F46-4948-AE74-DA5B6B5AFA86}";
        }

        private const string Usefastparameter = "usefast";

        protected bool UseFastSearch
        {
            get
            {
                return getParameter(Usefastparameter) == "true";
            }
        }
        public override void Display(DisplayElement dElement)
        {
            var item = dElement.Element as Item;
            if (item != null)
            {
                //find if alias
                var alias = FindAlias(item);
                //add it to the report

                if(alias != null)
                {
                    dElement.AddColumn("Alias",alias.Name);
                }
            }          
        }

        private static Item FindAliasFast(Item item)
        {
            const string queryFormatString = "fast:/sitecore/system/aliases/*[@linked item ='%{0}%']";
            var query = string.Format(queryFormatString, item.ID);
            var items = item.Database.SelectItems(query);
            if (items != null && items.Length > 0) return items[0];
            return null;
        }

        private Item FindAlias(Item item)
        {
            return UseFastSearch ? FindAliasFast(item) : FindAliasLinks(item);
        }

        private static Item FindAliasLinks(Item item)
        {
            var linksDb = Sitecore.Configuration.Factory.GetLinkDatabase();
            var itemlinks = linksDb.GetReferrers(item);
            foreach (var itemLink in itemlinks)
            {
                if ( itemLink.SourceDatabaseName == item.Database.Name)
                {
                    var sourceitem = itemLink.GetSourceItem();
                    if ( sourceitem.TemplateID.ToString() == AliasTemplateID)
                    {
                        return sourceitem;
                    }
                }
            }
            return null;
        }
    }
}
