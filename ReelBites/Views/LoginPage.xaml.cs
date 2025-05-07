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

            // Si ya hay una sesi�n activa, redirigir a la p�gina principal
            if (_viewModel.IsUserLoggedIn())
            {
                Shell.Current.GoToAsync("//home");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Previene que el usuario retroceda desde la p�gina de login
            // Esto es �til cuando redirigimos al usuario a login desde p�ginas
            // que requieren autenticaci�n
            return true;
        }
    }
}