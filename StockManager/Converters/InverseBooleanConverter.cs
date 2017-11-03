﻿using System;
using System.Windows.Data;

namespace StockManager.Converters
{
    /// <summary>
    /// Инвертирует булевское значение.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                throw new InvalidOperationException("The value must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (!(value is bool))
                throw new InvalidOperationException("The value must be a boolean");

            return !(bool)value;
        }

        #endregion
    }
}
