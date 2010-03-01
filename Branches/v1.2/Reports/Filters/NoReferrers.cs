using Sitecore.Data.Items;
using Sitecore.Links;

namespace ASR.Reports.Filters
{
	class NoReferrers : ASR.Interface.BaseFilter
	{
		/// <summary>
		/// Filters the specified element.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns></returns>
		public override bool Filter(object element)
		{
			Item item = element as Item;
			if (item != null)
			{
				ItemLink[] links = Sitecore.Globals.LinkDatabase.GetReferrers(item);
				if (links.Length == 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}
