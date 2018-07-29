using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockManager.ViewModels {
    abstract class SingletonViewModelBase<ViewModelT>
        : ViewModelBase where ViewModelT: ViewModelBase {
        private static readonly Lazy<ViewModelT> singleton
            = new Lazy<ViewModelT>();

        public static ViewModelT Singleton {
            get {
                singleton.Value.Refresh();
                return singleton.Value;
            }
        }
    }
}
