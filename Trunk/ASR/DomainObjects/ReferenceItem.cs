using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePoint.DomainObjects;
using System.Reflection;

namespace ASR.App
{
    
    [Template ("system/item reporter/reference")]
    public class ReferenceItem:CorePoint.DomainObjects.SC.StandardTemplate
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
        [Field("attributes")]
        protected string _Attributes
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
            object o = Sitecore.Reflection.ReflectionUtil.CreateObject(_Assembly, _Class, args);

            if (!string.IsNullOrEmpty(_Attributes))
            {
                string[] attrs = _Attributes.Split('&');
                foreach (string attr in attrs)
                {
                    string[] stArr = attr.Split('=');
                    if (!string.IsNullOrEmpty(stArr[0]) && !string.IsNullOrEmpty(stArr[1]))
                    {
                        PropertyInfo pi =
                            o.GetType().GetProperty(stArr[0]);
                        if (pi!=null)
                        {
                            pi.SetValue(o, stArr[1], null);
                        }
                    }
                }
            }
            
            return o;
        }

    }
}
