using ASR.Interface;
using Sitecore.Diagnostics;
using Sitecore.Data.Items;
using Sitecore.Workflows;

namespace ASR.Reports.Filters
{
    class StaleWorkflowItems : BaseFilter
    {
        public const string AGE_PARAMETER = "Age";
        private int _age = int.MinValue;
        protected int Age
        {
            get
            { 
                if(_age == int.MinValue)
                {
                    if (!int.TryParse(getParameter(AGE_PARAMETER), out _age))
                    {
                        _age = 0;
                    }
                }
                return _age;
            }
        }

        public override bool Filter(object element)
        {
            Debug.ArgumentNotNull(element, "element");
            Item item = element as Item;
            if (item == null) return true;

            IWorkflow wf = Sitecore.Context.Workflow.GetWorkflow(item);
            if (wf == null) return false;
            WorkflowState state = wf.GetState(item);
            if (state == null || state.FinalState) return false;

            WorkflowEvent[] wevents = wf.GetHistory(item);
            if (wevents == null || wevents.Length == 0) return false;
            return (System.DateTime.Now - wevents[wevents.Length - 1].Date).Days > Age;
                        
        }
    }
}
