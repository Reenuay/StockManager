using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using StockManager.Commands;
using StockManager.Views;

namespace StockManager.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Fields

        private List<MainMenuItemViewModel> mainMenuItems;
        private Page currentPage;
        private ICommand changePageCommand;

        #endregion

        #region Properties / Events

        public event PropertyChangedEventHandler PropertyChanged;

        public MainMenuItemViewModel[] MainMenuItems
        {
            get
            {
                return mainMenuItems.ToArray();
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
                            if (p is Lazy<Page> page)
                            {
                                if (CurrentPage != page.Value)
                                {
                                    CurrentPage = page.Value;
                                }
                            }
                        },
                        p => p is Lazy<Page>
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
            mainMenuItems = new List<MainMenuItemViewModel>
            {
                new MainMenuItemViewModel()
                {
                    Name = "Home",
                    Icon = PackIconKind.Home,
                    Page = new Lazy<Page>(() => new HomePage()
                    {
                        DataContext = new HomePageViewModel()
                    })
                },
                new MainMenuItemViewModel()
                {
                    Name = "Icons",
                    Icon = PackIconKind.FormatListBulleted,
                    Page = new Lazy<Page>(() => new IconBasePage()
                    {
                        DataContext = new IconBasePageViewModel()
                    })
                },
                new MainMenuItemViewModel()
                {
                    Name = "Settings",
                    Icon = PackIconKind.Settings,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Page = new Lazy<Page>(() => new SettingsPage()
                    {
                        DataContext = new SettingsPageViewModel()
                    })
                }
            };

            //Переходим на домашнюю страницу
            CurrentPage = mainMenuItems[0].Page.Value;
        }
    }
}
