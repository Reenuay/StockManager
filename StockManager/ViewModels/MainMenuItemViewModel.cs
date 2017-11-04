using System;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using PropertyChanged;

namespace StockManager.ViewModels
{
    /// <summary>
    /// Являет собой элемент главного меню приложения.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    class MainMenuItemViewModel
    {
        public string Name { get; set; }
        public PackIconKind Icon { get; set; }
        public Lazy<Page> Page { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public bool IsSelected { get; set; }
    }
}
