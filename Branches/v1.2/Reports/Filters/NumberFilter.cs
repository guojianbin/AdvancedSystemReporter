
namespace ASR.Reports.Items
{
	public abstract class NumberFilter : ASR.Interface.BaseFilter
	{
		public static string NUMBER_PARAMETER = "number";

		private int _number = int.MinValue;
		protected int number
		{
			get
			{
				if (!int.TryParse(getParameter(NUMBER_PARAMETER), out _number))
					_number = 0;

				return _number;
			}
		}
	}
}
