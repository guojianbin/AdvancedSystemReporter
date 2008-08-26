using System.Collections.Generic;
using ASR.App;
using Sitecore.Data.Items;
using Sitecore.Jobs;
using ASR.Scanners;
using ASR.Filters;

namespace ASR
{
    class ReportRunner
    {
        public ASRproperties Properties
        {
            get;
            set;
        }
        

        public ReportRunner (ASRproperties properties)
        {
            Properties = properties;
        }

        public void Run()
        {

            Sitecore.Diagnostics.Debug.Assert(Properties.ReportItem != null, "There is no report");



            List<FilterItem> filterItems = Properties.ReportItem.Filters;
            IScanner scanner = Properties.ReportItem.GetScanner();
            if (filterItems != null && scanner != null)
            {
                IFilter[] filters = new IFilter[filterItems.Count];
                for (int i = 0; i < filterItems.Count; i++)
                {
                    filters[i] = filterItems[i].MakeObject() as IFilter;
                }



                Properties.CreateJobName();

                Sitecore.Shell.Applications.Dialogs.ProgressBoxes.ProgressBox.Execute(
     Properties.JobName, "Scanning items", Properties.ReportItem.Icon, new Sitecore.Shell.Applications.Dialogs.ProgressBoxes.ProgressBoxMethod(Execute),
     "MainForm:Finished", new object[] { Properties.RootItem, scanner, filters });

            }
        }



        public void Execute(object[] parameters)
        {
            Sitecore.Diagnostics.Log.Info("Starting job", this);
            Item rootitem = parameters[0] as Item;
            IScanner scanner = parameters[1] as IScanner;
            IFilter[] filters = parameters[2] as IFilter[];
            List<Item> itemsToAdd = new List<Item>();
            Job job = Sitecore.Context.Job;
            if (job != null && scanner != null && rootitem != null && itemsToAdd != null)
            {
                using (scanner)
                {
                    foreach (Sitecore.Data.Items.Item item in scanner.Scan(rootitem))
                    {
                        bool add = true;
                        foreach (IFilter myFilter in filters)
                        {
                            if (myFilter != null)
                            {
                                if (!myFilter.ExecFilter(item))
                                {
                                    add = false;
                                    break;
                                }
                            }

                        }


                        job.Status.LogInfo("Scanning " + item.Name);

                        if (add)
                        {
                            itemsToAdd.Add(item);
                        }
                    }
                    job.Options.CustomData = new object[] {itemsToAdd,
                    string.Format("{0} items added {1}", itemsToAdd.Count, scanner.StatusMessage())};
                }
            }



        }



        

   

    }
}
