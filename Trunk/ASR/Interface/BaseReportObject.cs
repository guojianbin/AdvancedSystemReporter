using System;
using System.Collections.Specialized;

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
		}

		public bool UpdateParameters(string values)
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
