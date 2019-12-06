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
    public class BookPageConverter : BaseValueConverter<BookPageConverter>
    {
        public static PagePage CashedPage = new PagePage();
        public override object Convert(object value, Type targetType = null, object parameter = null, CultureInfo culture = null)
        {
            // TODO: Decide wiether to cash or not
            var pageViewModel = value as PageViewModel;
            if (pageViewModel != null)
                return new PageControl();



            //switch (addContent)
            //{
            //    case AdditionalContent.EditReceipt:
            //        return new EditReceiptControl();
            //    case AdditionalContent.ReceiptDetails:
            //        return new ReceiptDetailsControl();
            //    case AdditionalContent.EditComplaint:
            //        return new EditComplaintControl();
            //    case AdditionalContent.ComplaintDetails:
            //        return new ComplaintDetailsControl();
            //    default:
            //        break;
            //}

            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
