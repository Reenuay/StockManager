using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using StockManager.Commands;
using StockManager.Models;
using StockManager.Views;

namespace StockManager.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        private List<MainMenuItem> _mainMenuItems;
        private Page currentPage;
        private ICommand changePageCommand;

        #endregion

        #region Properties / Events

        public event PropertyChangedEventHandler PropertyChanged;

        public MainMenuItem[] MainMenuItems
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
                return currentPage;
            }

            set
            {
                if (currentPage != value)
                {
                    currentPage = value;
                    NotifyPropertyChanged(nameof(CurrentPage));
                }
            }
        }

        public ICommand ChangePageCommand
        {
            get
            {
                changePageCommand = changePageCommand
                    ?? new RelayCommand(
                        p =>
                        {
                            Page page = p as Page;
                            if (CurrentPage != page && page != null)
                            {
                                CurrentPage = page;
                            }
                        },
                        p => p is Page
                    );

                return changePageCommand;
            }
        }

        #endregion

        #region Methods

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName)
            );
        }

        #endregion

        public MainWindowViewModel()
        {
            //Создаём список элементов меню
            _mainMenuItems = new List<MainMenuItem>
            {
                new MainMenuItem()
                {
                    Name = "Home",
                    Icon = PackIconKind.Home,
                    Page = new HomePage()
                    {
                        DataContext = new HomePageViewModel()
                    }
                },
                new MainMenuItem()
                {
                    Name = "IconBase",
                    Icon = PackIconKind.FormatListBulleted,
                    Page = new IconBasePage()
                    {
                        DataContext = new IconBasePageViewModel()
                    }
                },
                new MainMenuItem()
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
}
