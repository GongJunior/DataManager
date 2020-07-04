using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace DMCoreGUI.Validation
{
    public class StartRowValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var _ = Int32.Parse((string)value);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Illegal input given, try again");
            }

            return ValidationResult.ValidResult;
        }
    }

    public class SheetNameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty((string)value))
            {
                return new ValidationResult(false, "Sheet Required!");
            }
            return ValidationResult.ValidResult;
        }
    }
}
