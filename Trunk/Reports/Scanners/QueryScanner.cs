using System.Collections;
using ASR.Reports.Items.Exceptions;
using ASR.Reports.Scanners;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace ASR.Reports.Items
{
	/// <summary>
	/// Scans a database according to a Sitecore Query. Accepts as parameters:
	/// query: the sitcore query,
	/// db: the database to use. By default "master".
	/// root: the root item (an ID or a path). By default runs the query against the database object.
	/// </summary>
	public class QueryScanner : DatabaseScanner
	{
		
		public static string QUERY_PARAMETER = "query";
		

		

		public override ICollection Scan()
		{
			string query = getParameter(QUERY_PARAMETER);
			
			Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(query, "Query can't be empty");

			Item[] results;


			if (query.StartsWith("/") || query.StartsWith("fast:"))
			{
				results = Database.SelectItems(query);
			}
			else
			{
			    Item rootItem = GetRootItem();
				if (rootItem == null)
				{
					throw new RootItemNotFoundException("Can't find root item " + Root);
				}
				results = rootItem.Axes.SelectItems(query);
			}
		    return results ?? new Item[0];
		}
	}
}

