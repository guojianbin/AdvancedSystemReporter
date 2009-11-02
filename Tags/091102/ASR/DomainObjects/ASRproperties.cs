using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASR.App;
using Sitecore.Data.Items;
using CorePoint.DomainObjects.SC;
using ASR.Commands;

namespace ASR.App
{
    [Serializable]
    public class ASRproperties
    {
        public Sitecore.Data.ID CONFIGNODE_ID = new Sitecore.Data.ID("{30BB56D2-4F1E-4ECD-B7A6-D32A902DD8E5}");

        private string _databasename;

        public string Databasename
        {
            get { return _databasename; }
        }

        [NonSerialized]
        private Sitecore.Data.Database _db;
        public Sitecore.Data.Database Db
        {
            get
            {
                if (_db == null)
                {
                    _db = Sitecore.Context.ContentDatabase;
                    _databasename = _db.Name;
                }
                return _db;
            }

        }
        
        [NonSerialized]
        private ReportItem _reportItem;
        
        
         public ReportItem ReportItem
        {
            get
            {
                if (_reportItem == null && _reportItemId != Guid.Empty)
                {
                    _reportItem  = new SCDirector(Db.Name,"en").GetObjectByIdentifier<ReportItem>(_reportItemId);
                }
                return _reportItem;
            }
            private set
            {
                _reportItem = value;
                if (_reportItem != null)
                    _reportItemId = _reportItem.Id;
                else
                    _reportItemId = Guid.Empty;

                Save();
            }
        }

        private void Save()
        {
            Form.Properties = this;
        }

        private Guid _reportItemId;

        [NonSerialized]
        private Item _rootItem;

        private Guid _rootItemId = new Guid("{11111111-1111-1111-1111-111111111111}");
        public Item RootItem
        {
            get
            {
                if (_rootItem == null)
                {
                    if (_rootItemId != null && _rootItemId != Guid.Empty)
                    {
                        _rootItem = Db.GetItem(new Sitecore.Data.ID(_rootItemId));
                    }
                }
                return _rootItem;
            }
            set
            {
                _rootItem = value;
                if (_rootItem != null)
                {
                    _rootItemId = _rootItem.ID.Guid;
                }
                else
                {
                    _rootItemId = Guid.Empty;
                }
                Save();
            }
        }

        [NonSerialized]
        private ASR.App.MainForm _form;
        
        public ASR.App.MainForm Form
        {
            get
            {
                return _form;
            }
            set
            {
                _form = value;
            }
        }
       
        public ASRproperties(ASR.App.MainForm form)
        {
            Form = form;
            
        }
        
        public void SetReportItem(Guid id)
        {
            if (!id.Equals(Guid.Empty))
            {
                CorePoint.DomainObjects.SC.SCDirector director = new CorePoint.DomainObjects.SC.SCDirector("master", "en");
                ReportItem = director.GetObjectByIdentifier<ReportItem>(id);
            }
            else
            {
                ReportItem = null;
            }
        }
       
        public ICommand Command
        {
            get
            {
                if (ReportItem != null)
                {
                    CommandItem ci = ReportItem.Command;
                    if (ci != null)
                    {
                        return ci.MakeObject() as ICommand;
                    }
                }
                return null;
            }
        }

        [NonSerialized]
        private string _jobName;

        public string JobName
        {
            get
            {
                if (_jobName == null)
                {
                    _jobName = Form.ServerProperties["ASR:jobname"].ToString();
                }
                return _jobName;
            }
        }

        public void CreateJobName()
        {
            string jobname = Guid.NewGuid().ToString();

            Form.ServerProperties.Add("ASR:jobname", jobname);
        }
    }

}
