using Microsoft.Maui.Controls.Shapes;

namespace XXRead.Helpers.Styles
{
	public class ShapeStyles
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

  //      public static Style MainFrameStyle = new Style(typeof(Frame))
		//{
		//	Setters =
		//	{
		//		new Setter() { Property = Frame.HasShadowProperty, Value = true },
		//		new Setter() { Property = Frame.CornerRadiusProperty, Value = 3 },
		//		new Setter() { Property = Layout.PaddingProperty, Value = new Thickness(5) },
		//		new Setter() { Property = View.MarginProperty, Value = new Thickness(13) },
		//		new Setter() { Property = Frame.BorderColorProperty, Value = Colors.Transparent },
		//		new Setter() { Property = Frame.BackgroundColorProperty, Value = Colors.Transparent },
		//	}
		//};

		public static Style MainBorderStyle = new Style(typeof(Border))
		{
			Setters =
			{
				new Setter() {Property = Border.ShadowProperty, Value = new Thickness(0) },
				new Setter() {Property = Border.StrokeProperty, Value = Colors.Transparent },
				new Setter() {Property = Border.BackgroundColorProperty, Value = Colors.Transparent },
				new Setter() {Property = Border.StrokeShapeProperty, Value = new RoundRectangle{ CornerRadius = 3 } },
				new Setter() {Property = Layout.PaddingProperty, Value = new Thickness(5) },
				new Setter() {Property = View.MarginProperty, Value = new Thickness(13) },
			}
		};
	}
}
