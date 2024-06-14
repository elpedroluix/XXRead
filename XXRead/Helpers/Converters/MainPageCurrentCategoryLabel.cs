using System.Globalization;

namespace XXRead.Helpers.Converters
{
    public class MainPageCurrentCategoryLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var category = value as XStory.DTO.Category;
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
