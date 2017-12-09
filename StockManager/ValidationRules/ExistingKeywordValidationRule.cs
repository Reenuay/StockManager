using StockManager.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace StockManager.ValidationRules
{
    class ExistingKeywordValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var collection = KeywordsCollection.Value as IEnumerable<Keyword>;
            var newName = value as string;

            return collection.Any(k => k.Name == newName.ToLower())
                ? new ValidationResult(false, Message)
                : ValidationResult.ValidResult;
        }

        public Wrapper KeywordsCollection { get; set; }

        public string Message { get; set; } = "Keyword already existing.";
    }
}

