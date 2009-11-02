using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorePoint.DomainObjects;

namespace ASR.App
{
    [Template("system/item reporter/standard field mapping")]
    public class FieldItem : CorePoint.DomainObjects.SC.StandardTemplate
    {
        [Field("type")]
        public string _parameters
        {
             get;
            private set;
        }
        [Field("fieldid")]
        public string FieldID
        {
            get;
            private set;
        }
        private string _fieldtype;

        public string Fieldtype
        {
            get 
            {
                if (_fieldtype==null)
                {
                    digest();
                }
                return _fieldtype; 
            }

        }
        private string[] _props;

        public string[] Props
        {
            get 
            {
                if (_props==null)
                {
                    digest();
                }
                return _props; 
            }

        }

        
        private void digest()
        {
            if (!string.IsNullOrEmpty(_parameters))
            {
                int i = _parameters.IndexOf('|');
                if (i > 0)
                {
                    _fieldtype = _parameters.Substring(0, i);
                    _props = _parameters.Substring(i + 1).Split(',');
                }
            }
        }
    }
}
