using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Security;
using Sitecore.Web.UI.Sheer;
using Sitecore.Diagnostics;

namespace ASR.Controls
{
    public class UserSelector : ItemSelector
    {
		public new void Clicked(ClientPipelineArgs args)
		{
			if (!args.IsPostBack)
			{
				SelectAccountOptions options = new SelectAccountOptions();
				options.Multiple = false;
				options.ExcludeRoles = false;
				options.DomainName = "sitecore";
				SheerResponse.ShowModalDialog(options.ToUrlString().ToString(), true);
				args.WaitForPostBack();
			}
			else
			{
				if (args.Result != null)
				{
					Value = args.Result;

					Sitecore.Context.ClientPage.ClientResponse.Refresh(this.Parent);
				}
			}
		}

		protected override string ResultDisplay(string p)
        {
            return resultToString(p);
        }

        protected override string ResultValue(string p)
        {
            return resultToString(p);
        }

        private string resultToString(string resultString)
        {
            Assert.ArgumentNotNull(resultString, "resultstring");

            int i = resultString.IndexOf('^');
            if (i > 0)
            {
                return resultString.Substring(0, i);
            }
            if (resultString == "undefined")
            {
                return string.Empty;
            }
            return resultString;
        }
    }
}
