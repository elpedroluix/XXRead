namespace XXRead.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupSelectCategoryPage : CommunityToolkit.Maui.Views.Popup
    {
        public PopupSelectCategoryPage(ViewModels.PopupViewModels.PopupSelectCategoryPageViewModel viewModel)
        {
            InitializeComponent();

            this.BindingContext = viewModel;
        }
    }
}
