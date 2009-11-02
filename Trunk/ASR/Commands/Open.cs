using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using CorePoint.DomainObjects.SC;
using ASR.DomainObjects;

namespace ASR.Commands
{
    class Open : Command
    {
        public override void Execute(CommandContext context)
        {
            Sitecore.Context.ClientPage.Start(this, "Start");
        }

        public void Start(ClientPipelineArgs args)
        {

            if (!args.IsPostBack)
            {
                Util.ShowItemBrowser(
                    "Select the report",
                    "Select the report",
                    "Database/32x32/view_h.png",
                    "Select",
                    Current.Context.Settings.ReportsFolder,
                    Current.Context.ReportItem == null ? Current.Context.Settings.ReportsFolder : Current.Context.ReportItem.Path,
                    Current.Context.Settings.ConfigurationDatabase);
                args.WaitForPostBack();
            }
            else
            {
                if (!Sitecore.Data.ID.IsID(args.Result))
                {
                    return;
                }
                SCDirector director = new SCDirector(Current.Context.Settings.ConfigurationDatabase, "en");
                ReportItem rItem = director.GetObjectByIdentifier<ReportItem>(args.Result);
                if (rItem != null)
                {
                    Current.Context.ReportItem = rItem;
                    Current.Context.Report = null;
                    Sitecore.Context.ClientPage.SendMessage(this, "ASR.MainForm:update");
                   // Sitecore.Context.ClientPage.SendMessage(this, "ASR.MainForm:toolbarupdate");
                }
            }
        }

    }
}
