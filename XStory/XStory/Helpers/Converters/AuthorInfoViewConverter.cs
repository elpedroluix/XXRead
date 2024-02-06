using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Forms;

namespace XStory.Helpers.Converters
{
	public class AuthorInfoViewConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//DTO.Author author = value as DTO.Author;
			//if (author.Url.Contains("xstory-fr.com"))
			//{
			return new Views.ContentViews.Author.AuthorInfoXStoryView();
			//}
			//else
			//{
			//	return new Views.ContentViews.Author.AuthorInfoHDSView();
			//}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
