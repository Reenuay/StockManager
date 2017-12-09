using StockManager.Models;

namespace StockManager.ViewModels
{
    class SelectableListBoxItem<T> : ViewModelBase where T : Base
    {
        public T Item { get; private set; }

        public bool IsSelected { get; set; }

        public SelectableListBoxItem(T theme, bool isSelected = false)
        {
            Item = theme;
            IsSelected = isSelected;
        }
    }
}
