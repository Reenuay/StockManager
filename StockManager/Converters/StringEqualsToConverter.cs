using System;
using System.Windows.Data;

namespace StockManager.Converters {
    public class StringEqualsToConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return ((string)parameter == (string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return (bool)value ? parameter : Binding.DoNothing;
        }
    }
}
