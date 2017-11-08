using NLog;
using PropertyChanged;

namespace StockManager.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    abstract class ViewModelBase
    {
        private Logger logger;

        public Logger Logger
        {
            get
            {
                return logger
                    ?? (logger = LogManager.GetLogger(GetType().Name));
            }
        }
    }
}
