using System;
using System.Globalization;
using System.Windows.Controls;

namespace DMCoreGUI.Validation
{
    public class StartRowValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var _ = int.Parse((string)value);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Row must be a number!");
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
