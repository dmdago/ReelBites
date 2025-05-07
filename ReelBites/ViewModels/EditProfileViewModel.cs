using ReelBites.Data;
using ReelBites.Models;
using ReelBites.Services;
using System.Text.RegularExpressions;

namespace ReelBites.ViewModels
{
    public class EditProfileViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IPreferencesService _preferencesService;

        private User _originalUser;
        private string _profileImage;
        private string _username;
        private string _fullName;
        private string _bio;
        private string _email;
        private bool _darkModeEnabled;
        private bool _notificationsEnabled;
        private bool _privateAccount;

        private string _usernameError;
        private bool _hasUsernameError;
        private string _errorMessage;
        private bool _hasError;

        public bool HasUnsavedChanges { get; private set; }

        public string ProfileImage
        {
            get => _profileImage;
            set
            {
                if (SetProperty(ref _profileImage, value))
                {
                    HasUnsavedChanges = true;
                }
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    ValidateUsername();
                    HasUnsavedChanges = true;
                }
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                if (SetProperty(ref _fullName, value))
                {
                    HasUnsavedChanges = true;
                }
            }
        }

        public string Bio
        {
            get => _bio;
            set
            {
                if (SetProperty(ref _bio, value))
                {
                    // Limitar bio a 150 caracteres
                    if (!string.IsNullOrEmpty(_bio) && _bio.Length > 150)
                    {
                        _bio = _bio.Substring(0, 150);
                    }

                    OnPropertyChanged(nameof(CharCount));
                    HasUnsavedChanges = true;
                }
            }
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public bool DarkModeEnabled
        {
            get => _darkModeEnabled;
            set
            {
                if (SetProperty(ref _darkModeEnabled, value))
                {
                    _preferencesService.SetDarkMode(value);
                    HasUnsavedChanges = true;

                    // Actualizar tema de la aplicación
                    ApplyTheme();
                }
            }
        }

        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set
            {
                if (SetProperty(ref _notificationsEnabled, value))
                {
                    HasUnsavedChanges = true;
                }
            }
        }

        public bool PrivateAccount
        {
            get => _privateAccount;
            set
            {
                if (SetProperty(ref _privateAccount, value))
                {
                    HasUnsavedChanges = true;
                }
            }
        }

        public string UsernameError
        {
            get => _usernameError;
            set => SetProperty(ref _usernameError, value);
        }

        public bool HasUsernameError
        {
            get => _hasUsernameError;
            set => SetProperty(ref _hasUsernameError, value);
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

        public int CharCount => Bio?.Length ?? 0;

        public Command SaveCommand { get; }
        public Command ChangePhotoCommand { get; }
        public Command ChangePasswordCommand { get; }
        public Command LogoutCommand { get; }
        public Command DeleteAccountCommand { get; }

        public EditProfileViewModel(IUserService userService, IAuthService authService, IPreferencesService preferencesService)
        {
            Title = "Edit Profile";
            _userService = userService;
            _authService = authService;
            _preferencesService = preferencesService;

            SaveCommand = new Command(async () => await SaveProfile(), () => CanSave());
            ChangePhotoCommand = new Command(async () => await ChangePhoto());
            ChangePasswordCommand = new Command(async () => await ChangePassword());
            LogoutCommand = new Command(async () => await Logout());
            DeleteAccountCommand = new Command(async () => await DeleteAccount());

            // Inicializar
            DarkModeEnabled = _preferencesService.IsDarkMode();
            HasUnsavedChanges = false;
        }

        public async Task LoadProfileAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                _originalUser = await _userService.GetCurrentUserAsync();

                if (_originalUser != null)
                {
                    ProfileImage = _originalUser.ProfilePictureUrl;
                    Username = _originalUser.Username;
                    FullName = _originalUser.FullName;
                    Bio = _originalUser.Bio;
                    Email = _originalUser.Email;
                    NotificationsEnabled = _originalUser.NotificationsEnabled;
                    PrivateAccount = _originalUser.IsPrivate;

                    // Restablecer estado
                    HasUnsavedChanges = false;
                }
                else
                {
                    ErrorMessage = "Failed to load profile information.";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading profile: {ex.Message}");
                ErrorMessage = "An error occurred while loading your profile.";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
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

            SaveCommand.ChangeCanExecute();
        }

        private bool CanSave()
        {
            return HasUnsavedChanges && !HasUsernameError && !string.IsNullOrWhiteSpace(Username);
        }

        private async Task SaveProfile()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                // Actualizar modelo de usuario
                var updatedUser = new User
                {
                    Id = _originalUser.Id,
                    Username = Username,
                    FullName = FullName,
                    Bio = Bio,
                    Email = Email,
                    ProfilePictureUrl = ProfileImage,
                    NotificationsEnabled = NotificationsEnabled,
                    IsPrivate = PrivateAccount
                };

                bool success = await _userService.UpdateUserProfileAsync(updatedUser);

                if (success)
                {
                    // Actualizar usuario original con los nuevos valores
                    _originalUser = updatedUser;
                    HasUnsavedChanges = false;

                    await Application.Current.MainPage.DisplayAlert(
                        "Success",
                        "Your profile has been updated successfully!",
                        "OK");
                }
                else
                {
                    ErrorMessage = "Failed to update profile. Please try again.";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving profile: {ex.Message}");
                ErrorMessage = "An error occurred while saving your profile.";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ChangePhoto()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select Profile Picture"
                });

                if (result != null)
                {
                    // Subir imagen y obtener URL (simulado)
                    await Task.Delay(1000); // Simulando carga

                    // En una implementación real, aquí subirías la imagen a un servidor
                    // y recibirías la URL. Por ahora, simulamos una URL
                    ProfileImage = "https://api.reelbites.com/profiles/avatar123.jpg";
                    HasUnsavedChanges = true;
                    SaveCommand.ChangeCanExecute();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error picking photo: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    "Could not access the photo gallery. Please check app permissions.",
                    "OK");
            }
        }

        private async Task ChangePassword()
        {
            // Navegar a la página de cambio de contraseña
            await Shell.Current.GoToAsync("changepassword");
        }

        private async Task Logout()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Logout",
                "Are you sure you want to log out?",
                "Yes", "No");

            if (confirm)
            {
                await _authService.LogoutAsync();
                await Shell.Current.GoToAsync("//login");
            }
        }

        private async Task DeleteAccount()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Delete Account",
                "Are you sure you want to delete your account? This action cannot be undone.",
                "Delete", "Cancel");

            if (confirm)
            {
                // Solicitar contraseña para confirmar
                string password = await Application.Current.MainPage.DisplayPromptAsync(
                    "Confirm Password",
                    "Please enter your password to confirm account deletion",
                    "Delete", "Cancel",
                    placeholder: "Password",
                    maxLength: 50,
                    keyboard: Keyboard.Default,
                    isPassword: true);

                if (!string.IsNullOrEmpty(password))
                {
                    // Aquí iría la lógica para eliminar la cuenta
                    // Por ahora simulamos éxito
                    await Task.Delay(1000);

                    await _authService.LogoutAsync();
                    await Shell.Current.GoToAsync("//login");

                    await Application.Current.MainPage.DisplayAlert(
                        "Account Deleted",
                        "Your account has been successfully deleted.",
                        "OK");
                }
            }
        }

        private void ApplyTheme()
        {
            // Cambiar tema de la aplicación
            Application.Current.UserAppTheme = DarkModeEnabled
                ? AppTheme.Dark
                : AppTheme.Light;
        }

        public bool IsUserLoggedIn()
        {
            return _authService.IsAuthenticated();
        }
    }
}