using System;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace StockManager.Models
{
    /// <summary>
    /// Являет собой элемент главного меню приложения.
    /// </summary>
    class MainMenuItem
    {
        public string Name
        {
            get; set;
        }

        public PackIconKind Icon
        {
            get; set;
        }

        public Lazy<Page> Page
        {
            get; set;
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get; set;
        }
    }
}
