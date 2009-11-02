using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASR.Scanners
{
    public class Items : IScanner
    {
        int itemCount = 0;
        public IEnumerable<Sitecore.Data.Items.Item> Scan(Sitecore.Data.Items.Item root)
        {
            Sitecore.Data.Items.Item[] items =
                 root.Axes.GetDescendants();
            foreach (Sitecore.Data.Items.Item item in items)
            {
                itemCount++;
                yield return item;
            }
        }
        public string StatusMessage()
        {
            return string.Format("{0} items scanned.", itemCount);
        }


        #region IDisposable Members

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion


    }
}
