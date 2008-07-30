using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASR
{
    public interface IScanner:IDisposable
    {
        IEnumerable<Sitecore.Data.Items.Item> Scan(Sitecore.Data.Items.Item root);
        string StatusMessage();
    }
}
