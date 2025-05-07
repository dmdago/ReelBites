using ReelBites.Services;
using System.Text.RegularExpressions;

namespace ReelBites.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        private string _username;
        private string _email;
        private string _password;
        private string _confirmPassword;
        private bool _acceptTerms;

        private string _usernameError;
        private string _emailError;
        private string _passwordError;
        private string _confirmPasswordError;
        private string _termsError;
        private string _errorMessage;

        private bool _hasUsernameError;
        private bool _hasEmailError;
        private bool _hasPasswordError;
        private bool _hasConfirmPasswordError;
        private bool _hasTermsError;
        private bool _hasError;

        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                ValidateUsername();
                UpdateCanRegister();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                ValidateEmail();
                UpdateCanRegister();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ValidatePassword();
                UpdateCanRegister();
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                SetProperty(ref _confirmPassword, value);
                ValidateConfirmPassword();
                UpdateCanRegister();
            }
        }

        public bool AcceptTerms
        {
            get => _acceptTerms;
            set
            {
                SetProperty(ref _acceptTerms, value);
                ValidateTerms();
                UpdateCanRegister();
            }
        }

        public string UsernameError
        {
            get => _usernameError;
            set => SetProperty(ref _usernameError, value);
        }

        public string EmailError
        {
            get => _emailError;
            set => SetProperty(ref _emailError, value);
        }

        public string PasswordError
        {
            get => _passwordError;
            set => SetProperty(ref _passwordError, value);
        }

        public string ConfirmPasswordError
        {
            get => _confirmPasswordError;
            set => SetProperty(ref _confirmPasswordError, value);
        }

        public string TermsError
        {
            get => _termsError;
            set => SetProperty(ref _termsError, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool HasUsernameError
        {
            get => _hasUsernameError;
            set => SetProperty(ref _hasUsernameError, value);
        }

        public bool HasEmailError
        {
            get => _hasEmailError;
            set => SetProperty(ref _hasEmailError, value);
        }

        public bool HasPasswordError
        {
            get => _hasPasswordError;
            set => SetProperty(ref _hasPasswordError, value);
        }

        public bool HasConfirmPasswordError
        {
            get => _hasConfirmPasswordError;
            set => SetProperty(ref _hasConfirmPasswordError, value);
        }

        public bool HasTermsError
        {
            get => _hasTermsError;
            set => SetProperty(ref _hasTermsError, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public bool CanRegister { get; private set; }

        public Command RegisterCommand { get; }
        public Command LoginCommand { get; }
        public Command BackCommand { get; }

        public RegisterViewModel(IAuthService authService)
        {
            Title = "Register";
            _authService = authService;

            RegisterCommand = new Command(async () => await Register(), () => CanRegister);
            LoginCommand = new Command(async () => await GoToLogin());
            BackCommand = new Command(async () => await GoToLogin());

            // Inicializar valores
            Username = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            AcceptTerms = false;

            UsernameError = string.Empty;
            EmailError = string.Empty;
            PasswordError = string.Empty;
            ConfirmPasswordError = string.Empty;
            TermsError = string.Empty;
            ErrorMessage = string.Empty;

            HasUsernameError = false;
            HasEmailError = false;
            HasPasswordError = false;
            HasConfirmPasswordError = false;
            HasTermsError = false;
            HasError = false;

            CanRegister = false;
        }

        private void ValidateUsername()
        {
            HasUsernameError = false;
            UsernameError = string.Empty;

            if (string.IsNullOrWhiteSpace(Username))
            {
                UsernameError = "Username is required";
                HasUsernameError = true;
                return;
            }

            if (Username.Length < 3)
            {
                UsernameError = "Username must be at least 3 characters";
                HasUsernameError = true;
                return;
            }

            if (Username.Length > 20)
            {
                UsernameError = "Username must be less than 20 characters";
                HasUsernameError = true;
                return;
            }

            // Solo permitir letras, números y algunos caracteres especiales
            string usernamePattern = @"^[a-zA-Z0-9._-]+$";
            if (!Regex.IsMatch(Username, usernamePattern))
            {
                UsernameError = "Username can only contain letters, numbers, dots, underscores, and hyphens";
                HasUsernameError = true;
            }
        }

        private void ValidateEmail()
        {
            HasEmailError = false;
            EmailError = string.Empty;

            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailError = "Email is required";
                HasEmailError = true;
                return;
            }

            // Validar formato de email
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(Email, emailPattern))
            {
                EmailError = "Please enter a valid email address";
                HasEmailError = true;
            }
        }

        private void ValidatePassword()
        {
            HasPasswordError = false;
            PasswordError = string.Empty;

            if (string.IsNullOrWhiteSpace(Password))
            {
                PasswordError = "Password is required";
                HasPasswordError = true;
                return;
            }

            if (Password.Length < 8)
            {
                PasswordError = "Password must be at least 8 characters";
                HasPasswordError = true;
                return;
            }

            // Validar complejidad de la contraseña (al menos una letra minúscula, una mayúscula y un número)
            bool hasLower = Password.Any(char.IsLower);
            bool hasUpper = Password.Any(char.IsUpper);
            bool hasDigit = Password.Any(char.IsDigit);

            if (!hasLower || !hasUpper || !hasDigit)
            {
                PasswordError = "Password must contain at least one lowercase letter, one uppercase letter, and one number";
                HasPasswordError = true;
            }

            // Si la contraseña cambia, validar también la confirmación
            if (!string.IsNullOrEmpty(ConfirmPassword))
            {
                ValidateConfirmPassword();
            }
        }

        private void ValidateConfirmPassword()
        {
            HasConfirmPasswordError = false;
            ConfirmPasswordError = string.Empty;

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ConfirmPasswordError = "Please confirm your password";
                HasConfirmPasswordError = true;
                return;
            }

            if (Password != ConfirmPassword)
            {
                ConfirmPasswordError = "Passwords do not match";
                HasConfirmPasswordError = true;
            }
        }

        private void ValidateTerms()
        {
            HasTermsError = false;
            TermsError = string.Empty;

            if (!AcceptTerms)
            {
                TermsError = "You must accept the Terms of Service and Privacy Policy";
                HasTermsError = true;
            }
        }

        private void UpdateCanRegister()
        {
            CanRegister = !string.IsNullOrWhiteSpace(Username) &&
                          !string.IsNullOrWhiteSpace(Email) &&
                          !string.IsNullOrWhiteSpace(Password) &&
                          !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                          AcceptTerms &&
                          !HasUsernameError &&
                          !HasEmailError &&
                          !HasPasswordError &&
                          !HasConfirmPasswordError &&
                          !HasTermsError;

            RegisterCommand.ChangeCanExecute();
        }

        async Task Register()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            // Validar todos los campos una vez más
            ValidateUsername();
            ValidateEmail();
            ValidatePassword();
            ValidateConfirmPassword();
            ValidateTerms();
            UpdateCanRegister();

            if (!CanRegister)
            {
                IsBusy = false;
                return;
            }

            try
            {
                var success = await _authService.RegisterAsync(Email, Username, Password);

                if (success)
                {
                    // Mostrar mensaje de éxito
                    await Application.Current.MainPage.DisplayAlert(
                        "Success",
                        "Your account has been created successfully!",
                        "OK");

                    // Navegar a la página principal
                    await Shell.Current.GoToAsync("//home");
                }
                else
                {
                    ErrorMessage = "Registration failed. Please try again.";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                ErrorMessage = "An error occurred during registration. Please try again.";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task GoToLogin()
        {
            await Shell.Current.GoToAsync("//login");
        }

        public bool IsUserLoggedIn()
        {
            return _authService.IsAuthenticated();
        }
    }
}