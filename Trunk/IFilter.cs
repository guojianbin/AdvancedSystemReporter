using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASR
{
    public interface IFilter
    {
        bool ExecFilter(Sitecore.Data.Items.Item item);
    }
}
