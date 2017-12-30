using System.Collections.ObjectModel;
using System.Numerics;
using System.Windows.Input;
using StockManager.Commands;
using StockManager.Models;
using StockManager.Services;
using System.Text;

namespace StockManager.ViewModels
{
    class GeneratorPageViewModel : ViewModelBase
    {
        public ObservableCollection<Theme> Themes { get; private set; }
            = new ObservableCollection<Theme>();

        public ObservableCollection<Template> Templates { get; private set; }
            = new ObservableCollection<Template>();

        public ObservableCollection<Background> Backgrounds { get; private set; }
            = new ObservableCollection<Background>();

        public Theme Theme { get; set; }
        public Template Template { get; set; }
        public Background Background { get; set; }
        public int Percentage { get; set; }
        public int Maximum { get; set; }
        public bool IsWorking { get; private set; } = SetGenerator.IsWorking;
        public int MatchingIcons { get; private set; }
        public BigInteger Combinations { get; private set; } = SetGenerator.CombinationsCount;
        public BigInteger UsedCombinations { get; private set; } = SetGenerator.ExistingCombinationsCount;
        public string Log { get; private set; }

        public ICommand StartGenerationCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    SetGenerator.SetSettingsAndStart(
                        Theme,
                        Template,
                        Background,
                        Percentage,
                        Maximum
                    );
                });
            }
        }

        public ICommand StopGenerationCommand
        {
            get
            {
                return new RelayCommand(o =>
                {
                    SetGenerator.StopOperations();
                });
            }
        }

        public GeneratorPageViewModel()
        {
            Themes = App.GetRepository<Theme>().SelectAll();
            Templates = App.GetRepository<Template>().SelectAll();
            Backgrounds = App.GetRepository<Background>().SelectAll();

            if (Themes.Count > 0)
                Theme = Themes[0];

            if (Templates.Count > 0)
                Template = Templates[0];

            if (Backgrounds.Count > 0)
                Background = Backgrounds[0];

            if (SetGenerator.Theme != null)
                Theme = SetGenerator.Theme;

            if (SetGenerator.Template != null)
                Template = SetGenerator.Template;

            if (SetGenerator.Background != null)
                Background = SetGenerator.Background;

            MatchingIcons = SetGenerator.MatchingIcons == null
                ? 0
                : SetGenerator.MatchingIcons.Count;

            SetGenerator.SettingsRecalculationCompleted += (sender, e) =>
            {
                MatchingIcons = SetGenerator.MatchingIcons == null
                    ? 0
                    : SetGenerator.MatchingIcons.Count;
                Combinations = SetGenerator.CombinationsCount;
                UsedCombinations = SetGenerator.ExistingCombinationsCount;
            };

            SetGenerator.GenerationStarted += (sender, e) =>
            {
                IsWorking = true;
            };

            SetGenerator.GenerationCompleted += (sender, e) =>
            {
                IsWorking = false;
            };

            SetGenerator.LogChanged += (sender, e) =>
            {
                Log = SetGenerator.ActionLog;
            };

            Log = SetGenerator.ActionLog;
        }
    }
}
