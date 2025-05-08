namespace ReelBites.Data
{
    public class PreferencesService : IPreferencesService
    {
        private const string AuthTokenKey = "auth_token";
        private const string UserIdKey = "user_id";
        private const string DarkModeKey = "dark_mode";
        private const string AutoplayKey = "autoplay";
        private const string LanguageKey = "language";

        public string GetAuthToken()
        {
            return Preferences.Get(AuthTokenKey, string.Empty);
        }

        public void SetAuthToken(string token)
        {
            Preferences.Set(AuthTokenKey, token);
        }

        public void ClearAuthToken()
        {
            Preferences.Remove(AuthTokenKey);
        }

        public bool IsDarkMode()
        {
            return Preferences.Get(DarkModeKey, false);
        }

        public void SetDarkMode(bool isDarkMode)
        {
            Preferences.Set(DarkModeKey, isDarkMode);
        }

        public string GetUserId()
        {
            return Preferences.Get(UserIdKey, string.Empty);
        }

        public void SetUserId(string userId)
        {
            Preferences.Set(UserIdKey, userId);
        }

        public void ClearUserId()
        {
            Preferences.Remove(UserIdKey);
        }
        public bool IsAutoplayEnabled()
        {
            return Preferences.Get(AutoplayKey, true); // Por defecto habilitado
        }
        public void SetAutoplay(bool enabled)
        {
            Preferences.Set(AutoplayKey, enabled);
        }

        public string GetLanguage()
        {
            return Preferences.Get(LanguageKey, "English");
        }

        public void SetLanguage(string language)
        {
            Preferences.Set(LanguageKey, language ?? "English");
        }
        private const string GuestModeKey = "guest_mode";

        public bool IsGuestMode()
        {
            return Preferences.Get(GuestModeKey, false);
        }

        public void SetIsGuestMode(bool isGuest)
        {
            Preferences.Set(GuestModeKey, isGuest);
        }
    }
}
