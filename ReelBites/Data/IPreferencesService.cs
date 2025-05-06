using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
