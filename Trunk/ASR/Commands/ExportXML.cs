using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace ASR.Commands
{
     class ExportXML : Command
    {

        public override void Execute(CommandContext context)
        {
            try
            {
                SheerResponse.Download(new Export.XMLExport().Save("asr","xml"));
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
