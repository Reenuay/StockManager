using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockManager.Models;
using System.Collections.ObjectModel;
using StockManager.Properties;

namespace StockManager.Utilities
{
    static class NameGenerator
    {
        private static Random dice = new Random();

        public static string GenerateName(Composition composition)
        {
            var tepmlateString
                = Settings.Default.NameTemplates[
                    dice.Next(Settings.Default.NameTemplates.Count)
                ];

            return "";
        }
    }
}
