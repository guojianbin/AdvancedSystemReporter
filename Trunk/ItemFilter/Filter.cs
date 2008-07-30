using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemFilter
{
    public class FilterLocked:ASR.IFilter
    {
        #region IFilter Members

        public bool ExecFilter(Sitecore.Data.Items.Item item)
        {
            if (item != null)
            {
                return item.Locking.IsLocked();
            }
            return false;
        }

        #endregion
    }

    public class FilterVersions : ASR.IFilter
    {
        #region IFilter Members

        public bool ExecFilter(Sitecore.Data.Items.Item item)
        {
            if (item != null)
            {
                return item.Versions.Count > 1;
            }
            return false;
        }

        #endregion
    }
}
