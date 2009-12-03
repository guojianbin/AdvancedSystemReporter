using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Tasks;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;

namespace ASR.Commands
{
    public class ScheduledExecution
    {
        public void EmailReports(Item[] itemarray, CommandItem commandItem, ScheduleItem scheduleItem)
        {
            //MultilistField mf = commandItem.InnerItem.Fields["reports"];
            //if (mf != null)
            //{
            //    foreach (Item item in mf.GetItems())
            //    {
            //        runReport(item);
            //    }
            //}
        }

        private void runReport(Item item)
        {
            throw new NotImplementedException(item.Name);
        }
    }
}
