using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using StockManager.Commands;
using StockManager.Models;
using StockManager.Services;
using SWM = System.Windows.Media;

namespace StockManager.ViewModels {
    class BackgroundPageViewModel : ViewModelBase {

        private Context context = new Context();

        public ObservableCollection<Background> BackgroundList { get; private set; }
        public ObservableCollection<SelectableListBoxItem<Template>> TemplateList { get; private set; }
        public ObservableCollection<SelectableListBoxItem<Color>> ColorList { get; private set; }

        public SWM.Color? NewColor { get; set; }

        public ICommand AddColorCommand {
            get {
                return new RelayCommand(
                    o => {
                        if (NewColor.HasValue) {
                            var hex = NewColor.Value.ToString().Substring(3, 6);

                            var color = context.Colors
                                .Where(c => c.HEX == hex)
                                .SingleOrDefault();

                            if (color == null) {
                                color = new Color {
                                    HEX = hex,
                                };

                                if (BackgroundInfo != null) {
                                    color.Backgrounds.Add(
                                        BackgroundInfo.Background
                                    );
                                    context.Colors.Add(color);
                                    context.SaveChanges();
                                    RefreshColorList();
                                }
                            }
                            else {
                                if (BackgroundInfo != null) {
                                    color.Backgrounds.Add(BackgroundInfo.Background);
                                    context.SaveChanges();
                                    RefreshColorList();
                                }
                            }
                        }
                    }
                );
            }
        }

        public ICommand ShowInfoCommand {
            get {
                return new RelayCommand(
                    o => {
                        if (o is Background b) {
                            BackgroundInfo = new BackgroundViewModel(b);
                            foreach (var item in TemplateList) {
                                item.IsSelected
                                    = b.Templates.Any(t => t.Id == item.Item.Id);
                            }

                            foreach (var item in ColorList) {
                                item.IsSelected
                                    = b.Colors.Any(t => t.Id == item.Item.Id);
                            }
                        }
                    }
                );
            }
        }

        public ICommand BindTemplateCommand {
            get {
                return new RelayCommand(
                    o => {
                        if (o is SelectableListBoxItem<Template> t) {
                            if (BackgroundInfo == null || BackgroundInfo.Background == null)
                                return;

                            if (t.IsSelected) {
                                BackgroundInfo.Background.Templates.Remove(t.Item);
                                context.SaveChanges();
                                t.IsSelected = false;
                            }
                            else {
                                BackgroundInfo.Background.Templates.Add(t.Item);
                                context.SaveChanges();
                                t.IsSelected = true;
                            }
                        }
                    }
                );
            }
        }

        public ICommand BindColorCommand {
            get {
                return new RelayCommand(
                    o => {
                        if (o is SelectableListBoxItem<Color> c) {
                            if (BackgroundInfo == null || BackgroundInfo.Background == null)
                                return;

                            if (c.IsSelected) {
                                BackgroundInfo.Background.Colors.Remove(c.Item);
                                context.SaveChanges();
                                c.IsSelected = false;
                            }
                            else {
                                BackgroundInfo.Background.Colors.Add(c.Item);
                                context.SaveChanges();
                                c.IsSelected = true;
                            }
                        }
                    }
                );
            }
        }

        public BackgroundViewModel BackgroundInfo { get; private set; }

        private void RefreshBackroundList() {
            BackgroundList = new ObservableCollection<Background>(
                context.Backgrounds
                    .Where(b => !b.IsDeleted)
                    .ToList()
            );
        }

        public void RefreshColorList() {
            ColorList = new ObservableCollection<SelectableListBoxItem<Color>>(
                context
                .Colors
                .OrderBy(c => c.HEX)
                .AsEnumerable()
                .Select(c => new SelectableListBoxItem<Color>(
                    c,
                    BackgroundInfo != null && BackgroundInfo.Background != null
                        ? BackgroundInfo.Background
                            .Colors
                            .Any(bc => bc.HEX == c.HEX)
                        : false
                ))
            );
        }

        public BackgroundPageViewModel() {
            RefreshBackroundList();
            TemplateList = new ObservableCollection<SelectableListBoxItem<Template>>(
                context
                .Templates
                .AsEnumerable()
                .Select(t => new SelectableListBoxItem<Template>(t))
            );
            RefreshColorList();
        }
    }
}
