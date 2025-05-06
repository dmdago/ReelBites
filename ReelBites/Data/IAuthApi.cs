using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReelBites.Data
{
    public interface IAuthApi
    {
        Task<string> RegisterAsync(string email, string username, string password);
        Task<string> LoginAsync(string email, string password);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        Task<bool> ValidateTokenAsync(string token);
    }
}
