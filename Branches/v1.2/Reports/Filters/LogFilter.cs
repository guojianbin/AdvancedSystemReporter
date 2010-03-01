using System;

namespace ASR.Reports.Logs
{
    public class LogFilter : ASR.Interface.BaseFilter
    {
        public static string AGE_PARAMETER = "age";

        private int _age = int.MinValue;
        public int Age
        {
            get
            {
                if (_age == int.MinValue)
                {
                    if (!int.TryParse(getParameter(AGE_PARAMETER), out _age))
                    {
                        _age = -1;
                    }
                }
                return _age;
            }
        }
        public override bool Filter(object element)
        {
            LogItem li = element as LogItem;

            if (li == null || Age < 0)
            {
                return true;
            }

            return DateTime.Now.CompareTo(li.DateTime.AddHours(Age)) < 0;
        }
    }
}
