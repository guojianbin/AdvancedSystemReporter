using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASR.Scanners
{
    public class Versions : IScanner
    {
        int itemCount = 0;
        int versionCount = 0;
        public IEnumerable<Sitecore.Data.Items.Item> Scan(Sitecore.Data.Items.Item root)
        {
            Sitecore.Data.Items.Item[] items =
               root.Axes.GetDescendants();
            foreach (Sitecore.Data.Items.Item item in items)
            {
                itemCount++;
                Sitecore.Data.Items.Item[] itemVersions = item.Versions.GetVersions(true);

                foreach (Sitecore.Data.Items.Item myitem in itemVersions)
                {
                    versionCount++;
                    yield return myitem;
                }

            }
        }
        public string StatusMessage()
        {
            return string.Format("{0} items scanned with {1} versions.", itemCount, versionCount);
        }

        #region IDisposable Members

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion

    }
}
