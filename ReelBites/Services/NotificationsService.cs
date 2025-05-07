using Newtonsoft.Json;
using ReelBites.Data;
using ReelBites.Models;

namespace ReelBites.Services
{
    public interface INotificationService
    {
        Task<List<Notification>> GetNotificationsAsync(string userId, int page = 1, int pageSize = 20);
        Task<List<Notification>> GetNewNotificationsAsync(string userId, DateTime? since = null);
        Task<bool> MarkAsReadAsync(string notificationId);
        Task<bool> MarkAllAsReadAsync(string userId);
        Task<int> GetUnreadCountAsync(string userId);
    }

    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.reelbites.com/v1";
        private readonly IPreferencesService _preferencesService;

        public NotificationService(HttpClient httpClient, IPreferencesService preferencesService)
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

        public async Task<List<Notification>> GetNotificationsAsync(string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/notifications?userId={userId}&page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Notification>>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new List<Notification>();
            }
        }

        public async Task<List<Notification>> GetNewNotificationsAsync(string userId, DateTime? since = null)
        {
            try
            {
                string url = $"{_baseUrl}/notifications/new?userId={userId}";

                if (since.HasValue)
                {
                    url += $"&since={since.Value:s}";
                }

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Notification>>(jsonResponse) ?? new List<Notification>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new List<Notification>(); // Siempre devuelve una lista vacía en caso de error, nunca null
            }
        }

        public async Task<bool> MarkAsReadAsync(string notificationId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/notifications/{notificationId}/read", null);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> MarkAllAsReadAsync(string userId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/notifications/mark-all-read?userId={userId}", null);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/notifications/unread-count?userId={userId}");
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UnreadCountResponse>(jsonResponse);

                return result?.Count ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return 0;
            }
        }

        private class UnreadCountResponse
        {
            public int Count { get; set; }
        }
    }
}