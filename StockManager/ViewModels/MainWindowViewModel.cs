using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using StockManager.Commands;
using StockManager.Views;

namespace StockManager.ViewModels
{
    /// <summary>
    /// Реализует логику взаимодействия вида окна и промежуточных данных.
    /// </summary>
    class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        private List<MainMenuItemViewModel> mainMenuItems;
        private ICommand changePageCommand;

        #endregion

        #region Properties

        public MainMenuItemViewModel[] MainMenuItems
        {
            get
            {
                return mainMenuItems.ToArray();
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
                            if (p is MainMenuItemViewModel item)
                            {
                                if (CurrentPage.GetType() != item.Page().GetType())
                                {
                                    CurrentPage = item.Page();
                                    mainMenuItems.ForEach(i =>
                                    {
                                        i.IsSelected = false;
                                    });
                                    item.IsSelected = true;
                                }
                            }
                        },
                        p => p is MainMenuItemViewModel
                    );

                return changePageCommand;
            }
        }

        public Page CurrentPage { get; private set; }

        #endregion

        public MainWindowViewModel()
        {
            //Создаём список элементов меню
            mainMenuItems = new List<MainMenuItemViewModel>
            {
                new MainMenuItemViewModel()
                {
                    Name = "Icons",
                    Icon = PackIconKind.ImageMultiple,
                    Page = () => new IconBasePage()
                    {
                        DataContext = new IconBasePageViewModel()
                    }
                },
                new MainMenuItemViewModel()
                {
                    Name = "Themes",
                    Icon = PackIconKind.Animation,
                    Page = () => new ThemesPage()
                    {
                        DataContext = new ThemesPageViewModel()
                    }
                },
                new MainMenuItemViewModel()
                {
                    Name = "Templates",
                    Icon = PackIconKind.Apps,
                    Page = () => new TemplatesPage()
                    {
                        DataContext = new TemplatesPageViewModel()
                    }
                },
                new MainMenuItemViewModel()
                {
                    Name = "Generator",
                    Icon = PackIconKind.Flash,
                    Page = () => new GeneratorPage()
                    {
                        DataContext = new GeneratorPageViewModel()
                    }
                },
                new MainMenuItemViewModel()
                {
                    Name = "Settings",
                    Icon = PackIconKind.Settings,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Page = () => new SettingsPage()
                    {
                        DataContext = new SettingsPageViewModel()
                    }
                }
            };

            //Переходим на домашнюю страницу
            CurrentPage = mainMenuItems[0].Page();
            mainMenuItems[0].IsSelected = true;
        }
    }
}
