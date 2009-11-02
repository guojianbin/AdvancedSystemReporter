using System;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Data;
using Sitecore.Data.Items;
using ASR.DomainObjects;
using Sitecore.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using Sitecore.Diagnostics;
using System.Linq;
using CorePoint.DomainObjects.SC;
using ASR.Controls;

namespace ASR.App
{
	public class MainForm : Sitecore.Web.UI.Sheer.BaseForm
	{

		#region Cariables
		protected ASRListview ItemList;
		protected ListviewHeader LViewHeader;

		protected Literal Status;

		protected Toolbar MyToolbar;

		protected Sitecore.Web.UI.WebControls.GridPanel MainPanel;
		protected Section ConfigSection;
		protected ContextMenu cm;

		#endregion

		protected override void OnLoad(EventArgs args)
		{
			base.OnLoad(args);
			if (!Sitecore.Context.ClientPage.IsEvent)
			{
				Current.ClearContext();
				LoadMenuItems();
				ItemList.View = "Details";
				ItemList.DblClick = "OnOpen";

				openReport(Sitecore.Web.WebUtil.GetQueryString());

			}
		}

		private void openReport(string qs)
		{
			if (string.IsNullOrEmpty(qs))
				return;
			NameValueCollection nvc = Sitecore.StringUtil.ParseNameValueCollection(qs, '&', '=');
			string id = nvc["id"];
			if (string.IsNullOrEmpty(id))
				return;

			SCDirector director = new SCDirector("master", "en");
			if (director.ObjectExists(id))
			{
				ReportItem rItem = director.GetObjectByIdentifier<ReportItem>(id);
				foreach (string key in nvc.Keys)
				{
					if (key.Contains("^"))
					{
						string[] item_parameter = key.Split('^');
						ReferenceItem ri = rItem.FindItem(item_parameter[0]);
						if (ri != null)
						{
							ri.SetAttributeValue(item_parameter[1], nvc[key]);
						}

					}
				}
				Current.Context.ReportItem = rItem;
				Current.Context.Report = null;
				updateInterface(null);
				//updateParameters(null);
				//new ReportRunner().RunCommand(null);                                    
			}
		}
		private void LoadMenuItems()
		{
			MyToolbar.Controls.Clear();
			loadMenuItemsRec(Sitecore.Context.Item.Children["menu"], MyToolbar);
		}

		private void loadMenuItemsRec(Item root, Control ctl)
		{
			ChildList menuitems = root.Children;
			foreach (Item mItem in menuitems)
			{
				Control child = null;
				switch (mItem.Template.Key)
				{
					case "toolmenu":
						//Toolmenu tm = new Toolmenu();
						ASR.Controls.Toolmenu tm = new ASR.Controls.Toolmenu();
						tm.ID = Control.GetUniqueID("T");
						tm.LoadFromItem(mItem);
						loadMenuItemsRec(mItem, tm);
						child = tm;
						break;
					case "toolbar divider":
						Tooldivider td = new Tooldivider();
						child = td;
						break;
					case "toolbutton":
						ASR.Controls.ToolButton tb = new ASR.Controls.ToolButton();
						//Toolbutton tb = new Toolbutton();     
						tb.LoadFromItem(mItem);
						child = tb;
						break;
					case "toolmenubutton":
						//Toolmenubutton tmb = new Toolmenubutton();
						ASR.Controls.ToolMenuButton tmb = new ASR.Controls.ToolMenuButton();
						tmb.LoadFromItem(mItem);
						child = tmb;
						break;
					case "menu item":
						MenuItem mi = new MenuItem();
						mi.LoadFromItem(mItem);
						child = mi;
						break;
					default:
						break;
				}
				if (child != null)
				{
					ctl.Controls.Add(child);
				}
			}

		}


		[HandleMessage("MainForm:runfinished", false)]
		private void runFinished(Sitecore.Web.UI.Sheer.Message message)
		{
			populateItemList(1, Current.Context.Settings.PageSize);
			SheerResponse.Refresh(MyToolbar);
		}

		public override void HandleMessage(Message message)
		{

			if (message.Name == "ASR.MainForm:updateSection")
			{
				Control ctl = message.Sender as Control;
				if (ctl != null)
					Sitecore.Context.ClientPage.ClientResponse.Refresh(ctl);
			}

			else if (message.Name == "event:click")
			{
				NameValueCollection nvc = message.Sender as NameValueCollection;
				if (nvc != null)
				{
					string eventname = nvc["__PARAMETERS"];
					if (!string.IsNullOrEmpty(eventname))
					{
						handleClickEvent(message, eventname);
					}
					else
					{
						base.HandleMessage(message);
					}
				}
				else
				{
					base.HandleMessage(message);
				}

			}
			else
			{
				base.HandleMessage(message);
			}
		}

		private void handleClickEvent(Message message, string eventname)
		{
			if (eventname.StartsWith("itemselector"))
			{
				string[] parts = eventname.Split(':');
				if (parts.Length == 2)
				{
					ASR.Controls.ItemSelector iselector = Sitecore.Context.ClientPage.FindSubControl(parts[1]) as ASR.Controls.ItemSelector;
					if (iselector != null)
					{
						Sitecore.Context.ClientPage.Start(iselector, "Clicked");
					}
				}
			}
			else if (eventname.StartsWith("ASRMainFormcommand:"))
			{
				string commandname = eventname.Substring(eventname.IndexOf(':') + 1);
				NameValueCollection parameters = new NameValueCollection();
				parameters.Add("name", commandname);


				Sitecore.Context.ClientPage.Start(this, "runCommand", parameters);
			}
			else if (eventname.StartsWith("changepage"))
			{
				string pageno = eventname.Substring(eventname.IndexOf(':') + 1);
				changePage(pageno);
			}
			else
			{
				base.HandleMessage(message);
			}
		}
		
		[HandleMessage("ASR.MainForm:update", false)]
		private void updateInterface(Sitecore.Web.UI.Sheer.Message message)
		{
			ItemList.ColumnNames.Clear();
			ItemList.Controls.Clear();
			Status.Text = "";
			createParameters();
			createActions();
			ConfigSection.Header = string.Concat("Configure report - ", Current.Context.ReportItem.Name);
			Sitecore.Context.ClientPage.ClientResponse.Refresh(MainPanel);
			Sitecore.Context.ClientPage.ClientResponse.Refresh(Status);
		}
		
		[HandleMessage("ASR.MainForm:toolbarupdate", false)]
		private void updateToolbar(ClientPipelineArgs args)
		{
			LoadMenuItems();
			Sitecore.Context.ClientPage.ClientResponse.Refresh(MyToolbar);
		}


		private void runCommand(ClientPipelineArgs args)
		{
			string commandname = args.Parameters["name"];
			StringList sl = new StringList();
			foreach (var item in ItemList.SelectedItems)
			{
				sl.Add(item.Value);
			}
			if (sl.Count > 0)
			{
				Current.Context.ReportItem.RunCommand(commandname, sl);
			}
			else
			{
				SheerResponse.Alert("You need to select at least one item");
			}
		}

		private void createActions()
		{
			purgeOldActions();

			Tooldivider t = new Tooldivider();
			t.ID = Control.GetUniqueID("action");
			MyToolbar.Controls.Add(t);
			foreach (var item in Current.Context.ReportItem.Commands)
			{
				Controls.ToolButton ctl = new Controls.ToolButton();
				ctl.Header = item.Name;
				ctl.Icon = item.Icon;
				ctl.Click = string.Concat("ASRMainFormcommand:", item.Name);
				ctl.ID = Control.GetUniqueID("action");

				MyToolbar.Controls.Add(ctl);
			}

			Sitecore.Context.ClientPage.ClientResponse.Refresh(MyToolbar);
		}

		private void purgeOldActions()
		{
			List<System.Web.UI.Control> cc = new List<System.Web.UI.Control>();
			foreach (System.Web.UI.Control ctl in MyToolbar.Controls)
			{
				if (ctl.ID != null && ctl.ID.StartsWith("action"))
					cc.Add(ctl);
			}
			foreach (System.Web.UI.Control ctl in cc)
			{
				MyToolbar.Controls.Remove(ctl);
			}
		}

		[HandleMessage("ASR.MainForm:updateparameters", false)]
		[HandleMessage("asr:linkit")]
		private void updateParameters(Sitecore.Web.UI.Sheer.Message message)
		{
			foreach (Control ctl in ConfigSection.Controls)
			{
				if (ctl.ID != null && ctl.ID.StartsWith("params_"))
				{
					string[] splitid = ctl.ID.Split('_');

					foreach (Control subCtl in ctl.Controls)
					{

						if (subCtl.ID != null && subCtl.ID.StartsWith("params_"))
						{
							string tid = subCtl.ID.Substring(7);
							int indexof = tid.IndexOf('_');
							tid = tid.Substring(0, indexof);
							Control input = subCtl.FindControl(subCtl.Value) as Control;
							if (input != null)
							{
								if (splitid[1].StartsWith("scanner"))
								{
									Current.Context.ReportItem.Scanners.First(s => s.Name.ToLower() == splitid[2]).SetAttributeValue(tid, input.Value);
								}
								else if (splitid[1].StartsWith("viewer"))
								{
									Current.Context.ReportItem.Viewers.First(v => v.Name.ToLower() == splitid[2]).SetAttributeValue(tid, input.Value);
								}
								else
								{
									Current.Context.ReportItem.Filters.First(f => f.Name.ToLower() == splitid[2]).SetAttributeValue(tid, input.Value);
								}
							}
						}
					}
				}
			}
			if (message.Name == "asr:linkit")
			{
				Sitecore.Context.ClientPage.SendMessage(this, "asr:createlink");
			}
		}

		private void createParameters()
		{
			ConfigSection.Controls.Clear();
			foreach (var scanner in Current.Context.ReportItem.Scanners)
			{
				makeControls(scanner);
			}

			foreach (var refItem in Current.Context.ReportItem.Filters)
			{
				makeControls(refItem);
			}
			foreach (var viewer in Current.Context.ReportItem.Viewers)
			{
				makeControls(viewer);
			}
		}

		private void makeControls(ReferenceItem referenceItem)
		{
			if (!referenceItem.HasParameters)
			{
				return;
			}
			Panel panel = new Panel();
			panel.Style.Add("border", "none");
			panel.Style.Add("margin-bottom", "10px");
			Literal literal = new Literal();
			literal.Text = string.Format("<strong>{0}</strong><br/>", referenceItem.Name);
			panel.ID = Control.GetUniqueID(string.Concat("params_", referenceItem.GetType().Name.ToLower(), "_", referenceItem.Name.ToLower(), "_"));
			panel.Controls.Add(literal);
			foreach (var pi in referenceItem.Parameters)
			{
				Inline i = new Inline();
				Label l = new Label();

				l.Header = pi.Title + ":";
				l.Style.Add("font-weight", "bold");
				l.Style.Add("margin-right", "10px");
				l.Style.Add("margin-left", "20px");
				l.Style.Add("width", "85px");
				l.Style.Add("text-align", "right");

				Control input = null;
				if (pi.Type == "Text")
				{
					input = new Edit();
					input.ID = Control.GetUniqueID("input");
				}
				else if (pi.Type == "Dropdown")
				{
					Combobox c = new Combobox();
					foreach (var value in pi.PossibleValues())
					{
						ListItem li = new ListItem();
						li.Header = value.Name;
						li.Value = value.Value;
						c.Controls.Add(li);
					}
					input = c;
					input.ID = Control.GetUniqueID("input");
				}
				else if (pi.Type == "Item Selector")
				{
					ASR.Controls.ItemSelector iSelect = new ASR.Controls.ItemSelector();
					input = iSelect;
					input.ID = Control.GetUniqueID("input");
					iSelect.Click = string.Concat("itemselector", ":", input.ID);
				}
				else if (pi.Type == "User Selector")
				{
					ASR.Controls.UserSelector uSelect = new ASR.Controls.UserSelector();
					input = uSelect;
					input.ID = Control.GetUniqueID("input");
					uSelect.Click = string.Concat("itemselector", ":", input.ID);
				}
				else if (pi.Type == "Date picker")
				{
					DateTimePicker dtPicker = new DateTimePicker();
					dtPicker.ID = Sitecore.Web.UI.HtmlControls.Control.GetUniqueID("input");
					dtPicker.ShowTime = false;
					dtPicker.Click = "datepicker" + ":" + dtPicker.ID;
					dtPicker.Style.Add(System.Web.UI.HtmlTextWriterStyle.Display, "inline");
					dtPicker.Style.Add(System.Web.UI.HtmlTextWriterStyle.VerticalAlign, "middle");
					input = dtPicker;
				}

				input.Value = pi.Value;

				i.Value = input.ID;
				i.ID = Control.GetUniqueID("params_" + pi.Name + "_");
				i.Controls.Add(l);
				i.Controls.Add(input);
				Literal lit = new Literal();
				lit.Text = "<br/>";
				i.Controls.Add(lit);
				panel.Controls.Add(i);
			}
			ConfigSection.Controls.Add(panel);
		}

		private void changePage(string pageno)
		{
			int start = (int.Parse(pageno) - 1) * Current.Context.Settings.PageSize + 1;
			populateItemList(start, Current.Context.Settings.PageSize);
		}

		private void populateItemList(int start, int count)
		{
			ItemList.Controls.Clear();
			ItemList.ColumnNames.Clear();
			ItemList.ColumnNames.Add("Icon", "Icon");

			HashSet<string> columnNames = new HashSet<string>();

			foreach (var result in Current.Context.Report.GetResultElements(start - 1, count))
			{
				ListviewItem lvi = new ListviewItem();
				lvi.ID = Control.GetUniqueID("lvi");
				lvi.Icon = result.Icon;
				lvi.Value = result.Value;
				foreach (var column in result.GetColumnNames())
				{
					columnNames.Add(column);
					lvi.ColumnValues.Add(column, result.GetColumnValue(column));
				}
				ItemList.Controls.Add(lvi);
			}
			foreach (var column in columnNames)
			{
				ItemList.ColumnNames.Add(column, column);
			}

			Status.Text = string.Format("{0} results found.", Current.Context.Report.ResultsCount());

			int noPages = (int)Math.Ceiling((decimal)Current.Context.Report.ResultsCount() / Current.Context.Settings.PageSize);
			ItemList.CurrentPage = (int)Math.Ceiling((decimal)start / Current.Context.Settings.PageSize);

			int startpage = noPages > Current.Context.Settings.MaxNumberPages
				&& ItemList.CurrentPage > Current.Context.Settings.MaxNumberPages / 2 ? ItemList.CurrentPage - Current.Context.Settings.MaxNumberPages / 2 : 1;
			int endpage = Math.Min(startpage + Current.Context.Settings.MaxNumberPages, noPages);
			if (noPages > 0)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder("&nbsp;&nbsp; Page ");
				if (startpage > 1)
				{
					int newpage = Math.Max(1, startpage - Current.Context.Settings.MaxNumberPages);
					if (newpage > 1)
					{
						ASR.Controls.LinkButton b = new ASR.Controls.LinkButton();
						b.Header = "first";
						b.Click = "changepage:" + 1;
						sb.Append(b.RenderAsText());
					}
					ASR.Controls.LinkButton lb = new ASR.Controls.LinkButton();
					lb.Header = "...";
					lb.Click = "changepage:" + newpage;
					sb.Append(lb.RenderAsText());
				}
				for (int i = startpage; i <= endpage; i++)
				{
					ASR.Controls.LinkButton b = new ASR.Controls.LinkButton();
					b.Header = i.ToString();
					b.Selected = i == ItemList.CurrentPage;
					b.Click = "changepage:" + i;
					sb.Append(b.RenderAsText());
				}
				if (endpage < noPages)
				{
					int newpage = Math.Min(noPages, endpage + Current.Context.Settings.MaxNumberPages / 2);
					ASR.Controls.LinkButton b = new ASR.Controls.LinkButton();
					b.Header = "...";
					b.Click = "changepage:" + newpage;
					sb.Append(b.RenderAsText());
					if (newpage < noPages)
					{
						b = new ASR.Controls.LinkButton();
						b.Header = "last";
						b.Click = "changepage:" + noPages;
						sb.Append(b.RenderAsText());
					}
				}
				Status.Text += sb.ToString();
			}

			Sitecore.Context.ClientPage.ClientResponse.Refresh(ItemList);

			Sitecore.Context.ClientPage.ClientResponse.Refresh(Status);
		}

		protected void OnOpen()
		{
			if (ItemList.GetSelectedItems().Length > 0)
			{
				string o = ItemList.GetSelectedItems()[0].Value;
				ItemUri uri = ItemUri.Parse(o);
				if (uri != null)
				{
					Util.OpenItem(uri);
				}
			}
		}
	}
}
