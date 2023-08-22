using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace XStory.Helpers.Styles
{
	public class RectangleStyles
	{
		public static Style SeparatorRectangleStyle = new Style(typeof(Rectangle))
		{
			Setters =
			{
				new Setter() { Property = VisualElement.HeightRequestProperty, Value = 1 },
				new Setter() { Property = View.MarginProperty, Value = new Thickness(20) },
				new Setter() { Property = VisualElement.HeightRequestProperty, Value = 1 },
			}
		};
	}
}
