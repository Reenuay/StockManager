using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StockManager.ViewModels;

namespace StockManager.Views {
    /// <summary>
    /// Логика взаимодействия для BackgroundPage.xaml
    /// </summary>
    public partial class BackgroundPage : Page {
        public BackgroundPage() {
            InitializeComponent();
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e) {
            ((BackgroundPageViewModel)DataContext).NewColor = e.NewValue;
        }
    }
}
