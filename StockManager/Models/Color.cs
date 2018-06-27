using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SWM = System.Windows.Media;

namespace StockManager.Models {
    public class Color : Identity {
        [Required, MinLength(6), MaxLength(6), Index(IsUnique =true)]
        public string HEX { get; set; }

        public SWM.Color AsColor {
            get {
                return (SWM.Color)SWM.ColorConverter.ConvertFromString("#" + HEX);
            }
        }

        public SWM.Brush AsBrush {
            get {
                return new SWM.SolidColorBrush(AsColor);
            }
        }

        public virtual ObservableCollection<Background> Backgrounds { get; set; }
            = new ObservableCollection<Background>();
    }
}
