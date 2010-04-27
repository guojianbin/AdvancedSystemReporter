
namespace ASR.Reports.Logs
{
  public class AuditFilter : Interface.BaseFilter
  {
    public static string USER_PARAMETER = "user";
    public static string VERB_PARAMETER = "verb";

    public string Verb { get { return getParameter(VERB_PARAMETER); } }
    public string User { get { return getParameter(USER_PARAMETER); } }

    public override bool Filter(object element)
    {
      var ai = element as AuditItem;

      if (ai == null)
      {
        return true;
      }

      return (string.IsNullOrEmpty(Verb) || ai.Verb == null ||
              ai.Verb.Contains(Verb))
              && (string.IsNullOrEmpty(User) || ai.User == null ||
            ai.User.Equals(User, System.StringComparison.InvariantCultureIgnoreCase));
    }
  }
}
