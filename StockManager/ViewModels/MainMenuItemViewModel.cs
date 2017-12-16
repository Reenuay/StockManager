using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace StockManager.ViewModels
{
    /// <summary>
    /// Являет собой элемент главного меню приложения.
    /// </summary>
    class MainMenuItemViewModel : ViewModelBase
    {
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }
        public Page Page { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public bool IsSelected { get; set; }
    }
}
