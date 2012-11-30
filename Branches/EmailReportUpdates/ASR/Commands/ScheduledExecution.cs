using System;
using System.Linq;

using Sitecore.Tasks;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using ASR.DomainObjects;

namespace ASR.Commands
{
    using System.Net.Mail;

    using CommandItem = Sitecore.Tasks.CommandItem;
    using Sitecore;

    public class ScheduledExecution
    {
        private const string LogPrefix = "ASR.Email -- ";

        public void EmailReports(Item[] itemarray, CommandItem commandItem, ScheduleItem scheduleItem)
        {
            var item = commandItem.InnerItem;
            if (item["active"] != "1") return;

            Log("Starting report email task");
            MultilistField mf = item.Fields["reports"];
            if (mf == null) return;
            var force = item["sendempty"] == "1";
            var filePaths = mf.GetItems().Select(i => runReport(i, force));


            MailMessage mailMessage;

            try
            {
                mailMessage = new MailMessage
                {
                    From = new MailAddress(item["from"]),
                    Subject = item["subject"],
                };
            }
            catch (Exception ex)
            {
                LogException("FROM email address error.");
                return;
            }

            var senders = item["to"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var sender in senders)
            {
                // test that each email address is valid. Continue to the next if it isn't.
                try
                {
                    var toAddress = new MailAddress(sender);
                    mailMessage.To.Add(toAddress);
                }
                catch (Exception)
                {
                    LogException("TO email address error. " + sender);
                }
            }

            string[] ccAddressList = item["cc"].Split(new[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);

            if (ccAddressList.Any())
            {
                foreach (var ccAddress in ccAddressList)
                {
                    try
                    {
                        MailAddress ccMailAddress = new MailAddress(ccAddress);
                        mailMessage.CC.Add(ccMailAddress);
                    }
                    catch (Exception)
                    {
                        LogException("CC email address error. " + ccAddress);
                    }
                }
            }

            string[] bccAddressList = item["bcc"].Split(new[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);

            if (bccAddressList.Any())
            {
                foreach (var bccAddress in bccAddressList)
                {
                    try
                    {
                        MailAddress bccMailAddress = new MailAddress(bccAddress);
                        mailMessage.Bcc.Add(bccMailAddress);
                    }
                    catch (Exception)
                    {
                        LogException("BCC email address error. " + bccAddress);
                    }
                }
            }

            mailMessage.Body = Sitecore.Web.UI.WebControls.FieldRenderer.Render(item, "text");
            mailMessage.IsBodyHtml = true;

            foreach (var path in filePaths.Where(st => !string.IsNullOrEmpty(st)))
            {
                mailMessage.Attachments.Add(new Attachment(path));
            }

            Log("attempting to send message");
            MainUtil.SendMail(mailMessage);
            Log("task report email finished");
        }

        private void Log(string message)
        {
            Sitecore.Diagnostics.Log.Info(string.Concat(LogPrefix, message), this);
        }

        private void LogException(string message)
        {
            Sitecore.Diagnostics.Log.Error(string.Concat(LogPrefix, message), this);
        }
        
        private string runReport(Item item, bool force)
        {
            Assert.IsNotNull(item, "item");            
            var reportItem = ReportItem.CreateFromParameters(item["parameters"]);
            var prefix = reportItem.Name;
            var report = reportItem.TransformToReport(null);
            report.Run(null);
            Log(string.Concat("Run",reportItem.Name));
            return report.ResultsCount() != 0 || force
                    ? new Export.HtmlExport(report, reportItem).SaveFile(prefix, "html")
                    : null;
        }   
    }
}
