using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore.Text;

namespace ASR
{
    class Utils
    { 

        public static void ShowItemBrowser(string header, string text, string icon, string button, Sitecore.Data.ID root, Sitecore.Data.ID selected, string database)
        {
            //string str = selected;
            //Item item = Client.ContentDatabase.Items[selected];
            //if (item != null)
            //{
            //    selected = item.ID.ToString();
            //    if (item.Parent != null)
            //    {
            //        str = item.ParentID.ToString();
            //    }
            //}
            UrlString str2 = new UrlString("/sitecore/shell/Applications/Item browser.aspx");
            str2.Append("db", database);
            str2.Append("id", selected.ToString());
            str2.Append("fo", selected.ToString());
            str2.Append("ro", root.ToString());
            str2.Append("he", header);
            str2.Append("txt", text);
            str2.Append("ic", icon);
            str2.Append("btn", button);
            SheerResponse.ShowModalDialog(str2.ToString(), true);
        }
        public static void ShowItemBrowser(string header, string text, string icon, string button, Sitecore.Data.ID root, Sitecore.Data.ID selected)
        {
            ShowItemBrowser(
                header, text, icon, button, root, selected, Sitecore.Context.ContentDatabase.Name);
        }
    }
}
