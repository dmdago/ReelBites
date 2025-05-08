using ReelBites.Services;
using ReelBites.ViewModels;
using ReelBites.Views;

namespace ReelBites
{
    public partial class App : Application
    {
        private readonly IAuthService _authService;

        public App(IAuthService authService = null)
        {
            InitializeComponent();
            _authService = authService;

            UserAppTheme = AppTheme.Dark;

            MainPage = new AppShell();
            // Navegar a login al inicio de la app sin depender de authService
            Dispatcher.Dispatch(async () =>
            {
                if (_authService.IsAuthenticated())
                {
                    // Si ya está autenticado, navegar a la ruta principal con TabBar
                    await Shell.Current.GoToAsync("//main");
                }
                else
                {
                    // Si no está autenticado, navegar a login
                    await Shell.Current.GoToAsync("//login");
                }
            });
        }
    }
}