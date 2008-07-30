using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePoint.DomainObjects;

namespace ASR
{
    
    [Template ("system/item reporter/reference")]
    class ReferenceItem:CorePoint.DomainObjects.SC.StandardTemplate
    {
        [Field ("assembly")]
        protected string _Assembly
        {
            get;
            set;
        }
        [Field("class")]
        protected string _Class
        {
            get;
            set;
        }
        public object MakeObject()
        {
            return MakeObject(new object[] {});
        }
        public object MakeObject(object[] args)
        {
            return Sitecore.Reflection.ReflectionUtil.CreateObject(_Assembly, _Class, args);
        }

    }
}
