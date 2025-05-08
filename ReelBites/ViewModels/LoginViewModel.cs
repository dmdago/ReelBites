using ReelBites.Services;
using System.Text.RegularExpressions;

namespace ReelBites.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        private string _email;
        private string _password;
        private bool _rememberMe;
        private string _errorMessage;
        private bool _hasError;

        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                ValidateInputs();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ValidateInputs();
            }
        }

        public bool RememberMe
        {
            get => _rememberMe;
            set => SetProperty(ref _rememberMe, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public bool CanLogin { get; private set; }

        public Command LoginCommand { get; }
        public Command GoogleLoginCommand { get; }
        public Command FacebookLoginCommand { get; }
        public Command ForgotPasswordCommand { get; }
        public Command RegisterCommand { get; }

        public LoginViewModel(IAuthService authService)
        {
            Title = "Sign In";
            _authService = authService;

            LoginCommand = new Command(async () => await Login(), () => CanLogin);
            GoogleLoginCommand = new Command(async () => await GoogleLogin());
            FacebookLoginCommand = new Command(async () => await FacebookLogin());
            ForgotPasswordCommand = new Command(async () => await ForgotPassword());
            RegisterCommand = new Command(async () => await Register());

            // Inicializar valores
            Email = string.Empty;
            Password = string.Empty;
            RememberMe = false;
            ErrorMessage = string.Empty;
            HasError = false;
            CanLogin = false;
        }

        private void ValidateInputs()
        {
            // Resetear mensaje de error
            HasError = false;
            ErrorMessage = string.Empty;

            // Validar email
            if (!string.IsNullOrWhiteSpace(Email))
            {
                // Verificar formato de email
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(Email, emailPattern))
                {
                    ErrorMessage = "Please enter a valid email address";
                    HasError = true;
                }
            }

            // Verificar que email y contraseña no estén vacíos
            CanLogin = !string.IsNullOrWhiteSpace(Email) &&
                       !string.IsNullOrWhiteSpace(Password) &&
                       !HasError;

            // Actualizar estado del comando
            LoginCommand.ChangeCanExecute();
        }

        async Task Login()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                var success = await _authService.LoginAsync(Email, Password);

                if (success)
                {
                    // Guardar preferencia de recordar usuario si está marcada
                    if (RememberMe)
                    {
                        // Aquí podrías guardar el email en las preferencias
                        // Preferences.Set("saved_email", Email);
                    }

                    // Navegar a la página principal
                    await Shell.Current.GoToAsync("//main");
                }
                else
                {
                    ErrorMessage = "Invalid email or password";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                ErrorMessage = "An error occurred during login. Please try again.";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task GoogleLogin()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            HasError = false;

            try
            {
                // Implementar lógica de autenticación con Google
                // Esto generalmente implica usar un servicio específico para autenticación social
                // Por ejemplo: var success = await _socialAuthService.LoginWithGoogleAsync();

                // Por ahora simulamos un éxito
                bool success = true;

                if (success)
                {
                    await Shell.Current.GoToAsync("//main");
                }
                else
                {
                    ErrorMessage = "Google authentication failed";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Google login error: {ex.Message}");
                ErrorMessage = "An error occurred during Google login";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task FacebookLogin()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            HasError = false;

            try
            {
                // Implementar lógica de autenticación con Facebook
                // Por ahora simulamos un éxito
                bool success = true;

                if (success)
                {
                    await Shell.Current.GoToAsync("//main");
                }
                else
                {
                    ErrorMessage = "Facebook authentication failed";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Facebook login error: {ex.Message}");
                ErrorMessage = "An error occurred during Facebook login";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ForgotPassword()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please enter your email address first";
                HasError = true;
                return;
            }

            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var success = await _authService.ForgotPasswordAsync(Email);

                if (success)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Password Reset",
                        "If your email is registered in our system, you will receive instructions to reset your password.",
                        "OK");
                }
                else
                {
                    // Mantenemos el mismo mensaje incluso si falla para no revelar
                    // si el email existe en nuestro sistema (seguridad)
                    await Application.Current.MainPage.DisplayAlert(
                        "Password Reset",
                        "If your email is registered in our system, you will receive instructions to reset your password.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Forgot password error: {ex.Message}");
                ErrorMessage = "An error occurred. Please try again.";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task Register()
        {
            // Navegar a la página de registro
            await Shell.Current.GoToAsync("register");
        }

        public bool IsUserLoggedIn()
        {
            return _authService.IsAuthenticated();
        }
    }
}