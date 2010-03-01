using ASR.DomainObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Resources;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XmlControls;
using CorePoint.DomainObjects.SC;

namespace ASR.Controls
{
    class ColumnEditorForm : DialogForm
    {
        protected ASRComboEdit ColumnName;

        protected Edit ColumnHeader;
        protected Scrollbox Columns;
        protected Listbox lstPredefinedColumns;
        protected Edit txtFieldKey;
        protected Edit txtAttributes;
        protected Literal litColumnHelp;

        /// <summary>
        /// Gets or sets the index of the selected column.
        /// </summary>
        /// <value>The index of the selected column.</value>
        public int SelectedIndex
        {
            get
            {
                return MainUtil.GetInt(Sitecore.Context.ClientPage.ServerProperties["SelectedIndex"], -1);
            }
            set
            {
                Sitecore.Context.ClientPage.ServerProperties["SelectedIndex"] = value;
            }
        }

        public IEnumerable<ColumnItem> PredefinedColumnItems
        {
            get
            {
                IEnumerable<ColumnItem> columns = (IEnumerable<ColumnItem>)WebUtil.GetSessionValue("ASR_PredefinedColumnItems");

                if (columns == null)
                {
                    SCDirector director = new SCDirector("master", "en");
                    columns = ViewerColumnFolder.GetChildren()
                                                .InnerChildren
                                                .Select<Item, ColumnItem>(x => director.LoadObjectFromItem<ColumnItem>(x));

                    this.PredefinedColumnItems = columns;
                }
                return columns;
            }
            set
            {
                WebUtil.SetSessionValue("ASR_PredefinedColumnItems", value);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!Sitecore.Context.ClientPage.IsEvent)
            {
                PopulatePredefinedColumns();
            }
            base.OnLoad(e);
            if (!Sitecore.Context.ClientPage.IsEvent)
            {
                Refresh();
                SelectedIndex = -1;
                ColumnHeader.Value = Sitecore.Context.Request.QueryString["itemtype"] ?? "<no name>";
            }
        }


        protected override void OnOK(object sender, EventArgs args)
        {
            this.PredefinedColumnItems = null;
            Sitecore.Context.ClientPage.ClientResponse.SetDialogValue("yes");
            base.OnOK(sender, args);
        }
        protected override void OnCancel(object sender, EventArgs args)
        {
            this.PredefinedColumnItems = null;
            base.OnCancel(sender, args);
        }

        private void PopulatePredefinedColumns()
        {
            lstPredefinedColumns.Controls.Clear();
            foreach (ColumnItem ci in this.PredefinedColumnItems)
            {
                if (ci.Name != "*")
                {
                    ListItem li = new ListItem();
                    li.Header = ci.DefaultHeader;
                    li.Value = ci.Key;
                    lstPredefinedColumns.Controls.Add(li);
                }
            }
        }

        private Item ViewerColumnFolder
        {
            get
            {
                Database db = Sitecore.Configuration.Factory.GetDatabase(Settings.Instance.ConfigurationDatabase);
                Item vcf = db.GetItem(Settings.Instance.ViewerColumnsFolder);
                return vcf;
            }
        }

        private void Refresh()
        {
            Columns.Controls.Clear();
            Controls = new ArrayList();
            RenderColumns();
            SheerResponse.SetOuterHtml("Columns", Columns);
        }


        private void RenderColumns()
        {
            //LayoutDefinition.Parse(WebUtil.GetSessionString("SC_DEVICEEDITOR"));
            string xml = WebUtil.GetSessionString("ASR_COLUMNEDITOR");
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                XmlNodeList columnNodes = doc.DocumentElement.SelectNodes("Column");
                for (int i = 0; i < columnNodes.Count; i++)
                {
                    XmlControl webControl = Resource.GetWebControl("Column") as XmlControl;
                    Assert.IsNotNull(webControl, typeof(XmlControl));
                    Columns.Controls.Add(webControl);
                    string uniqueID = Control.GetUniqueID("C");
                    webControl["Click"] = "OnColumnClick(\"" + i + "\")";
                    webControl["DblClick"] = "device:edit";
                    if (i == SelectedIndex)
                    {
                        webControl["Background"] = "#EDB078";
                    }
                    webControl["ID"] = uniqueID;
                    webControl["Header"] = columnNodes[i].InnerText;
                    webControl["Name"] = columnNodes[i].Attributes["name"].Value;
                    if (columnNodes[i].Attributes["attributes"] != null)
                        webControl["Attr"] = "  {" + columnNodes[i].Attributes["attributes"].Value + "}";

                    Controls.Add(uniqueID);
                }
            }
        }

        protected void OnColumnClick(string index)
        {
            Assert.ArgumentNotNull(index, "index");
            //deselect current
            if (SelectedIndex >= 0)
            {
                SheerResponse.SetStyle(StringUtil.GetString(Controls[SelectedIndex]), "background", string.Empty);
            }
            //select current
            SelectedIndex = MainUtil.GetInt(index, -1);
            if (SelectedIndex >= 0)
            {
                SheerResponse.SetStyle(StringUtil.GetString(Controls[SelectedIndex]), "background", "#EDB078");
            }
        }

        protected void lstPredefinedColumns_OnDblClick()
        {
            if (lstPredefinedColumns != null && lstPredefinedColumns.Selected.Length > 0)
            {
                ListItem li = lstPredefinedColumns.Selected[0];
                ColumnHeader.Value = li.Header;
                txtFieldKey.Value = li.Value;

                ColumnItem ci = this.PredefinedColumnItems.SingleOrDefault(x => x.Key == li.Value);
                if (ci != null)
                {
                    txtAttributes.Value = ci.Attributes;
                    if (!string.IsNullOrEmpty(ci.ColumnHelp))
                        litColumnHelp.Text = ci.ColumnHelp;
                }
                li.Selected = false;
                SheerResponse.Refresh(lstPredefinedColumns);
            }
        }

        public ArrayList Controls
        {
            get
            {
                return (ArrayList)Sitecore.Context.ClientPage.ServerProperties["Controls"];
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                Sitecore.Context.ClientPage.ServerProperties["Controls"] = value;
            }
        }

        protected void MoveUp()
        {
            if (SelectedIndex > 0)
            {
                string xml = WebUtil.GetSessionString("ASR_COLUMNEDITOR");
                if (!String.IsNullOrEmpty(xml))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    XmlNode columnNode = doc.DocumentElement.ChildNodes[SelectedIndex];
                    XmlNode previousSibling = columnNode.PreviousSibling;
                    doc.DocumentElement.RemoveChild(columnNode);
                    doc.DocumentElement.InsertBefore(columnNode, previousSibling);

                    SelectedIndex--;

                    WebUtil.SetSessionValue("ASR_COLUMNEDITOR", doc.OuterXml);
                    Refresh();
                }
            }
        }

        protected void MoveDown()
        {
            if (SelectedIndex < (Controls.Count - 1))
            {
                string xml = WebUtil.GetSessionString("ASR_COLUMNEDITOR");
                if (!String.IsNullOrEmpty(xml))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    XmlNode columnNode = doc.DocumentElement.ChildNodes[SelectedIndex];
                    XmlNode nextSibling = columnNode.NextSibling;
                    doc.DocumentElement.RemoveChild(columnNode);
                    doc.DocumentElement.InsertAfter(columnNode, nextSibling);

                    SelectedIndex++;

                    WebUtil.SetSessionValue("ASR_COLUMNEDITOR", doc.OuterXml);
                    Refresh();
                }
            }
        }

        protected void Remove()
        {
            if (SelectedIndex > -1)
            {
                string xml = WebUtil.GetSessionString("ASR_COLUMNEDITOR");
                if (!String.IsNullOrEmpty(xml))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);

                    XmlNode columnNode = doc.DocumentElement.ChildNodes[SelectedIndex];
                    doc.DocumentElement.RemoveChild(columnNode);

                    SelectedIndex = -1;

                    WebUtil.SetSessionValue("ASR_COLUMNEDITOR", doc.OuterXml);
                    Refresh();
                }
            }
        }

        protected void Add()
        {
            string xml = WebUtil.GetSessionString("ASR_COLUMNEDITOR");
            if (!String.IsNullOrEmpty(xml))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlElement columnNode = doc.CreateElement("Column");

                columnNode.Attributes.Append(AddAttribute(doc, "name", txtFieldKey.Value));

                if (!string.IsNullOrEmpty(txtAttributes.Value))
                    columnNode.Attributes.Append(AddAttribute(doc, "attributes", txtAttributes.Value));

                columnNode.InnerText = ColumnHeader.Value;

                doc.DocumentElement.AppendChild(columnNode);


                ColumnHeader.Value = "<no name>";
                txtFieldKey.Value = "";
                txtAttributes.Value = "";

                WebUtil.SetSessionValue("ASR_COLUMNEDITOR", doc.OuterXml);
                Refresh();
            }
        }

        private XmlAttribute AddAttribute(XmlDocument doc, string name, string value)
        {
            XmlAttribute a = doc.CreateAttribute(name);
            a.Value = value;
            return a;
        }
    }
}
