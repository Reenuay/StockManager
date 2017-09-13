using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using StockManager.Utils;
using StockManager.Views;

namespace StockManager.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        private List<MainMenuItemSource> _mainMenuItems;
        private Page _currentPage;
        private ICommand _changePageCommand;

        #endregion

        #region Properties / Events

        public event PropertyChangedEventHandler PropertyChanged;

        public MainMenuItemSource[] MainMenuItems
        {
            get
            {
                return _mainMenuItems.ToArray();
            }
        }

        public Page CurrentPage
        {
            get
            {
                return _currentPage;
            }

            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    NotifyPropertyChanged("CurrentPage");
                }
            }
        }

        public ICommand ChangePageCommand
        {
            get
            {
                _changePageCommand = _changePageCommand
                    ?? new RelayCommand(p => p is Page, p => ChangePage(p as Page));

                return _changePageCommand;
            }
        }

        #endregion

        #region Methods

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ChangePage(Page page)
        {
            if (CurrentPage != page && page != null)
            {
                CurrentPage = page;
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            //Создаём список элементов меню
            _mainMenuItems = new List<MainMenuItemSource>
            {
                new MainMenuItemSource()
                {
                    Name = "Home",
                    Icon = PackIconKind.Home,
                    Page = new HomePage()
                    {
                        DataContext = new HomePageViewModel()
                    }
                },
                new MainMenuItemSource()
                {
                    Name = "IconBase",
                    Icon = PackIconKind.FileTree,
                    Page = new IconBasePage()
                    {
                        DataContext = new IconBasePageViewModel()
                    }
                },
                new MainMenuItemSource()
                {
                    Name = "Settings",
                    Icon = PackIconKind.Settings,
                    Page = new SettingsPage()
                    {
                        DataContext = new SettingsPageViewModel()
                    },
                    HorizontalAlignment = HorizontalAlignment.Right
                }
            };

            //Переходим на домашнюю страницу
            CurrentPage = _mainMenuItems[0].Page;
        }
    }

    struct MainMenuItemSource
    {
        public string Name
        {
            get; set;
        }

        public PackIconKind Icon
        {
            get; set;
        }

        public Page Page
        {
            get; set;
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get; set;
        }
    }
}
