using System.Globalization;
using System.Windows.Controls;

namespace StockManager.ValidationRules
{
    public class NotNullValidationRules : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return value == null
                ? new ValidationResult(false, Message)
                : ValidationResult.ValidResult;
        }

        public string Message { get; set; } = "Field is required.";
    }
}
