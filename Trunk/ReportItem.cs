using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePoint.DomainObjects;

namespace ASR
{
    [Serializable]
    [Template ("system/item reporter/report")]
    class ReportItem : CorePoint.DomainObjects.SC.StandardTemplate
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
        
        public IScanner GetScanner()
        {
            ReferenceItem scItem = Director.GetObjectByIdentifier<ReferenceItem>(_Scanner);
            Sitecore.Diagnostics.Debug.Assert(scItem != null, "could not load scanner");
            return (IScanner)scItem.MakeObject() as IScanner;
        }

        
        public CommandItem GetCommand()
        {
            List<CommandItem> commands =
                Director.GetChildObjects<CommandItem>(this.Id);
            if (commands.Count>0)
            {
                return commands[0];
            }
            return null;
        }
        public List<FilterItem> GetFilters()
        {
            List<FilterItem> filters =
                Director.GetChildObjects<FilterItem>(this.Id);
            return filters;
        }
        

        
    }
}
