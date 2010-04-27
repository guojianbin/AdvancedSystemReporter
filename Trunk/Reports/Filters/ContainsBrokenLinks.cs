using ASR.Interface;
using Sitecore.Data.Items;

namespace ASR.Reports.Filters
{
  public class ContainsBrokenLinks : BaseFilter
  {
    public static string ALLVERSIONS_PARAMETER = "allversions";
    public override bool Filter(object element)
    {
      var item = element as Item;
      if (item == null) return true;
      var allversions = "true" == getParameter(ALLVERSIONS_PARAMETER);
      var brokenlinks = item.Links.GetBrokenLinks(allversions);
      return (brokenlinks != null && brokenlinks.Length > 0);
    }
  }
}
