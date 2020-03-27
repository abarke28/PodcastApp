using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PodcastApp.ViewModel.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case true:
                    return "Visible";

                case false:
                    return "Hidden";
            }

            return "Hidden";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString())
            {
                case "Visible":
                    return true;

                case "Hidden":
                    return false;

                case "Collapsed":
                    return false;
            }

            return false;
        }
    }
}
