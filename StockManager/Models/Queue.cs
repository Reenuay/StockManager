using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace StockManager.Models
{
    public class Queue : Changeable
    {
        public ObservableCollection<Task> Tasks { get; private set; }
            = new ObservableCollection<Task>();
    }
}
