using Prism.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace XStory.Helpers.CustomRenderers
{
    public class LabelJustify : Xamarin.Forms.Label
    {
        public static readonly BindableProperty JustifyTextProperty =
            BindableProperty.Create("JustifyText", typeof(bool), typeof(Label), false);

        public bool JustifyText
        {
            get { return (bool)GetValue(JustifyTextProperty); }
            set { SetValue(JustifyTextProperty, value); }
        }
    }
}
