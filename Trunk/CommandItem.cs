using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePoint.DomainObjects.SC;
using CorePoint.DomainObjects;

namespace ASR
{
    [Template("system/item reporter/command")]
    class CommandItem:ReferenceItem
    {
        [Field("name")]
        public string Name
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
