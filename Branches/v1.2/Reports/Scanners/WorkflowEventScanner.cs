using System;
using System.Collections.Generic;
using System.Linq;
using ASR.Interface;
using System.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Workflows;
using Sitecore.Configuration;
using ASR.Reports.Items.Exceptions;
using ASR.Reports.DisplayItems;

namespace ASR.Reports.Scanners
{
	class WorkflowEventScanner : BaseScanner
	{
		#region Parameters
		public enum Mode { Item = -1, Descendants = 1, Children = 0 };
		private string _deep;
		/// <summary>
		/// Gets the scanning deeph.
		/// </summary>
		/// <value>The deep.</value>
		/// <remarks>We can scan Item (-1), Descendants (1) or Children (0).</remarks>
		public Mode Deep
		{
			get
			{
				if (string.IsNullOrEmpty(_deep))
				{
					_deep = getParameter("deep");
				}
				return (Mode)int.Parse(_deep);
			}
		}

		private Item _root;
		/// <summary>
		/// Gets the root.
		/// </summary>
		/// <value>The root.</value>
		public Item Root
		{
			get
			{
				if (_root == null)
				{
					_root = Db.GetItem(getParameter("root"));
				}
				return _root;
			}
		}

		public string _allversions;
		/// <summary>
		/// Gets a value indicating whether all versions should be scanned.
		/// </summary>
		/// <value><c>true</c> if all versions should be scanned; otherwise, <c>false</c>.</value>
		public bool AllVersions
		{
			get
			{
				if (string.IsNullOrEmpty(_allversions))
				{
					_allversions = getParameter("allversions");
				}
				return _allversions == "1";
			}
		}

		private Database _db;
		public Database Db
		{
			get
			{
				if (_db == null)
				{
					string databaseName = base.getParameter("db");
					if (!string.IsNullOrEmpty(databaseName))
					{
						_db = Factory.GetDatabase(databaseName);
					}
					if (_db == null)
					{
						throw new DatabaseNotFoundException();
					}
				}
				return _db;
			}
		}
		#endregion

		/// <summary>
		/// Scans for workflow events.
		/// </summary>
		/// <returns></returns>
		public override ICollection Scan()
		{
			List<ItemWorkflowEvent> results = new List<ItemWorkflowEvent>();

			Item[] items;
			switch (Deep)
			{
				case Mode.Descendants:
					items = Root.Axes.GetDescendants();
					break;
				case Mode.Children:
					items = Root.Axes.SelectItems("./*");
					break;
				default:
					items = new Item[] { Root };
					break;
			}

			foreach (Item item in items)
			{
				if (AllVersions)
				{
					var versions = item.Versions.GetVersions().OrderBy(i => i.Version.Number);
					foreach (var version in versions)
					{
						AddEvents(results, version);
					}
				}
				else
				{
					AddEvents(results, item);
				}
			}

			return results;
		}

		public IWorkflow GetWorkflow(Item item)
		{
			return item.Database.WorkflowProvider.GetWorkflow(item);
		}

		/// <summary>
		/// Adds the events.
		/// </summary>
		/// <param name="results">The results.</param>
		/// <param name="version">The version.</param>
		private void AddEvents(List<ItemWorkflowEvent> results, Item item)
		{
			IWorkflow workflow = GetWorkflow(item);
			if (workflow != null)
			{
				var events = from wEvent in workflow.GetHistory(item)
							 orderby wEvent.Date ascending
							 select new ItemWorkflowEvent(wEvent.OldState, wEvent.NewState, wEvent.Text, wEvent.User, wEvent.Date)
							 {
								 Item = item
							 };
				results.AddRange(events);
			}
		}
	}
}
