using ReelBites.Data;
using System.Net.Http.Json;
using System.Text.Json;

namespace ReelBites.Services
{
    public class AuthApi : IAuthApi
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.microdrama.com/v1/auth";

        public AuthApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> RegisterAsync(string email, string username, string password)
        {
            try
            {
                var registerData = new
                {
                    email,
                    username,
                    password
                };

                var content = JsonContent.Create(registerData);
                var response = await _httpClient.PostAsync($"{_baseUrl}/register", content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TokenResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return null;
            }
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            try
            {
                var loginData = new
                {
                    email,
                    password
                };

                var content = JsonContent.Create(loginData);
                var response = await _httpClient.PostAsync($"{_baseUrl}/login", content);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TokenResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Token;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var forgotData = new
                {
                    email
                };

                var content = JsonContent.Create(forgotData);
                var response = await _httpClient.PostAsync($"{_baseUrl}/forgot-password", content);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            try
            {
                var resetData = new
                {
                    token,
                    newPassword
                };

                var content = JsonContent.Create(resetData);
                var response = await _httpClient.PostAsync($"{_baseUrl}/reset-password", content);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{_baseUrl}/validate");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        private class TokenResponse
        {
            public string Token { get; set; }
        }
    }
}
