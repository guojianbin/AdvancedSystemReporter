using System;
using System.Collections.Generic;
using CorePoint.DomainObjects;
using System.Collections;
using System.Text.RegularExpressions;
using Sitecore.Data;
using Sitecore.Data.Fields;
using System.Linq;
using ASR.Interface;
using Sitecore.Collections;

namespace ASR.DomainObjects
{
    [Serializable]
    [Template ("System/ASR/Report")]
    public class ReportItem : CorePoint.DomainObjects.SC.StandardTemplate
    {

        [Field("__icon")]
        public string Icon
        {
            get;
            set;
        }

        [Field("scanners")]
        private List<Guid> _scanner
        {
            get;
            set;
        }

        public ScannerItem[] _scanners;
        public IEnumerable<ScannerItem> Scanners
        {
            get
            {
                if (_scanners == null)
                {
                    _scanners = _scanner.ConvertAll<ScannerItem>(id => this.Director.GetObjectByIdentifier<ScannerItem>(id)).ToArray();
                }
                return _scanners;
            }
        }

        [Field("viewers")]
        private List<Guid> _viewer
        {
            get;
            set;
        }

        private ViewerItem[] _viewers = null;
        public IEnumerable<ViewerItem> Viewers
        {
            get
            {
                if (_viewers == null)
                {
                    _viewers = _viewer.ConvertAll<ViewerItem>(g => this.Director.GetObjectByIdentifier<ViewerItem>(g)).ToArray();                    
                }
                return _viewers;
            }
        }

        [Field("commands")]
        public List<Guid> _Commands
        {
            get;
            set;
        }
        private CommandItem[] _commands;
        public IEnumerable<CommandItem> Commands
        {
            
            get
            {
                if (_commands == null)
                {
                    _commands = _Commands.ConvertAll<CommandItem>(g => this.Director.GetObjectByIdentifier<CommandItem>(g)).ToArray();
                }
                return _commands;
            }
        }

        [Field("filters")]
        public List<Guid> _Filters
        {
            get;
            set;
        }
        private FilterItem[] _filters;
        public IEnumerable<FilterItem> Filters
        {
            get
            {
                if (_filters == null)
                {
                    _filters = _Filters.ConvertAll<FilterItem>(g => this.Director.GetObjectByIdentifier<FilterItem>(g)).ToArray();

                }
                return _filters;
            }
        }

        [Field("email text")]
        public string EmailText
        {
            get;
            set;
        }
        public void RunCommand(string commandname, StringList values)
        {
            foreach (var item in Commands)
            {
                if (item.Name == commandname)
                {
                    item.Run(values);
                    break;
                }
            }
        }

        private HashSet<ReferenceItem> objects;

        public ReferenceItem FindItem(string name)
        {
            if (objects == null)
            {
                loadObjects();
            }
            return objects.First(ri => ri.Name == name);
        }

        public ReferenceItem FindItem(Guid name)
        {
            if (objects == null)
            {
                loadObjects();
            }
            return objects.First(ri => ri.Id == name);
        }

        private void loadObjects()
        {
            objects = new HashSet<ReferenceItem>();
            foreach (var item in Scanners) { objects.Add(item); }
            foreach (var item in Viewers) { objects.Add(item); }
            foreach (var item in Filters) { objects.Add(item); }
        }

        public string SerializeParameters()
        {
            return SerializeParameters("^", "&");
        }

        public string SerializeParameters(string valueSeparator, string parameterSeparator)
        {
            System.Collections.Specialized.NameValueCollection nvc = 
                new System.Collections.Specialized.NameValueCollection();
            nvc.Add("id", new ID(Current.Context.ReportItem.Id).ToString());
            foreach (var item in Current.Context.ReportItem.Scanners)
            {
                foreach (var p in item.Parameters)
                {
                    nvc.Add(string.Concat(item.Id, valueSeparator, p.Name), p.Value);
                }
            }
            foreach (var item in Current.Context.ReportItem.Filters)
            {
                foreach (var p in item.Parameters)
                {
                    nvc.Add(string.Concat(item.Id, valueSeparator, p.Name), p.Value);
                }
            }
            foreach (var item in Current.Context.ReportItem.Viewers)
            {
                foreach (var p in item.Parameters)
                {
                    nvc.Add(string.Concat(item.Id, valueSeparator, p.Name), p.Value);
                }
            }
            return Sitecore.StringUtil.NameValuesToString(nvc, parameterSeparator);
        }

    }
}
