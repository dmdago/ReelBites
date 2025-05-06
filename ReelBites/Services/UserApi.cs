using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using ReelBites.Models;
using Newtonsoft.Json;

namespace ReelBites.Services
{
    public class UserApi : IUserApi
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.microdrama.com/v1/users";
        private readonly IPreferencesService _preferencesService;

        public UserApi(HttpClient httpClient, IPreferencesService preferencesService)
        {
            _httpClient = httpClient;
            _preferencesService = preferencesService;

            // Configure authentication header
            string token = _preferencesService.GetAuthToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{id}");
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<User>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateUserProfileAsync(User user)
        {
            try
            {
                var content = JsonContent.Create(user);
                var response = await _httpClient.PutAsync($"{_baseUrl}/{user.Id}", content);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> FollowUserAsync(string followerId, string followedId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/{followerId}/follow/{followedId}", null);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UnfollowUserAsync(string followerId, string followedId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/{followerId}/follow/{followedId}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<User>> GetFollowersAsync(string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{userId}/followers?page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<User>>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<List<User>> GetFollowingAsync(string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{userId}/following?page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<User>>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<bool> IsFollowingAsync(string followerId, string followedId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/{followerId}/is-following/{followedId}");
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<IsFollowingResponse>(jsonResponse);

                return result?.IsFollowing ?? false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        private class IsFollowingResponse
        {
            public bool IsFollowing { get; set; }
        }
    }
}
