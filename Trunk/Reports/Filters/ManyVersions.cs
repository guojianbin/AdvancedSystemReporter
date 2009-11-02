using System;
using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
    public class ManyVersions : ASR.Interface.BaseFilter
    {
		/// <summary>
		/// Number of versions, 1 by default
		/// </summary>
		private int numberOfVersions = 1;

		/// <summary>
		/// Gets or sets the number of versions.
		/// </summary>
		/// <value>The number of versions.</value>
		public string NumberOfVersions
		{
			set
			{
				int parameter;
				Int32.TryParse(value, out parameter);
				numberOfVersions = Math.Max(1, parameter);
			}
		}

		#region ASR.Interface.BaseFilter Members

		/// <summary>
		/// Executes the filter.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public bool ExecFilter(Item item)
		{
			return ((item != null) && (item.Versions.Count > numberOfVersions));
		}

		#endregion

		public override bool Filter(object element)
		{
			throw new NotImplementedException();
		}
	}
}

