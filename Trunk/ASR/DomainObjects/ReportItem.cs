using System;
using System.Collections.Generic;
using CorePoint.DomainObjects;
using ASR.Scanners;

namespace ASR.App
{
    [Serializable]
    [Template ("system/item reporter/report")]
    public class ReportItem : CorePoint.DomainObjects.SC.StandardTemplate
    {
        [Field("icon")]
        public string Icon
        {
            get;
            private set;
        }
        [Field("scanner")]
        private Guid _Scanner
        {
            get;
            set;
        }
        
        [Field("text")]
        public string Text
        {
            get;
            private set;
        }

        [Field("language")]
        public bool Language
        {
            get;
            private set;
        }

        [Field("version")]
        public bool Version
        {
            get;
            private set;
        }


        [Field("custom fields")]
        private string _customFields
        {
            get;
            set;
        }

        private string[] customFields;
        public string[] CustomFields
        {
            get
            {
                if (customFields == null)
                {
                    if (string.IsNullOrEmpty(_customFields))
                    {
                        customFields = new string[0];
                    }
                    else
                        customFields = _customFields.Split(',');

                }
                return customFields;
            }
        }

        [Field("standard fields")]
        private List<Guid> stFields
        {
            get;
            set;
        }
        private List<FieldItem> _stFields = null;
        public  List<FieldItem> StFields
        {
            get
            {
                if (_stFields == null)
                {
                    _stFields = stFields.ConvertAll<FieldItem>(
                               delegate(Guid g)
                               {
                                   return Director.GetObjectByIdentifier<FieldItem>(g);
                               }
                           ); 
                }
                return _stFields;
            }
        }
        
        public IScanner GetScanner()
        {
            ReferenceItem scItem = Director.GetObjectByIdentifier<ReferenceItem>(_Scanner);
            Sitecore.Diagnostics.Debug.Assert(scItem != null, "could not load scanner");
            return (IScanner)scItem.MakeObject() as IScanner;
        }
        [Field("command")]
        private Guid _Command
        {
            get;
            set;
        }
        private CommandItem _command;
        public CommandItem Command
        {
            get
            {
                if (_command==null)
                {
                    if (_Command == Guid.Empty)
                        _command = null;
                    else
                        _command = Director.GetObjectByIdentifier<CommandItem>(_Command);
                }
                
                return _command;
            }
        }

        [Field("filters")]
        private List<Guid> _Filters
        {
            get;
            set;
        }
        private List<FilterItem> _filters;
        public List<FilterItem> Filters
        {
             get
            {
                if (_filters == null)
                {
                    _filters = _Filters.ConvertAll<FilterItem>(
                               delegate(Guid g)
                               {
                                   return Director.GetObjectByIdentifier<FilterItem>(g);
                               }
                           ); 
                }
                return _filters;
            }
        }
        

        
    }
}
