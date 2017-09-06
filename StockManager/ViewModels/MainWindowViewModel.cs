using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StockManager.ViewModels
{
    class MainWindowViewModel
    {
        public MainMenuItem[] MainMenuItems
        {
            get
            {
                return new MainMenuItem[] {
                    new MainMenuItem
                    {
                        Name = "Home",
                        Icon = "Home"
                    },
                    new MainMenuItem
                    {
                        Name = "Icons",
                        Icon = "FileTree"
                    },
                    new MainMenuItem
                    {
                        Name = "Sets",
                        Icon = "PackageVariant"
                    },
                    new MainMenuItem
                    {
                        Name = "Keywords",
                        Icon = "Key"
                    }
                };
            }
        }
    }

    struct MainMenuItem
    {
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}
