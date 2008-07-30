using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASR
{
    public class MyInsertRule:Sitecore.Data.Masters.InsertRule
    {
        private const string TEMPLATE_ID = "{A2F37FA0-B644-4FA1-8DBE-6DD346F48ABB}";

        public MyInsertRule(int i)
        {
           // Sitecore.Diagnostics.Log.Info("Constructor param" + i,this);
        }
        public override void Expand(List<Sitecore.Data.Items.Item> masters, Sitecore.Data.Items.Item item)
        {
            Sitecore.Data.ID id = new Sitecore.Data.ID(TEMPLATE_ID);

            Sitecore.Collections.ChildList items = item.Children;
            foreach (Sitecore.Data.Items.Item child in items)
            {
                if (child.TemplateID.Equals(id))
                {
                    for (int i = 0; i < masters.Count; i++)
                    {
                        if (masters[i].ID.Equals(id))
                        {
                            masters.RemoveAt(i);
                            break;
                        }
                    }
                    break;
                }
            }
        }
    }
}
