using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace XStory.Helpers.CustomRenderers
{
	public class EditorJustify : Xamarin.Forms.Editor
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
