using System;
using Sitecore.Data;

namespace ASR.Reports.Filters
{
	class ItemsOfType : ASR.Interface.BaseFilter
	{
		public string TemplateID { get; set; }

		#region ASR.Interface.BaseFilter Members

		public bool ExecFilter(Sitecore.Data.Items.Item item)
		{
			string[] templateIDs = TemplateID.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (string templateID in templateIDs)
			{
				if (item.TemplateID.ToString() == templateID)
				{
					return true;
				}
			}
			return false;
		}

		#endregion

		public override bool Filter(object element)
		{
			throw new NotImplementedException();
		}
	}
}
