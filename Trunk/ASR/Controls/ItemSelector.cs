using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Items;
using Sitecore.Text;
using Sitecore;

namespace ASR.Controls
{
	public class ItemSelector : Sitecore.Web.UI.HtmlControls.Control
	{
		protected Edit e;
		protected Button b;

		public ItemSelector()
			: base()
		{
			e = new Edit();
			e.ReadOnly = true;
			e.Width = new System.Web.UI.WebControls.Unit(60, System.Web.UI.WebControls.UnitType.Percentage);

			Literal l = new Literal();
			l.Text = "&nbsp;&nbsp;";
			b = new Button();
			b.Header = "select...";

			this.Controls.Add(e);
			this.Controls.Add(l);
			this.Controls.Add(b);
		}

		public override string Value
		{
			get
			{
				return base.GetViewStateString("Value");
			}
			set
			{
				if (value != this.Value)
				{
					e.Value = ResultDisplay(value);
					string resultToStore = ResultValue(value);
					base.SetViewStateString("Value", resultToStore);
					SheerResponse.SetAttribute(e.ID, "value", resultToStore);
					base.Attributes["value"] = resultToStore;
				}
			}
		}

		public string Click
		{
			get
			{
				return b.Click;
			}
			set
			{
				b.Click = value;
			}
		}

		public void Clicked(ClientPipelineArgs args)
		{
			if (!args.IsPostBack)
			{
				ItemSelectorDialog.Show();
				args.WaitForPostBack();
			}
			else
			{
				if (args.Result != null && args.Result != "undefined")
				{
					Value = args.Result;

					Sitecore.Context.ClientPage.ClientResponse.Refresh(this.Parent);
				}
			}
		}

		protected virtual string ResultDisplay(string p)
		{
			Item item = Sitecore.Context.ContentDatabase.GetItem(p);
			if (item != null)
			{
				return item.Paths.FullPath;
			}
			return p;
		}

		protected virtual string ResultValue(string p)
		{
			return p;
		}
	}
}
