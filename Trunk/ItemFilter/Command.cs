using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemFilter
{
    class CheckIn:ASR.ICommand
    {

        #region ICommand Members

        public bool ExecCommand(Sitecore.Data.Items.Item item)
        {
            return item.Locking.Unlock();
        }

        #endregion
    }

    class RemoveVersions : ASR.ICommand
    {

        #region ICommand Members

        public bool ExecCommand(Sitecore.Data.Items.Item item)
        {
            Sitecore.Data.Items.Item[] versions = item.Versions.GetOlderVersions();
            foreach (Sitecore.Data.Items.Item versionsItem in versions)
            {
                if (versionsItem.Version.Number < item.Version.Number)
                {
                    versionsItem.Delete();
                }
            }
            return item.Versions.GetVersions().Length == 1;
        }

        #endregion
    }
}
