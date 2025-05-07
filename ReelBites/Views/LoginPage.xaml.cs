using ReelBites.ViewModels;

namespace ReelBites.Views
{
    public partial class LoginPage : ContentPage
    {
        private LoginViewModel _viewModel;

        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Si ya hay una sesión activa, redirigir a la página principal
            if (_viewModel.IsUserLoggedIn())
            {
                Shell.Current.GoToAsync("//home");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Previene que el usuario retroceda desde la página de login
            // Esto es útil cuando redirigimos al usuario a login desde páginas
            // que requieren autenticación
            return true;
        }
    }
}