using StockManager.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace StockManager.ValidationRules
{
    class UniqueTemplateNameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var collection = TemplatesCollection.Value as IEnumerable<Template>;
            var newName = value as string;

            return collection.Any(t => t.Name == newName)
                ? new ValidationResult(false, Message)
                : ValidationResult.ValidResult;
        }

        public Wrapper TemplatesCollection { get; set; }

        public string Message { get; set; } = "Template with this name is already existing.";
    }
}