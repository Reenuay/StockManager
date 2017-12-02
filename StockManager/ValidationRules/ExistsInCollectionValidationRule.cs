using System.Globalization;
using System.Windows.Controls;
using System.Collections;

namespace StockManager.ValidationRules
{
    class ExistsInCollectionValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return ((IList)Collection.Value).Contains(value)
                ? new ValidationResult(false, Message)
                : ValidationResult.ValidResult;
        }

        public Wrapper Collection { get; set; }

        public string Message { get; set; }
    }
}
