namespace XXRead.Helpers.CustomRenderers
{
    public class LabelJustify : Label
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
