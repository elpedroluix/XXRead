using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XStory.Helpers.Converters
{
    public class MainPageCurrentCategoryLabel : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var category = value as DTO.Category;
            if (category != null)
            {
                return category.Title;
            }
            else
            {
                return "Toutes";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
