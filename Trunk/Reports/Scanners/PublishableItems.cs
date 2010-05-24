using System;
using System.Linq;
using ASR.Reports.Items;
using Sitecore.Data.Items;
using Sitecore;

namespace ASR.Reports.Scanners
{
  class PublishableItems : QueryScanner
  {
    public DateTime DateTime
    {
      get
      {
        var date = getParameter("date");
        return DateUtil.ParseDateTime(date, DateTime.Now);
      }
    }
    public bool Approved { get { return "true" == getParameter("approved"); } }
    public override System.Collections.ICollection Scan()
    {
      var results = base.Scan();
      return results.OfType<Item>().Select(i => i.Publishing.GetValidVersion(DateTime, Approved)).Where(i => i != null).ToArray();
    }
  }
}
