using System.Collections;
using ASR.Reports.Items.Exceptions;
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
	public class QueryScanner : ASR.Interface.BaseScanner
	{
		public static string DB_PARAMETER = "db";
		public static string QUERY_PARAMETER = "query";
		public static string ROOT_PARAMETER = "root";

		Database _db = null;
		/// <summary>
		/// Gets the db.
		/// </summary>
		/// <value>The db.</value>
		public Database Db
		{
			get
			{
				if (_db == null)
				{
					string dbName = getParameter(DB_PARAMETER);
					if (!string.IsNullOrEmpty(dbName))
					{
						_db = Sitecore.Configuration.Factory.GetDatabase(dbName);
					}
					else
					{
						_db = Sitecore.Context.ContentDatabase;
					}

					if (_db == null)
					{
						throw new DatabaseNotFoundException();
					}
				}
				return _db;
			}
		}

		public override ICollection Scan()
		{
			string query = getParameter(QUERY_PARAMETER);
			string root = getParameter(ROOT_PARAMETER);


			Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(query, "Query can't be empty");

			Item[] results;


			if (query.StartsWith("/"))
			{
				results = Db.SelectItems(query);
			}
			else
			{
				Item rootItem = string.IsNullOrEmpty(root) ? Db.GetRootItem() : Db.GetItem(root);
				if (rootItem == null)
				{
					throw new RootItemNotFoundException("Can't find root item " + root);
				}
				results = rootItem.Axes.SelectItems(query);
			}
			if (results == null)
				results = new Item[0];

			return results;
		}
	}
}

