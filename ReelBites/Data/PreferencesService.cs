namespace ReelBites.Data
{
    public class PreferencesService : IPreferencesService
    {
        private const string AuthTokenKey = "auth_token";
        private const string UserIdKey = "user_id";
        private const string DarkModeKey = "dark_mode";

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
    }
}
