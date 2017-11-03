using System;
using System.Windows.Data;

namespace StockManager.Converters
{
    /// <summary>
    /// Возвращает true или false в зависимости от того, инициализирована ли
    /// переменная или нет.
    /// </summary>
    [ValueConversion(typeof(object), typeof(bool))]
    class ObjectToBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
