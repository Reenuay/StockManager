using System.ComponentModel;
using NLog;

namespace StockManager.ViewModels {
    abstract class ViewModelBase : INotifyPropertyChanged {
        private Logger logger;

        public Logger Logger {
            get {
                return logger
                    ?? (logger = LogManager.GetLogger(GetType().Name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void Refresh() { }
    }
}
