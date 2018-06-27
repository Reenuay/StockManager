﻿using StockManager.Models;

namespace StockManager.ViewModels
{
    class SelectableListBoxItem<T> : ViewModelBase where T : Base
    {
        public T Item { get; private set; }

        public bool IsSelected { get; set; }

        public SelectableListBoxItem(T item, bool isSelected = false)
        {
            Item = item;
            IsSelected = isSelected;
        }
    }
}
