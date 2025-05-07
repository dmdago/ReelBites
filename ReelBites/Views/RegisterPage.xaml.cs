using ReelBites.ViewModels;

namespace ReelBites.Views
{
    public partial class RegisterPage : ContentPage
    {
        private RegisterViewModel _viewModel;

        public RegisterPage(RegisterViewModel viewModel)
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
            // Permitir el bot�n de retroceso para volver a login
            Shell.Current.GoToAsync("//login");
            return true;
        }
    }
}