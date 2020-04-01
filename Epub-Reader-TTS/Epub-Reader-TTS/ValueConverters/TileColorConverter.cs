using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// multiply the input numirical value with the choosen parameter
    /// </summary>
    public class TileColorConverter : BaseValueConverter<TileColorConverter>
    {
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            if ((bool)value)
                return Application.Current.Resources["AccentColor2Brush"];

            return Application.Current.Resources["PrimaryBackGroundColorBrush"];

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}
