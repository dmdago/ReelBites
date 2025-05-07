using ReelBites.ViewModels;

namespace ReelBites.Views
{
    public partial class SettingsPage : ContentPage
    {
        private SettingsViewModel _viewModel;

        public SettingsPage(SettingsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Verificar autenticación
            if (!_viewModel.IsUserLoggedIn())
            {
                Shell.Current.GoToAsync("//login");
                return;
            }

            // Cargar configuraciones
            _viewModel.LoadSettings();
        }
    }
}