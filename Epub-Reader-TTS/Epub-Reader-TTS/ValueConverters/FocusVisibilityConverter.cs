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
    /// A coverter to decide if the current paragraph is active and the app is reading, to but a border in the currently read paragraph 
    /// </summary>
    public class FocusVisibilityConverter : BaseValueConverter<FocusVisibilityConverter>
    {
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            if (!(bool)value)
                return Visibility.Hidden;

            var host = parameter as PageControl;

            var hostVm = host.DataContext as PageViewModel;

            if(hostVm == null)
                return Visibility.Hidden;


            if (hostVm.IsReading)
                return Visibility.Visible;


            return Visibility.Hidden;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }
}
