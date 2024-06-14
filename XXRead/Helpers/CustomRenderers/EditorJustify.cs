namespace XXRead.Helpers.CustomRenderers
{
	public class EditorJustify : Editor
	{
		public static readonly BindableProperty JustifyTextProperty =
			BindableProperty.Create("JustifyText", typeof(bool), typeof(Editor), false);

		public bool JustifyText
		{
			get { return (bool)GetValue(JustifyTextProperty); }
			set { SetValue(JustifyTextProperty, value); }
		}
	}
}
