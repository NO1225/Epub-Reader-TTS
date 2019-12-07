using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// Convert the <see cref="ApplicationPage"/> to an actual view/Page
    /// </summary>
    public class MultiplyConverter : BaseValueConverter<MultiplyConverter>
    {
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            var multiplyer = double.Parse(parameter.ToString());

            var result = double.Parse(value.ToString());

            return result * multiplyer;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
