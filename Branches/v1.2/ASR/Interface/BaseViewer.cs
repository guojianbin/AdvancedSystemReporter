using ASR.DomainObjects;
using ASR.Interface;
using CorePoint.DomainObjects.SC;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System.Collections.Generic;
using System.Xml;

namespace ASR.Interface
{
	public abstract class BaseViewer : BaseReportObject
	{
		#region Abstract methods

		public abstract void Display(DisplayElement dElement);

		#endregion

		private static BaseViewer Create(string type)
		{
			return BaseReportObject.createObject(type) as BaseViewer;
		}

		internal static BaseViewer Create(string type, string parameters)
		{
			Assert.ArgumentNotNull(type, "type");
			Assert.ArgumentNotNull(parameters, "parameters");
			BaseViewer oViewer = BaseViewer.Create(type);
			oViewer.AddParameters(parameters);
			return oViewer;
		}

		internal static BaseViewer Create(string type, string parameters, string columnsXml)
		{
			Assert.ArgumentNotNull(type, "type");
			Assert.ArgumentNotNull(parameters, "parameters");
			BaseViewer oViewer = BaseViewer.Create(type);
			oViewer.AddParameters(parameters);
			InitializeColumns(oViewer, columnsXml);
			return oViewer;
		}

		private static void InitializeColumns(BaseViewer oViewer, string columnsXml)
		{
            oViewer.Columns = new List<BaseColumn>();
			if (!string.IsNullOrEmpty(columnsXml))
			{
				XmlDocument doc = new XmlDocument();
				doc.LoadXml(columnsXml);
				XmlNodeList columnNodes = doc.DocumentElement.SelectNodes("Column");

                Database db = Sitecore.Configuration.Factory.GetDatabase(Settings.Instance.ConfigurationDatabase);
                Item ViewerColumnFolder = db.GetItem(Settings.Instance.ViewerColumnsFolder);
                SCDirector director = new SCDirector("master", "en");

				for (int i = 0; i < columnNodes.Count; i++)
				{
                    string name = columnNodes[i].Attributes["name"].Value;
                    Item item = ViewerColumnFolder.Axes.GetItem(name);
					ColumnItem ci = director.LoadObjectFromItem<ColumnItem>(item);

                    ci.Header = columnNodes[i].InnerText;

                    //If this is the wildcard item populate the name too
                    if (item.Name == "*")
                        ci.Name = name;
                    
                    if (columnNodes[i].Attributes["attributes"] != null) 
                        ci.Attributes = columnNodes[i].Attributes["attributes"].Value;

					oViewer.Columns.Add(BaseColumn.Create(ci));
				}
			}
		}

		/// <summary>
		/// Gets the columns.
		/// </summary>
		/// <value>The columns.</value>
		public List<BaseColumn> Columns { get; protected set; }
	}
}
