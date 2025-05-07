namespace ReelBites.Data
{
    public interface IPreferencesService
    {
        string GetAuthToken();
        void SetAuthToken(string token);
        void ClearAuthToken();
        bool IsDarkMode();
        void SetDarkMode(bool isDarkMode);
        string GetUserId();
        void SetUserId(string userId);
        void ClearUserId();
        bool IsAutoplayEnabled();
        void SetAutoplay(bool enabled);
        string GetLanguage();
        void SetLanguage(string language);
    }
}
