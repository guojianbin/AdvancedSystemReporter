using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASR.Interface;
using ASR.Reports.Items.Exceptions;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Workflows;

namespace ASR.Reports.Items
{
	public class ItemViewer : ASR.Interface.BaseViewer
	{
		public static string COLUMNS_PARAMETER = "columns";
		public static string HEADERS_PARAMETER = "headers";

		public override void Display(DisplayElement d_element)
		{
			Item itemElement = d_element.Element as Item;
			if (itemElement == null)
			{
				return;
			}
			d_element.Value = itemElement.Uri.ToString();

			d_element.Header = itemElement.Name;

			for (int i = 0; i < Columns.Count; i++)
			{
				ASR.DomainObjects.Column column = Columns[i];

				string text = getColumnText(column.Name, itemElement);

				if (string.IsNullOrEmpty(text))
				{
					d_element.AddColumn(column.Header, itemElement[column.Name]);
				}
				else
				{
					d_element.AddColumn(column.Header, text);
				}
			}
			d_element.Icon = itemElement.Appearance.Icon;
		}

		protected virtual string formatDateField(Item item, Sitecore.Data.ID fieldID)
		{
			DateField field = item.Fields[fieldID];
			if (field != null && !String.IsNullOrEmpty(field.Value))
			{
				return field.DateTime.ToString("dd/MM/yyyy HH:mm");
			}
			return string.Empty;
		}

		protected virtual string getColumnText(string name, Item itemElement)
		{
			switch (name)
			{
				case "name":
					return itemElement.Name;

				case "display name":
					return itemElement.DisplayName;

				case "created by":
					return itemElement[FieldIDs.CreatedBy];

				case "updated":
					return formatDateField(itemElement, FieldIDs.Updated);

				case "updated by":
					return itemElement[FieldIDs.UpdatedBy];

				case "created":
					return formatDateField(itemElement, FieldIDs.Created);

				case "locked by":
					Sitecore.Data.Fields.LockField lf = itemElement.Fields["__lock"];
					string locked = "unlocked";
					if (lf != null)
					{
						if (!string.IsNullOrEmpty(lf.Owner))
							locked = lf.Owner + " " + lf.Date.ToString("dd/MM/yy HH:mm");
					}
					return locked;
				case "template":
					return itemElement.Template.Name;

				case "path":
					return itemElement.Paths.FullPath;

				case "owner":
					return itemElement[FieldIDs.Owner];

				case "workflow":
					return getWorkflowInfo(itemElement);

				case "children count":
					return itemElement.Children.Count.ToString();

				case "version":
					return itemElement.Version.ToString();

				case "language":
					return itemElement.Language.CultureInfo.DisplayName;
				default:
					return getFriendlyFieldValue(name, itemElement);

			}
		}

		private string getFriendlyFieldValue(string name, Item itemElement)
		{
			Field field = itemElement.Fields[name];
			if (field != null)
			{
				switch (field.TypeKey)
				{
					case "date":
					case "datetime":
						return formatDateField(itemElement, field.ID);
					case "droplink":
					case "droptree":
						LookupField lookupFld = (LookupField)field;
						if (lookupFld != null && lookupFld.TargetItem != null)
						{
							return lookupFld.TargetItem.Name;
						}
						break;
					case "checklist":
					case "droplist":
					case "grouped droplink":
					case "grouped droplist":
					case "multilist":
					case "treelist":
					case "treelistex":
						MultilistField multilistField = (MultilistField)field;
						if (multilistField != null)
						{
							StringBuilder strBuilder = new StringBuilder();
							foreach (Item item in multilistField.GetItems())
							{
								strBuilder.AppendFormat("{0}, ", item.Name);
							}
							return strBuilder.ToString().TrimEnd(',', ' ');
						}
						break;
					default:
						return field.Value;
				}
			}
			return String.Empty;
		}

		private string getWorkflowInfo(Item itemElement)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			Sitecore.Workflows.IWorkflow iw = itemElement.State.GetWorkflow();
			if (iw != null)
			{
				sb.Append(iw.Appearance.DisplayName);
			}
			Sitecore.Workflows.WorkflowState ws = itemElement.State.GetWorkflowState();

			if (ws != null)
			{
				sb.AppendFormat(" ({0})", ws.DisplayName);
			}

			if (iw != null)
			{
				IEnumerable<WorkflowEvent> events = iw.GetHistory(itemElement).OrderByDescending(e => e.Date);
				IEnumerator<WorkflowEvent> enumerator = events.GetEnumerator();
				if (enumerator.MoveNext())
				{
					TimeSpan span = DateTime.Now.Subtract(enumerator.Current.Date);
					sb.AppendFormat(" for {0} days {1} hours {2} minutes", span.Days, span.Hours, span.Minutes);
				}
			}
			return sb.ToString();
		}
	}
}
