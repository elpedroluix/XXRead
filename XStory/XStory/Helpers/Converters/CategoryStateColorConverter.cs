using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace XStory.Helpers.Converters
{
    /// <summary>
    /// <br>Returns the Color of the Category icon regarding its state (Enabled/Disabled).</br>
    /// <br>If enabled : Main Color</br>
    /// <br>If disabled : Gray Color</br>
    /// </summary>
    public class CategoryStateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Color.FromHex(AppSettings.ThemeMain);
            }
            else
            {
                return Color.FromHex("#5E5E5E");
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
