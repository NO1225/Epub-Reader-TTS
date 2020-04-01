using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epub_Reader_TTS
{
    /// <summary>
    /// multiply the input numirical value with the choosen parameter
    /// </summary>
    public class RoundNumberConverter : BaseValueConverter<RoundNumberConverter>
    {
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            var decimils = int.Parse(parameter.ToString());

            var number = double.Parse(value.ToString());

            var outPut = Math.Round(number, decimils);

            return outPut;

        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
