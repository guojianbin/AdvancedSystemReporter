using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace ASR
{
    public class ASR : Sitecore.Web.UI.Sheer.BaseForm
    {
        
        #region variables

        protected Listview ItemList;

        protected Toolbutton ExecuteFilter;
        protected Toolbutton Versions;
        protected Toolbutton CommandButton;
        protected Toolbutton CommandAllButton;
       
        protected Toolbutton SourceItem;

        protected Literal Status;
        protected Toolbutton ReportButton;
        protected Toolbar MyToolbar;

        private Sitecore.Data.ID CONFIGNODE_ID = new Sitecore.Data.ID("{30BB56D2-4F1E-4ECD-B7A6-D32A902DD8E5}");
       
        #endregion


        #region properties
        private string rootItemKey
        {
            get
            {
                return string.Format("{0}:rootitem",Sitecore.Context.ClientPage.ClientID);
            }
        }
        private string reportItemKey
        {
            get
            {
                return string.Format("{0}:reportitem", Sitecore.Context.ClientPage.ClientID);
            }
        }
        private Sitecore.Data.Database Db
        {
            get
            {
                return Sitecore.Context.ContentDatabase;
            }
        }
        protected Sitecore.Data.Items.Item RootItem
        {
            get
            {
                Sitecore.Data.ID id =
                    Sitecore.Context.ClientPage.ServerProperties[rootItemKey] as Sitecore.Data.ID;
                Sitecore.Data.Items.Item item = null;
                if (id != (Sitecore.Data.ID)null)
                {
                    item = Db.GetItem(id);
                }

                if (item == null) item = Db.GetRootItem();

                return item;
            }
            set
            {
                Sitecore.Context.ClientPage.ServerProperties[rootItemKey] = value.ID;

            }
        }
        Guid ReportItemId
        {
            get
            {
                if (_reportItem != null)
                {
                    return _reportItem.Id;
                }
                object id =
                        Sitecore.Context.ClientPage.ServerProperties[reportItemKey];
                if (id!=null)
                {
                    return (Guid)id;
                }
                return Guid.Empty;
            }
        }
        private ReportItem _reportItem;
        ReportItem ReportItem
        {
            get
            {
                if (_reportItem == null)
                {
                    Guid id = ReportItemId;
                    if (!id.Equals(Guid.Empty))
                    {
                        CorePoint.DomainObjects.SC.SCDirector director = new CorePoint.DomainObjects.SC.SCDirector("master","en");
                        _reportItem = director.GetObjectByIdentifier<ReportItem>(id);

                    }
                    else
                    {
                        return null;
                    }
                }              
                return _reportItem;
            }
            set
            {
                if (value!=null)
                {
                    _reportItem = value;

                    Sitecore.Context.ClientPage.ServerProperties[reportItemKey] = _reportItem.Id;
                 }
            }
        }
       
        #endregion


        protected override void OnLoad(EventArgs args)
        {
            base.OnLoad(args);
            if (!Sitecore.Context.ClientPage.IsEvent)
            {
                ItemList.View = "Details";
                ExecuteFilter.Click = "OnExecuteQuery";

                CommandButton.Click = "OnCommandClick";
                CommandAllButton.Click = "OnCommandAllClick";

                SourceItem.Click = "SourceChange";

                ItemList.DblClick = "OnOpen";
                ReportButton.Click = "OnReportButtonClicked";
            }
        }

        

        #region SourceChange

        protected void SourceChange()
        {
            // start the dialog
            Sitecore.Context.ClientPage.Start(this, "DialogProcessor");

        }

        protected void DialogProcessor(ClientPipelineArgs args)
        {
            //string root="/"; string selected = RootItem.Paths.FullPath ;
            if (!args.IsPostBack)
            {
                //Sitecore.Shell.Framework.Dialogs.BrowseItem("Select Item", "where should I start looking for items?", "Database/32x32/view_h.png", "Select", root, selected);
                Utils.ShowItemBrowser(
                    "Select item",
                    "where should I start looking for items?",
                    "Database/32x32/view_h.png",
                    "Select",
                    Db.GetRootItem().ID,
                    RootItem.ID);
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
                    RootItem = Db.GetItem(Sitecore.Data.ID.Parse(res));
                }

            }
        }

        #endregion

        #region Report changed

        protected void OnReportButtonClicked()
        {
            Sitecore.Context.ClientPage.Start(this, "DialogReportProcessor");
        }


        protected void DialogReportProcessor(ClientPipelineArgs args)
        {
            //string root="/"; string selected = RootItem.Paths.FullPath ;
            if (!args.IsPostBack)
            {

                //Sitecore.Shell.Framework.Dialogs.BrowseItem("Select Item", "where should I start looking for items?", "Database/32x32/view_h.png", "Select", root, selected);
                Utils.ShowItemBrowser(
                    "Select report",
                    "which report should I use?",
                    "Database/32x32/view_h.png",
                    "Select",
                    CONFIGNODE_ID,
                    ReportItemId.Equals(Guid.Empty) ? CONFIGNODE_ID : new ID(ReportItemId),
                    "master");
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
                    CorePoint.DomainObjects.SC.SCDirector director = new CorePoint.DomainObjects.SC.SCDirector("master", "en");
                    ReportItem  = director.GetObjectByIdentifier(new Guid(res)) as ReportItem;
                    if (ReportItem != null)
                    {

                        //set command button names

                        
                        ExecuteFilter.Visible = true;

                        CommandItem commandItem = ReportItem.GetCommand();
                        if (commandItem != null)
                        {
                            CommandButton.Visible = true;
                            CommandAllButton.Visible = true;
                            CommandButton.Header = commandItem.Name;
                            CommandAllButton.Header = commandItem.Name + " all";
                            CommandButton.Icon = commandItem.Icon;
                            CommandAllButton.Icon = commandItem.IconAll;
                        }
                        else
                        {
                            CommandButton.Visible = false;
                            CommandAllButton.Visible = false;
                        }
                        ExecuteFilter.Header = ReportItem.Text;
                        ExecuteFilter.Icon = ReportItem.Icon;
                    }
                }

                else
                {
                    ExecuteFilter.Visible = false;
                    CommandButton.Visible = false;
                    CommandAllButton.Visible = false;
                }

                SheerResponse.Refresh(MyToolbar);
            }
        }

        
        #endregion

        

        #region running filters
        protected void OnExecuteQuery()
        {
            ItemList.Controls.Clear();

            Sitecore.Diagnostics.Debug.Assert(ReportItem != null, "There is no report");

            List<FilterItem> filterItems = ReportItem.GetFilters();
            IScanner scanner = ReportItem.GetScanner();
            if (filterItems.Count > 0 && scanner != null)
            {
                IFilter[] filters = new IFilter[filterItems.Count];
                for (int i = 0; i < filterItems.Count; i++)
                {
                    filters[i] = filterItems[i].MakeObject() as IFilter;
                }

                PopulateList(ItemList, scanner, filters);
                Sitecore.Context.ClientPage.ClientResponse.Refresh(ItemList);
            }
        }

      
        #endregion

        #region generic populate

       

        protected void PopulateList(Control parent, IScanner scanner, params IFilter[] filters)
        {
            using (scanner)
            {
                int itemAdded = 0;
                foreach (Sitecore.Data.Items.Item item in scanner.Scan(RootItem))
                {
                    bool add = true;
                    foreach (IFilter myFilter in filters)
                    {
                        if (myFilter != null)
                        {
                            if (!myFilter.ExecFilter(item))
                            {
                                add = false;
                                break;
                            }
                        }
                    }
                    if (add)
                    {
                        AddItem(item, parent);
                        itemAdded++;
                    }
                }
                Status.Text = string.Format("{0} items found. {1}", itemAdded, scanner.StatusMessage());
            }
        }

        #endregion

        #region executing command
        protected void OnCommandClick()
        {
            ICommand command = GetCommand();

            if (command == null)
            {
                return;
            }

            if (ItemList.SelectedItems.Length == 1)
            {
                Sitecore.Data.Items.Item item = selectedItem();


                if (item != null)
                {
                    if (command.ExecCommand(item))
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
        private ICommand GetCommand()
        {
            if (ReportItem != null)
            {
                CommandItem ci = ReportItem.GetCommand();
                if (ci != null)
                {
                    return ci.MakeObject() as ICommand;
                }
            }
            return null;
        }


        protected void OnCommandAllClick() // needs work
        {
            ICommand command = GetCommand();

            if (command == null)
            {
                return;
            }

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
                        if (command.ExecCommand(item))
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

        protected void OnOpen()
        {

            Sitecore.Data.Items.Item item = selectedItem();


            if (item != null)
            {
                Sitecore.Text.UrlString parameters = new Sitecore.Text.UrlString();
                parameters.Add("id", item.ID.ToString());
                parameters.Add("fo", item.ID.ToString());
                parameters.Add("la", item.Language.Name);
                parameters.Add("vs", item.Version.Number.ToString());
                Sitecore.Shell.Framework.Windows.RunApplication("Content editor", parameters.ToString());
            }



        }

        #region Adding and getting items from listview

        protected void AddItem(Sitecore.Data.Items.Item item, Control parent)
        {

            ListviewItem listItem = new ListviewItem();
            listItem.ID = Control.GetUniqueID("I");
            parent.Controls.Add(listItem);

            listItem.Icon = item.Appearance.Icon;
            listItem.Header = item.Paths.ContentPath;

            listItem.ColumnValues.Add("lang", item.Language.Name);
            listItem.ColumnValues.Add("version", item.Version.Number);

            if (item.Locking.IsLocked())
            {
                Sitecore.Data.Fields.LockField lockField = item.Fields[Sitecore.FieldIDs.Lock];



                if (lockField != null)
                {
                    listItem.ColumnValues.Add("date", lockField.Date.ToString());
                    listItem.ColumnValues.Add("user", lockField.Owner);

                }

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
            Sitecore.Globalization.Language lang;
            Sitecore.Data.Items.Item item = null;
            if (Sitecore.Globalization.Language.TryParse(listItem.ColumnValues["lang"].ToString(), out lang))
            {
                Sitecore.Data.Version version = new Sitecore.Data.Version(listItem.ColumnValues["version"].ToString());

                item =
                Db.GetItem(path, lang, version);

                if (item == null && Db.HasContentItem)
                {
                    path = "/sitecore/content/" + path;
                    item = Db.GetItem(path, lang, version);
                }

            }
            else
            {
                throw new Exception("Item does not exist in such language");
            }
            return item;
        }

        #endregion
    }
}
