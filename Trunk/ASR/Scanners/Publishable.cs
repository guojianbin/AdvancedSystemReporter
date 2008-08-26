using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASR.Scanners
{
    public class Publishable : IScanner
    {
        int itemCount = 0;
        int pubCount = 0;
        #region IScanner Members

        public IEnumerable<Sitecore.Data.Items.Item> Scan(Sitecore.Data.Items.Item root)
        {
            Sitecore.Data.Items.Item[] items =
               root.Axes.GetDescendants();

            foreach (Sitecore.Data.Items.Item item in items)
            {
                itemCount++;
                Sitecore.Data.Items.Item pubItem = item.Publishing.GetValidVersion(when, true);
                if (pubItem != null)
                {
                    pubCount++;
                    yield return pubItem;
                }
               
            }



        }

        public string StatusMessage()
        {
            return string.Format("{0} items scanned. {1} will be publishable", itemCount, pubCount);
        }

        #endregion
        private DateTime when = DateTime.Now;
        private string datetime;

        public string Datetime
        {
            get { return datetime; }
            set { datetime = value;
            if (!DateTime.TryParse(value, out when)) when = DateTime.Now;
            }
        }
        
        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }


}
