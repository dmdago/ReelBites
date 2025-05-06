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
