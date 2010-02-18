using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Shell.Framework;

namespace ASR.Commands
{
    public abstract class ExportBaseCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            try
            {
                if (Sitecore.Context.IsAdministrator || allowNonAdminDownload())
                {
                    string tempPath = GetFilePath();
                    SheerResponse.Download(tempPath);
                }
                else
                {
                    Sitecore.Context.ClientPage.Start(this, "Run");
                }

            }
            catch (Exception ex)
            {
                SheerResponse.Alert(ex.Message);
            }
        }

        private bool allowNonAdminDownload()
        {
            return "true" == Sitecore.Configuration.Settings.GetSetting("ASR.AllowNonAdminDownloads","false");
        }

        protected virtual void Run(ClientPipelineArgs args)
        {
            //if (Sitecore.Context.IsAdministrator || allowNonAdminDownload())
            //{
            //    string tempPath = GetFilePath();
            //    SheerResponse.Download(tempPath);
            //}
            //else
            //{
                if (!args.IsPostBack)
                {
                    string email = Sitecore.Context.User.Profile.Email;
                    SheerResponse.Input("Enter your email address", email);
                    args.WaitForPostBack();
                }
                else
                {
                    if (args.HasResult)
                    {                        
                        System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                        message.To.Add(args.Result);
                        string tempPath = GetFilePath();
                        message.Attachments.Add(new System.Net.Mail.Attachment(tempPath));
                        message.Subject = string.Format("ASR Report ({0})", Current.Context.ReportItem.Name);
                        message.From = new System.Net.Mail.MailAddress(Current.Context.Settings.EmailFrom);
                        message.Body = "Attached is your report sent at " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                        Sitecore.MainUtil.SendMail(message);
                    }
                }
            //}
        }

        protected abstract string GetFilePath();

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
