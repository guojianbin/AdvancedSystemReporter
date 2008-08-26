using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePoint.DomainObjects.SC;
using CorePoint.DomainObjects;
using ASR.App;

namespace ASR.App
{
    [Template("system/item reporter/command")]
    public class CommandItem:ReferenceItem
    {
        [Field("title")]
        public string Title
        {
            get;
            private set;
        }
        [Field("icon")]
        public string Icon
        {
            get;
            private set;
        }

        [Field("iconall")]
        public string IconAll
        {
            get;
            private set;
        }
    }
}
