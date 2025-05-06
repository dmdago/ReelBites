using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReelBites.Models;


namespace ReelBites.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string email, string username, string password);
        Task<bool> LoginAsync(string email, string password);
        Task<bool> LogoutAsync();
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        bool IsAuthenticated();
        string GetCurrentUserId();
    }
}
