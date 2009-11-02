using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace ASR.Commands
{
    class ExportCSV : Command
    {
        public override void Execute(CommandContext context)
        {
            try
            {
				if (Current.Context.Report.DisplayElements.Count == 0)
				{
					SheerResponse.Alert("Report cannot be exported to CSV if there are no result records.");
				}
				else
				{
					string tempPath = new Export.CsvExport().Save("asr", "csv");
					SheerResponse.Download(tempPath);
				}
            }
            catch (Exception ex)
            {
                SheerResponse.Alert(ex.Message);               
            }
        }
        public override CommandState QueryState(CommandContext context)
        {
            if (Current.Context.Report == null)
            {
                return CommandState.Disabled;
            }

            return base.QueryState(context);
        }
    }
}
