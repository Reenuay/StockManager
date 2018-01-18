using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using StockManager.Commands;
using StockManager.Properties;

namespace StockManager.ViewModels
{
    class SettingsPageViewModel : ViewModelBase
    {
        #region NameTemplates

        public ObservableCollection<string> NameTemplates { get; private set; }

        public string NewNameTemplate { get; set; }

        public string NameTemplatesToolTip
        {
            get
            {
                return Settings.Default.NameTemplatesInfo;
            }
        }

        public ICommand DeleteNameTemplateCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (o is string s)
                    {
                        Settings.Default.NameTemplates.Remove(s);
                        Settings.Default.Save();
                        RefreshNameTemplates();
                    }
                });
            }
        }

        public ICommand AddNameTemplateCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    if (!string.IsNullOrEmpty(NewNameTemplate) && !string.IsNullOrWhiteSpace(NewNameTemplate))
                    {
                        Settings.Default.NameTemplates.Add(NewNameTemplate);
                        Settings.Default.Save();
                        RefreshNameTemplates();
                        NewNameTemplate = "";
                    }
                });
            }
        }

        private void RefreshNameTemplates()
        {
            NameTemplates = new ObservableCollection<string>(
                Settings.Default.NameTemplates.Cast<string>()
            );
        }

        #endregion

        #region Restart

        public bool Restart
        {
            get
            {
                return Settings.Default.RestartIllustrator;
            }

            set
            {
                Settings.Default.RestartIllustrator = value;
                Settings.Default.Save();
            }
        }

        public int SetsCount
        {
            get
            {
                return Settings.Default.AfterEachNSets;
            }

            set
            {
                if (value < 1)
                    return;

                Settings.Default.AfterEachNSets = value;
                Settings.Default.Save();
            }
        }

        #endregion

        public SettingsPageViewModel()
        {
            RefreshNameTemplates();
        }
    }
}
