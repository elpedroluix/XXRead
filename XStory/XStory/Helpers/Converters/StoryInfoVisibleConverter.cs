using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XStory.Helpers.Converters
{
    public class StoryInfoVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isVisible = (bool)value;
            if (isVisible)
            {
                return Constants.StoryPageConstants.STORYPAGE_LESSINFO;
            }
            else
            {
                return Constants.StoryPageConstants.STORYPAGE_MOREINFO;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
