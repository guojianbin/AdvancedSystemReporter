using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASR
{
    public interface ICommand
    {
        //returns true if the item needs to be removed from the list
        bool ExecCommand(Sitecore.Data.Items.Item item);
    }
}
