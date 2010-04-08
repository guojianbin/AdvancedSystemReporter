using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePoint.DomainObjects;

namespace ASR.DomainObjects
{
    [Template("System/ASR/Column")]
    public class ColumnItem : ReferenceItem
    {

        private string _Key;
        [Field("key")]
        public string Key
        {
            get
            {
                return _Key;
            }
            set
            {
                _Key = value.ToLower();
            }
        }
        [Field("defaultheader")]
        public string DefaultHeader { get; set; }

        [Field("columnhelp")]
        public string ColumnHelp { get; set; }

        public string Header { get; set; }
    }
}
