using StockManager.Commands;
using StockManager.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace StockManager.ViewModels
{
    class TemplatesPageViewModel : ViewModelBase
    {
        public bool IsListView { get; set; } = true;

        public ICommand ChangeViewCommand
        {
            get
            {
                return new RelayCommand(
                    o =>
                    {
                        IsListView = !IsListView;
                    }
                );
            }
        }

        #region ListView

        public Template SelectedTemplate { get; set; }

        public ICommand SelectTemplateCommand
        {
            get
            {
                return new RelayCommand(
                    o =>
                    {
                        if (o is Template t)
                        {
                            SelectedTemplate = t;
                        }
                    },
                    o => o is Template
                );
            }
        }

        public ObservableCollection<Template> Templates { get; set; }
            = new ObservableCollection<Template>();

        #endregion

        public ObservableCollection<Cell> Cells { get; set; }
            = new ObservableCollection<Cell>();

        private int columns;
        private int rows = 2;
        private double padding = 10;
        private double margin = 10;

        public int Columns
        {
            get
            {
                return columns;
            }
            set
            {
                columns = value;
                RearrangeTemplate();
            }
        }

        public int Rows
        {
            get
            {
                return rows;
            }
            set
            {
                rows = value;
                RearrangeTemplate();
            }
        }

        public double Padding
        {
            get
            {
                return padding;
            }
            set
            {
                padding = value;
                RearrangeTemplate();
            }
        }

        public double Margin
        {
            get
            {
                return margin;
            }
            set
            {
                margin = value;
                RearrangeTemplate();
            }
        }

        public string NewName { get; set; }

        public ICommand CreateNewTemplateCommand
        {
            get
            {
                return new RelayCommand(
                    o =>
                    {
                        if (string.IsNullOrEmpty(NewName))
                            return;

                        var template = new Template
                        {
                            Name = NewName,
                        };

                        App.GetRepository<Template>().Insert(template);

                        var repo = App.GetRepository<Cell>();

                        repo.ExecuteTransaction(() =>
                        {
                            foreach (var cell in Cells)
                            {
                                cell.TemplateId = template.Id;
                                repo.Insert(cell);
                            }
                        });

                        Columns = Columns;
                        Templates = App.GetRepository<Template>().SelectAll();
                        NewName = "";
                        IsListView = true;
                    }
                );
            }
        }

        private void RearrangeTemplate()
        {
            double size;
            double hpad = 0.0,
                   vpad = 0.0;

            if (Columns >= Rows)
            {
                size = (
                    100.0
                    - (Padding * 2)
                    - (Margin * (Columns - 1))
                )
                / Columns;

                vpad = (
                    100.0
                    - (Padding * 2)
                    - (size * Rows)
                    - (Margin * (Rows - 1))
                )
                / 2;
            }
            else
            {
                size = (
                    100.0
                    - (Padding * 2)
                    - (Margin * (Rows - 1))
                )
                / Rows;

                hpad = (
                    100.0
                    - (Padding * 2)
                    - (size * Columns)
                    - (Margin * (Columns - 1))
                )
                / 2;
            }

            if (size <= 0)
                return;

            Cells = new ObservableCollection<Cell>();

            for (var i = 0; i < Columns; i++)
            {
                for (var j = 0; j < Rows; j++)
                {
                    Cells.Add(new Cell
                    {
                        X = (float)(Padding + (Margin + size) * i + hpad),
                        Y = (float)(Padding + (Margin + size) * j + vpad),
                        Width = (float)size,
                        Height = (float)size
                    });
                }
            }
        }

        public TemplatesPageViewModel()
        {
            var repo = App.GetRepository<Template>();

            Templates = repo.SelectAll();
            Columns = 2;
        }
    }
}
