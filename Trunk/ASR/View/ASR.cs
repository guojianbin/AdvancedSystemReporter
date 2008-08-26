using System;
using System.Collections.Generic;
using System.Text;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;
using System.Reflection;
using Sitecore.Jobs;
using ASR.App;
using Sitecore.Diagnostics;

namespace ASR.App
{
    public class MainForm : Sitecore.Web.UI.Sheer.BaseForm
    {
        
        #region variables

        protected Listview ItemList;
        protected ListviewHeader LViewHeader;
        protected Toolbutton ExecuteFilter;
        protected Toolbutton Versions;
        protected Toolbutton CommandButton;
        protected Toolbutton CommandAllButton;       
        protected Toolbutton SourceItem;
        protected Literal Status;
        protected Toolbutton ReportButton;
        protected Toolbar MyToolbar;
        protected Toolbutton OpenReport;
        
        #endregion


        #region properties

        private string propsKey
        {
            get
            {
                return string.Format("{0}:properties", Sitecore.Context.ClientPage.ClientID);
            }
        }
        private ASRproperties _properties;
        public  ASRproperties Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = ServerProperties[propsKey] as ASRproperties;
                    if (_properties != null)
                    {
                        _properties.Form = this;
                    }
                }
                return _properties;
            }
            set
            {
                _properties = value;
                ServerProperties.Add(propsKey, _properties);
            }
        }
       
        #endregion


        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);
            if (!Sitecore.Context.ClientPage.IsEvent)
            {
                Properties = new ASRproperties(this);

                ItemList.View = "Details";
                ExecuteFilter.Click = "OnExecuteQuery";
                CommandButton.Click = "OnCommandClick";
                CommandAllButton.Click = "OnCommandAllClick";
                SourceItem.Click = "SourceChange";
                ItemList.DblClick = "OnOpen";
                ReportButton.Click = "OnReportButtonClicked";
                OpenReport.Click = "OnOpenReportClicked";
            }
        }

        

        #region SourceChange

        protected void SourceChange()
        {
            // start the dialog
            Sitecore.Context.ClientPage.Start(this,"DialogProcessor");

        }

        protected void DialogProcessor(ClientPipelineArgs args)
        {
            //string root="/"; string selected = RootItem.Paths.FullPath ;
            if (!args.IsPostBack)
            {
                //Sitecore.Shell.Framework.Dialogs.BrowseItem("Select Item", "where should I start looking for items?", "Database/32x32/view_h.png", "Select", root, selected);
                Util.ShowItemBrowser(
                    "Select item",
                    "where should I start looking for items?",
                    "Database/32x32/view_h.png",
                    "Select",
                    Properties.Db.GetRootItem().ID,
                    Properties.RootItem.ID);
                args.WaitForPostBack();
            }
            // the result of a dialog is handled because a post back has occurred
            else
            {
                string res = args.Result;
                // Show an alert message box with the value from the modal dialog
                Sitecore.Data.ID myID;
                if (Sitecore.Data.ID.TryParse(res, out myID))
                {
                    Properties.RootItem = Properties.Db.GetItem(Sitecore.Data.ID.Parse(res));
                }

            }
        }

        #endregion

        #region Report changed

        protected void OnReportButtonClicked()
        {
            Sitecore.Context.ClientPage.Start(this,"ProcessReportChanged");
        }

        protected void ProcessReportChanged(ClientPipelineArgs args)
        {
            Assert.IsNotNull(Properties, "properties is null");
            //string root="/"; string selected = RootItem.Paths.FullPath ;
            if (!args.IsPostBack)
            {

                //Sitecore.Shell.Framework.Dialogs.BrowseItem("Select Item", "where should I start looking for items?", "Database/32x32/view_h.png", "Select", root, selected);
                Util.ShowItemBrowser(
                    "Select report",
                    "which report should I use?",
                    "Database/32x32/view_h.png",
                    "Select",
                    Properties.CONFIGNODE_ID,
                    Properties.ReportItem == null ? Properties.CONFIGNODE_ID : new Sitecore.Data.ID(Properties.ReportItem.Id),
                    "master");
                args.WaitForPostBack();
            }
            // the result of a dialog is handled because a post back has occurred
            else
            {
                string res = args.Result;
                Sitecore.Data.ID myID;
                if (Sitecore.Data.ID.TryParse(res, out myID))
                {
                    CorePoint.DomainObjects.SC.SCDirector director = new CorePoint.DomainObjects.SC.SCDirector("master", "en");
                    Properties.SetReportItem(new Guid(res));

                    ModifyIU();
                }

                //else
                //{
                //    ResetIU();
                //}


            }
        }

        
        #endregion

        #region running filters
        protected void OnExecuteQuery()
        {
            
            Assert.IsNotNull(Properties, "properties is null");
           
            new ReportRunner(Properties).Run();
        }


        public override void HandleMessage(Message message)
        {

            if (message.Name == "MainForm:Finished")
            {
                string jobname = Properties.JobName;
                if (!string.IsNullOrEmpty(jobname))
                {
                    Job j = JobManager.GetJob(jobname.ToString());
                    if (j != null)
                    {
                        object[] pars = j.Options.CustomData as object[];
                        if (pars != null && pars.Length == 2)
                        {
                            List<Item> itemsToAdd = pars[0] as List<Item>;
                            Status.Text = pars[1].ToString();
                            if (itemsToAdd != null)
                            {
                                ItemList.Controls.Clear();
                                foreach (Item item in itemsToAdd)
                                {
                                    AddItem(item);
                                }
                                Sitecore.Context.ClientPage.ClientResponse.Refresh(ItemList);
                            }
                        }
                    }
                }

            }
            else
                base.HandleMessage(message);
        }

      
        #endregion
        
        #region executing command
       
        
        protected void OnCommandClick()
        {
            Assert.IsNotNull(Properties, "properties is null");
            Assert.IsNotNull(Properties.Command, "command is null");

            if (ItemList.SelectedItems.Length == 1)
            {
                Sitecore.Data.Items.Item item = selectedItem();


                if (item != null)
                {
                    if (Properties.Command.ExecCommand(item))
                    {
                        ItemList.Controls.Remove(ItemList.GetItemByID(ItemList.SelectedItems[0].ID));
                        Sitecore.Context.ClientPage.ClientResponse.Refresh(ItemList);
                    }
                    else
                    {
                        Sitecore.Context.ClientPage.ClientResponse.Alert("You don't have permission to execute the command on this item");
                    }
                }
            }
        }

       


        protected void OnCommandAllClick() // needs work
        {
           

            Assert.IsNotNull(Properties,"properties is null");
            Assert.IsNotNull(Properties.Command, "command is null");
           
            
            
            int index = 0;
            int length = ItemList.Items.Length;
            for (int i = 0; i < length; i++)
            {
                ListviewItem lvi = ItemList.Items.GetValue(index) as ListviewItem;
                if (lvi != null)
                {
                    Sitecore.Data.Items.Item item = retrieveItem(lvi);
                    
                    if (item != null)
                    {
                        if (Properties.Command.ExecCommand(item))
                        {
                            ItemList.Controls.RemoveAt(index);
                        }
                        else
                        {
                            index++;
                        }
                    }
                }
            }
            Sitecore.Context.ClientPage.ClientResponse.Refresh(ItemList);
            if (index > 0)
                Sitecore.Context.ClientPage.ClientResponse.Alert("You don't have permission to execute the command on some of the items");
        }
        #endregion

        protected void OnOpenReportClicked()
        {
            Assert.IsNotNull(Properties, "properties is null");
            if (Properties.ReportItem != null)
            {
Sitecore.Data.Items.Item item = 
                Sitecore.Configuration.Factory.GetDatabase("master").GetItem( new Sitecore.Data.ID(Properties.ReportItem.Id));
            if (item!=null)
            {

                OpenContentEditor(item);
            }
            }
            

            
        }
        
        protected void OnOpen()
        {

            Item item = selectedItem();
            if (item != null)
            {
                OpenContentEditor(selectedItem());
            }
            


        }

        private static void OpenContentEditor(Sitecore.Data.Items.Item item)
        {
            Assert.IsNotNull(item,"item is null");

                Sitecore.Text.UrlString parameters = new Sitecore.Text.UrlString();
                parameters.Add("id", item.ID.ToString());
                parameters.Add("fo", item.ID.ToString());
                parameters.Add("la", item.Language.Name);
                parameters.Add("vs", item.Version.Number.ToString());
                Sitecore.Shell.Framework.Windows.RunApplication("Content editor", parameters.ToString());
            
        }


        #region Adding and getting items from listview

        protected void AddItem(Sitecore.Data.Items.Item item)
        {
            Control parent = ItemList;

            ListviewItem listItem = new ListviewItem();
            listItem.ID = Control.GetUniqueID("I");
            parent.Controls.Add(listItem);

            listItem.Icon = item.Appearance.Icon;
            listItem.Header = item.Paths.ContentPath;

            if (Properties.ReportItem.Version)
            {
                listItem.ColumnValues.Add("version", item.Version.ToString());
            }
            if (Properties.ReportItem.Language)
            {
                listItem.ColumnValues.Add("lang", item.Language.Name);
            }

            foreach (FieldItem field in Properties.ReportItem.StFields)
            {
                Field f = item.Fields[field.FieldID];
                object o = f;
                if (field.Props!=null)
                {
                    if (!string.IsNullOrEmpty(field.Fieldtype))
                    {
                         //o = Sitecore.Reflection.ReflectionUtil.CreateObject("Sitecore.kernel", field.Fieldtype, new object[]{f});
                         o = Sitecore.Reflection.ReflectionUtil.CreateObject(field.Fieldtype, new object[] { f });
                    }

                    StringBuilder sb = new StringBuilder();
                    foreach (string st in field.Props)
                    {
                        PropertyInfo pi = o.GetType().GetProperty(st);
                        if (pi != null)
                        {
                            sb.Append(pi.GetValue(o, null) + " ");
                        }

                    }
                    listItem.ColumnValues.Add(field.Name, sb.ToString());
                                       
                }
                else
                    listItem.ColumnValues.Add(field.Name, f.Value);
            }
            foreach (string st in Properties.ReportItem.CustomFields)
            {
                listItem.ColumnValues.Add(st, item[st]);
            }

        }

        private Sitecore.Data.Items.Item selectedItem()
        {
            if (ItemList.SelectedItems.Length == 1)
            {
                return retrieveItem(ItemList.SelectedItems[0]);
            }
            return null;
        }

        private Sitecore.Data.Items.Item retrieveItem(ListviewItem listItem)
        {

            string path = listItem.Header;
            Sitecore.Globalization.Language lang = Sitecore.Globalization.Language.Current;
            Sitecore.Data.Version version = Sitecore.Data.Version.Latest;
            Sitecore.Data.Items.Item item = null;
            if (listItem.ColumnValues.Contains("lang"))
            {
                Sitecore.Globalization.Language.TryParse(listItem.ColumnValues["lang"].ToString(), out lang);
            }
            if (listItem.ColumnValues.Contains("version"))
            {
                version = new Sitecore.Data.Version(listItem.ColumnValues["version"].ToString());
            }

            item =
                Properties.Db.GetItem(path, lang, version);
            if (item == null)
            {
                if (Properties.Db.HasContentItem)
                {
                    path = "/sitecore/content/" + path;
                    item = Properties.Db.GetItem(path, lang, version);
                    if (item  != null)
                    {
                        return item;
                    }
                }
                    throw new Exception("Item does not exist in such language");
            }
            return item;
        }

        #endregion

        internal void ModifyIU()
        {
            Assert.IsNotNull(Properties, "Properties is null");

            if (Properties.ReportItem != null)
            {
                //set command button names
                ExecuteFilter.Visible = true;
                CommandItem commandItem = Properties.ReportItem.Command;
                if (commandItem != null)
                {
                    CommandButton.Visible = true;
                    CommandAllButton.Visible = true;
                    CommandButton.Header = commandItem.Title;
                    CommandAllButton.Header = commandItem.Title + " all";
                    CommandButton.Icon = commandItem.Icon;
                    CommandAllButton.Icon = commandItem.IconAll;
                }
                else
                {
                    CommandButton.Visible = false;
                    CommandAllButton.Visible = false;
                }
                ExecuteFilter.Header = Properties.ReportItem.Text;
                ExecuteFilter.Icon = Properties.ReportItem.Icon;

                ItemList.Controls.Clear();
                ItemList.ColumnNames.Clear();
                ItemList.ColumnNames.Add("path", "Path");

                if (Properties.ReportItem.Version) ItemList.ColumnNames.Add("version", "Version");
                if (Properties.ReportItem.Language) ItemList.ColumnNames.Add("lang", "Language");
                foreach (FieldItem field in Properties.ReportItem.StFields)
                {
                    ItemList.ColumnNames.Add(field.Name, field.Name);
                }
                foreach (string st in Properties.ReportItem.CustomFields)
                {
                    ItemList.ColumnNames.Add(st, st);
                }
            }

            Sitecore.Web.UI.WebControls.GridPanel ctl = Sitecore.Context.ClientPage.FindControl("MainPanel") as
                Sitecore.Web.UI.WebControls.GridPanel;
            Assert.IsNotNull(ctl, "can't find gridpanel");

            Sitecore.Context.ClientPage.ClientResponse.Refresh(ctl) ;

            

        }

     
    }
}
