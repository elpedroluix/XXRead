
using CommunityToolkit.Maui.Behaviors;
using XXRead.Helpers;

namespace XXRead
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            InitializeComponent();

            Application.Current.Resources["ThemeMain"] = Color.FromArgb(AppSettings.ThemeMain);

            Application.Current.Resources["ThemeBackgroundPrimary"] = Color.FromArgb(AppSettings.ThemePrimary);
            Application.Current.Resources["ThemeBackgroundSecondary"] = Color.FromArgb(AppSettings.ThemeSecondary);

            Application.Current.Resources["ThemeFontPrimary"] = Color.FromArgb(AppSettings.ThemeFontPrimary);
            Application.Current.Resources["ThemeFontSecondary"] = Color.FromArgb(AppSettings.ThemeFontSecondary);

            this.InitLabelStoryContent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            Page startPage = AppSettings.FirstRun
                ? _serviceProvider.GetRequiredService<Views.WelcomePage>()
                : _serviceProvider.GetRequiredService<Views.MainPage>();

            return new Window(new Views.MyNavigationPage(startPage));
        }

        private void InitLabelStoryContent()
        {
            Microsoft.Maui.Handlers.LabelHandler.Mapper.AppendToMapping("", (handler, view) =>
            {
                if (view is Helpers.CustomControls.LabelStoryContent)
                {
#if ANDROID
                    handler.PlatformView.JustificationMode = Android.Text.JustificationMode.InterWord;
                    handler.PlatformView.SetTextIsSelectable(true);
                    // JE M'AMUSE
                    var mainColor = Android.Graphics.Color.ParseColor(AppSettings.ThemeMain);
                    handler.PlatformView.SetHighlightColor(Android.Graphics.Color.Argb(128, mainColor.R, mainColor.G, mainColor.B));
                    // JE M'AMUSE
#elif IOS || MACCATALYST
					//handler.PlatformView.EditingDidBegin += (s, e) =>
					//{
					//    handler.PlatformView.PerformSelector(new ObjCRuntime.Selector("selectAll"), null, 0.0f);
					//};
#elif WINDOWS
					handler.PlatformView.GotFocus += (s, e) =>
					{
						//handler.PlatformView.TextAlignment = Windows.UI.Xaml.TextAlignment.Justify;
					};
#endif
                }
            });
        }
    }
}
