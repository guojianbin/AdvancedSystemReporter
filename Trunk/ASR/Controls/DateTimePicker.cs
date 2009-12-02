using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Diagnostics;

namespace ASR.Controls
{
    public class DateTimePicker : Sitecore.Web.UI.HtmlControls.DateTimePicker
    {
        protected override void DoRender(System.Web.UI.HtmlTextWriter output)
        {
            output.Write("<div style='display:inline;float:left'>");
            base.DoRender(output);
            output.Write("</div>");

        }
    }
}
