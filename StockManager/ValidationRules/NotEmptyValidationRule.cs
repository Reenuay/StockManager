using System.Globalization;
using System.Windows.Controls;

namespace StockManager.ValidationRules
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, Message)
                : ValidationResult.ValidResult;
        }

        public string Message { get; set; } = "Field is required.";
    }
}
