using StockManager.Commands;
using StockManager.Models;
using StockManager.Repositories;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace StockManager.ViewModels
{
    class ThemesPageViewModel : ViewModelBase
    {
        #region Properties

        #region ThemesList

        public ObservableCollection<Theme> ThemesList { get; private set; }

        #endregion

        public ICommand SelectThemeCommand
        {
            get
            {
                return new RelayCommand(
                    o =>
                    {
                        if (o is Theme t)
                        {
                            SelectedTheme = t;
                            SelectedName = t.Name;
                            SelectedKeywords
                                = new ObservableCollection<string>(
                                    t.Keywords.Select(k => k.Name)
                                );
                        }
                    },
                    o => o is Theme
                );
            }
        }

        public ICommand DeselectThemeCommand
        {
            get
            {
                return new RelayCommand(
                    o =>
                    {
                        SelectedTheme = null;
                        SelectedName = "";
                        SelectedKeywords = new ObservableCollection<string>();
                    }
                );
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(
                    o =>
                    {
                        Repository<Theme> tRepo = App.GetRepository<Theme>();
                        Repository<Keyword> kRepo = App.GetRepository<Keyword>();

                        var keywords = kRepo.Select(
                            k => SelectedKeywords.Contains(k.Name)
                        );

                        kRepo.ExecuteTransaction(() =>
                        {
                            foreach (var keywordName in SelectedKeywords)
                            {
                                if (!keywords.Any(k => k.Name == keywordName))
                                {
                                    var keyword = new Keyword
                                    {
                                        Name = keywordName
                                    };

                                    kRepo.Insert(keyword);
                                    keywords.Add(keyword);
                                }
                            }
                        });

                        SelectedName = string.Join(" ",
                            SelectedName
                            .Split(' ')
                            .Select(s => {
                                return s.First().ToString().ToUpper()
                                    + s.Substring(1).ToLower();
                                }
                            )
                        );

                        if (SelectedTheme == null)
                        {
                            if (tRepo.Select(t => t.Name == SelectedName).Count > 0)
                            {
                                return;
                            }

                            var theme = new Theme
                            {
                                Name = SelectedName,
                                Keywords = keywords
                            };

                            tRepo.Insert(theme);
                            ThemesList = tRepo.SelectAll();
                        }
                        else
                        {
                            if (SelectedTheme.Name != SelectedName)
                            {
                                if (tRepo.Select(t => t.Name == SelectedName).Count > 0)
                                {
                                    return;
                                }
                            }

                            SelectedTheme.Name = SelectedName;
                            SelectedTheme.Keywords = keywords;
                            tRepo.Update(SelectedTheme);
                        }
                    }
                );
            }
        }

        public ICommand AddKeywordCommand
        {
            get
            {
                return new RelayCommand(
                    o =>
                    {
                        if (!string.IsNullOrEmpty(NewKeywordName))
                        {
                            var name = NewKeywordName.ToLower();
                            if (!SelectedKeywords.Contains(name))
                            {
                                SelectedKeywords.Add(name);
                                NewKeywordName = "";
                            }
                        }
                    }
                );
            }
        }

        public ICommand DeleteKeywordCommand
        {
            get
            {
                return new RelayCommand(
                    o =>
                    {
                        if (o is string s)
                        {
                            SelectedKeywords.Remove(s);
                        }
                    }
                );
            }
        }

        public ICommand RemoveThemeCommand
        {
            get
            {
                return new RelayCommand(
                    o =>
                    {
                        if (o is Theme t)
                        {
                            var repo = App.GetRepository<Theme>();
                            repo.Delete(t);
                            ThemesList.Remove(t);
                        }
                    },
                    o => o is Theme
                );
            }
        }

        public Theme SelectedTheme { get; set; }

        public string SelectedName { get; set; }
        public ObservableCollection<string> SelectedKeywords { get; set; }
            = new ObservableCollection<string>();

        public string NewKeywordName { get; set; }

        #endregion

        public ThemesPageViewModel()
        {
            ThemesList = new ObservableCollection<Theme>(
                App.GetRepository<Theme>().SelectAll()
            );
        }
    }
}
