using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Shell.Framework.Commands;
using ASR.Interface;
using System.Web.UI;
using Sitecore.Web.UI.Sheer;
using Sitecore.SecurityModel;
using Sitecore.Text;
using Sitecore.IO;
using Sitecore.Security.Accounts;
using System.IO;
using Sitecore.Diagnostics;

namespace ASR.Commands
{
	public class ExportExcel : Command
	{

		public override void Execute(CommandContext context)
		{
			try
			{
				string tempPath = new Export.HtmlExport(Current.Context.Report, Current.Context.ReportItem).SaveFile("asr", "xls");
				SheerResponse.Download(tempPath);
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
