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
	public class ExportExcel : ExportBaseCommand
	{
		protected override string GetFilePath()
        {
            string tempPath = new Export.HtmlExport(Current.Context.Report, Current.Context.ReportItem).SaveFile("asr", "xls");
           return tempPath;
        }
	}
}
