using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using StockManager.Commands;
using StockManager.Properties;

namespace StockManager.ViewModels {
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

        public int SetsCount {
            get {
                return Settings.Default.AfterEachNSets;
            }

            set {
                if (value < 1)
                    return;

                Settings.Default.AfterEachNSets = value;
                Settings.Default.Save();
            }
        }

        public int WaitForIllustrator {
            get {
                return Settings.Default.WaitForIllustrator;
            }

            set {
                if (value < 1000)
                    return;

                Settings.Default.WaitForIllustrator = value;
                Settings.Default.Save();
            }
        }

        public int WaitForIllustratorSavesFile {
            get {
                return Settings.Default.WaitForIllustratorSavesFile;
            }

            set {
                if (value < 1000)
                    return;

                Settings.Default.WaitForIllustratorSavesFile = value;
                Settings.Default.Save();
            }
        }

        public int WaitForFileTriesNumber {
            get {
                return Settings.Default.WaitForFileTriesNumber;
            }

            set {
                if (value < 1)
                    return;

                Settings.Default.WaitForFileTriesNumber = value;
                Settings.Default.Save();
            }
        }

        public int WaitForFileInterval {
            get {
                return Settings.Default.WaitForFileInterval;
            }

            set {
                if (value < 100)
                    return;

                Settings.Default.WaitForFileInterval = value;
                Settings.Default.Save();
            }
        }

        public string IllustratorPath {
            get {
                return Settings.Default.IllustratorPath;
            }

            set {
                Settings.Default.IllustratorPath = value;
                Settings.Default.Save();
            }
        }

        public int DocumentSize {
            get {
                return Settings.Default.DocumentSize;
            }

            set {
                if (value < 100)
                    return;

                Settings.Default.DocumentSize = value;
                Settings.Default.Save();
            }
        }

        public int JpegSize {
            get {
                return Settings.Default.JpegSize;
            }

            set {
                if (value < 2000)
                    return;

                Settings.Default.JpegSize = value;
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
