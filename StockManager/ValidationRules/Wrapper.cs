using System.Windows;

namespace StockManager.ValidationRules
{
    public class Wrapper : DependencyObject
    {
        public static readonly DependencyProperty ValueProperty =
             DependencyProperty.Register("Value", typeof(object), typeof(Wrapper), new FrameworkPropertyMetadata(null));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }
}
