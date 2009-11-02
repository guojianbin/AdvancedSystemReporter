using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASR
{
	public class Settings
	{
		private static Settings _instance;
		public static Settings Instance
		{
			get
			{
				if (Settings._instance == null)
				{
					Settings._instance = new Settings();
				}
				return Settings._instance;
			}
		}

		protected Settings() { }

		/// <summary>
		/// Gets the configuration node.
		/// </summary>
		/// <value>The configuration node.</value>
		public string ConfigurationNode
		{
			get
			{
				return "/sitecore/system/Modules/ASR";
			}
		}

		/// <summary>
		/// Gets the configuration database.
		/// </summary>
		/// <value>The configuration database.</value>
		/// 
		public string ConfigurationDatabase
		{
			get
			{
				return "master";
			}
		}

		public string EmailFrom
		{
			get
			{
				return Sitecore.Context.User.Profile.Email;
			}
		}
		/// <summary>
		/// Gets the reports folder.
		/// </summary>
		/// <value>The reports folder.</value>
		public string ReportsFolder
		{
			get
			{
				return "/sitecore/system/Modules/ASR/Reports";
			}
		}
		/// <summary>
		/// Gets the size of the page.
		/// </summary>
		/// <value>The size of the page.</value>
		public int PageSize
		{
			get
			{
				return 30;
			}
		}

		/// <summary>
		/// Gets the max number pages.
		/// </summary>
		/// <value>The max number pages.</value>
		public int MaxNumberPages
		{
			get
			{
				return 40;
			}
		}

		public string ParametersFolder
		{
			get
			{
				return "/sitecore/system/Modules/ASR/Configuration/Parameters";
			}
		}
	}
}
