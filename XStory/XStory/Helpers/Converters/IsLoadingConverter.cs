using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XStory.Helpers.Converters
{
    public class IsLoadingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // si value == false et parameter == null
            string loadingParameter = parameter as string;

            if((bool)value == true)
            {
                if (!string.IsNullOrWhiteSpace(loadingParameter))
                {
                    return true;
                }
                return false;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(loadingParameter))
                {
                    return false;
                }
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
