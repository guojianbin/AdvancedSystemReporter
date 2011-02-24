using System;
using System.Collections.Specialized;
using System.Linq;

namespace ASR.Interface
{
	public abstract class BaseReportObject
	{
		public string Name { get; set; }

		private NameValueCollection parameters;

		private NameValueCollection makeCollection(string values)
		{
			return Sitecore.StringUtil.ParseNameValueCollection(values, '|', '=');
		}

		public void AddParameters(string values)
		{
			parameters = makeCollection(values);
            assignProperties(values);
		}

		public bool UpdateParameters(string values)
		{
		    var flag = updateParameters(values);
		    if( flag)
		    {
		        assignProperties(values);
		    }
		    return flag;
		}

	    private bool updateParameters(string values)
	    {
	        if (parameters == null)
	        {
	            AddParameters(values);
	            return true;
	        }
	        NameValueCollection nvc = makeCollection(values);
	        if (nvc.Count != parameters.Count)
	        {
	            AddParameters(values);
	            return true;
	        }

	        foreach (var key in nvc.AllKeys)
	        {
	            if (nvc[key] != parameters[key])
	            {
	                parameters = nvc;
	                return true;
	            }
	        }
	        return false;
	    }

	    private void assignProperties(string values)
        {
            var parameters = Sitecore.StringUtil.GetNameValues(values, '=', '|');
            foreach(string param in parameters)
            {
                Sitecore.Reflection.ReflectionUtil.SetProperty(this,param,parameters[param]);
            }                       
        }

		protected string getParameter(string name)
		{
			if (parameters == null)
			{
				return null;
			}
			return parameters[name];
		}

		protected static object createObject(string type)
		{
			return Sitecore.Reflection.ReflectionUtil.CreateObject(type, new object[] { });
		}
	}
}
