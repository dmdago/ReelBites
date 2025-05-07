using ReelBites.Services;
using ReelBites.Data;
using System.Reflection;

namespace ReelBites.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly IPreferencesService _preferencesService;
        private readonly IAuthService _authService;

        private bool _darkModeEnabled;
        private bool _autoplayEnabled;
        private string _selectedLanguage;
        private string _appVersion;

        public bool DarkModeEnabled
        {
            get => _darkModeEnabled;
            set
            {
                if (SetProperty(ref _darkModeEnabled, value))
                {
                    _preferencesService.SetDarkMode(value);
                    ApplyTheme();
                }
            }
        }

        public bool AutoplayEnabled
        {
            get => _autoplayEnabled;
            set
            {
                if (SetProperty(ref _autoplayEnabled, value))
                {
                    _preferencesService.SetAutoplay(value);
                }
            }
        }

        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set => SetProperty(ref _selectedLanguage, value);
        }

        public string AppVersion
        {
            get => _appVersion;
            set => SetProperty(ref _appVersion, value);
        }

        public Command NavigateToCommand { get; }
        public Command SelectLanguageCommand { get; }
        public Command LogoutCommand { get; }

        public SettingsViewModel(IPreferencesService preferencesService, IAuthService authService)
        {
            Title = "Settings";
            _preferencesService = preferencesService;
            _authService = authService;

            NavigateToCommand = new Command<string>(async (route) => await NavigateTo(route));
            SelectLanguageCommand = new Command(async () => await SelectLanguage());
            LogoutCommand = new Command(async () => await Logout());

            // Cargar versión de la aplicación
            AppVersion = GetAppVersion();
        }

        public void LoadSettings()
        {
            // Cargar preferencias guardadas
            DarkModeEnabled = _preferencesService.IsDarkMode();
            AutoplayEnabled = _preferencesService.IsAutoplayEnabled();
            SelectedLanguage = _preferencesService.GetLanguage() ?? "English";
        }

        private async Task NavigateTo(string route)
        {
            if (string.IsNullOrEmpty(route))
                return;

            try
            {
                await Shell.Current.GoToAsync(route);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation error: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to navigate to the selected page.", "OK");
            }
        }

        private async Task SelectLanguage()
        {
            string[] languages = { "English", "Spanish", "French", "German", "Portuguese", "Italian", "Japanese", "Chinese", "Korean", "Arabic" };

            string result = await Application.Current.MainPage.DisplayActionSheet(
                "Select Language",
                "Cancel",
                null,
                languages);

            if (!string.IsNullOrEmpty(result) && result != "Cancel")
            {
                SelectedLanguage = result;
                _preferencesService.SetLanguage(result);

                // Aquí podríamos cambiar el idioma de la aplicación
                // Esto requeriría un sistema de localización
            }
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

        private void ApplyTheme()
        {
            // Cambiar tema de la aplicación
            Application.Current.UserAppTheme = DarkModeEnabled
                ? AppTheme.Dark
                : AppTheme.Light;
        }

        private string GetAppVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "1.0.0";
        }

        public bool IsUserLoggedIn()
        {
            return _authService.IsAuthenticated();
        }
    }
}