using ReelBites.Data;

namespace ReelBites.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthApi _authApi;
        private readonly IPreferencesService _preferencesService;
        private bool _isGuestMode = false;

        public AuthService(IAuthApi authApi, IPreferencesService preferencesService)
        {
            _authApi = authApi;
            _preferencesService = preferencesService;
        }

        public async Task<bool> RegisterAsync(string email, string username, string password)
        {
            try
            {
                string token = await _authApi.RegisterAsync(email, username, password);

                if (!string.IsNullOrEmpty(token))
                {
                    // Registration succeeded, save token and set user as authenticated
                    _preferencesService.SetAuthToken(token);

                    // We also need to get the user ID and save it
                    var userId = await _authApi.ValidateTokenAsync(token) ? token.Split('.')[0] : null;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        _preferencesService.SetUserId(userId);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                string token = await _authApi.LoginAsync(email, password);

                if (!string.IsNullOrEmpty(token))
                {
                    // Login succeeded, save token and set user as authenticated
                    _preferencesService.SetAuthToken(token);

                    // We also need to get the user ID and save it
                    var userId = await _authApi.ValidateTokenAsync(token) ? token.Split('.')[0] : null;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        _preferencesService.SetUserId(userId);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LogoutAsync()
        {
            _preferencesService.ClearAuthToken();
            _preferencesService.ClearUserId();

            return await Task.FromResult(true);
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                return await _authApi.ForgotPasswordAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Forgot password error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            try
            {
                return await _authApi.ResetPasswordAsync(token, newPassword);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reset password error: {ex.Message}");
                return false;
            }
        }

        public bool IsAuthenticated()
        {
            // Si está en modo invitado, considerar como "autenticado" para navegación,
            // pero con permisos limitados
            if (_isGuestMode || _preferencesService.IsGuestMode())
            {
                _isGuestMode = true;
                return true;
            }

            // Verificación normal de token
            return !string.IsNullOrEmpty(_preferencesService.GetAuthToken());
            string token = _preferencesService.GetAuthToken();
            return !string.IsNullOrEmpty(token);
        }

        public string GetCurrentUserId()
        {
            return _preferencesService.GetUserId();
        }
        public bool IsGuestMode()
        {
            return _isGuestMode;
        }

        public async Task LoginAsGuestAsync()
        {
            _isGuestMode = true;
            // No establecer token de autenticación
            // pero posiblemente guardar este estado en preferencias
            _preferencesService.SetIsGuestMode(true);
        }
    }
}
