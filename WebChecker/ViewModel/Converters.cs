using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace WebChecker.ViewModel
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var converter = new BrushConverter();
            string status = (string)value;
            switch (status)
            {
                case "OK":
                    return Brushes.Green;
                case "Checking":
                    return Brushes.Black;
                default:
                    return Brushes.Red;
            }
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SMTPConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string address = (string)values[0];
            SecureString password = Model.Secure.stringToSecureString((string)values[1]);
            string smtpServer = (string)values[2];
            int smtpPort = Int32.Parse((string)values[3]);
            bool enableSsl = (bool)values[4];

            if (!(address == null) && !((string)values[1] ==null) && !(smtpServer == null) && smtpPort > 0)
            {
                return new Model.Notify(address, password, smtpPort, enableSsl, smtpServer);
            }
            else return new Model.Notify("empty", new SecureString(), 0, false, "0");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class WindowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isWindowClosed = (bool)value;
            switch (isWindowClosed)
            {
                case true:
                    return false;
                case false:
                    return true;
                default:
                    return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
