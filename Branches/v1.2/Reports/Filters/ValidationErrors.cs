using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASR.Interface;
using Sitecore.Data.Validators;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace ASR.Reports.Filters
{
	public class ValidationErrors : BaseFilter
	{
		public override bool Filter(object element)
		{
			Item item = element as Item;
			if (item != null)
			{
				ValidatorCollection validators = ValidatorManager.BuildValidators(Mode, item);
				ValidatorManager.Validate(validators, new ValidatorOptions(false));
				foreach (BaseValidator validator in validators)
				{
					if (validator.Result >= MinErrorLevel)
					{
						return true;
					}
				}
			}
			return false;
		}

		public ValidatorResult MinErrorLevel
		{
			get
			{
				string value = base.getParameter("MinErrorLevel");
				return (ValidatorResult)Enum.Parse(typeof(ValidatorResult), value);
			}
		}
		public ValidatorsMode Mode
		{
			get
			{
				string value = getParameter("Mode");
				return (ValidatorsMode)Enum.Parse(typeof(ValidatorsMode), value);
			}
		}
	}
}
