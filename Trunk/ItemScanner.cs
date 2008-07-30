using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASR
{
    public class ScanItems:IScanner
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
    public class ScanVersions : IScanner
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
    public class ScanLanguages : IScanner
    {
        int itemCount = 0;
        int langCount = 0;
        public IEnumerable<Sitecore.Data.Items.Item> Scan(Sitecore.Data.Items.Item root)
        {
            Sitecore.Data.Items.Item[] items =
               root.Axes.GetDescendants();
            foreach (Sitecore.Data.Items.Item item in items)
            {
                itemCount++;
                foreach (Sitecore.Globalization.Language language in item.Languages)
                {
                    Sitecore.Data.Items.Item langItem =
                        root.Database.GetItem(item.ID, language);
                    if (langItem != null)
                    {
                        langCount++;
                        yield return langItem;
                    }
                }
            }
        }
        public string StatusMessage()
        {
            return string.Format("{0} items and {1} language versions scanned.", itemCount,langCount);
        }
        #region IDisposable Members

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion

    }
}
