using System;

namespace ASR.Reports.Filters
{
	class ItemsWithResettableVersions : ASR.Interface.BaseFilter
	{
		#region ASR.Interface.BaseFilter Members

		public bool ExecFilter(Sitecore.Data.Items.Item item)
		{
			return ((item.Versions.Count == 1) && (item.Version.Number > 1));
		}

		#endregion

		public override bool Filter(object element)
		{
			throw new NotImplementedException();
		}
	}
}
