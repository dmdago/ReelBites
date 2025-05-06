using Newtonsoft.Json;
using ReelBites.Models;
using System.Net.Http.Json;


namespace ReelBites.Data
{
    public class DramaApi : IDramaApi
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.microdrama.com/v1";
        private readonly IPreferencesService _preferencesService;

        public DramaApi(HttpClient httpClient, IPreferencesService preferencesService)
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

        public async Task<List<Drama>> GetTrendingDramasAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/dramas/trending?page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Drama>>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new List<Drama>();
            }
        }

        public async Task<List<Drama>> GetRecommendedDramasAsync(string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/dramas/recommended?userId={userId}&page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Drama>>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new List<Drama>();
            }
        }

        public async Task<List<Drama>> GetDramasByCategoryAsync(DramaCategory category, int page = 1, int pageSize = 20)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/dramas/category/{category}?page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Drama>>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new List<Drama>();
            }
        }

        public async Task<List<Drama>> GetDramasByAuthorAsync(string authorId, int page = 1, int pageSize = 20)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/dramas/author/{authorId}?page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Drama>>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new List<Drama>();
            }
        }

        public async Task<Drama> GetDramaByIdAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/dramas/{id}");
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Drama>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateDramaAsync(Drama drama)
        {
            try
            {
                var content = JsonContent.Create(drama);
                var response = await _httpClient.PostAsync($"{_baseUrl}/dramas", content);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateDramaAsync(Drama drama)
        {
            try
            {
                var content = JsonContent.Create(drama);
                var response = await _httpClient.PutAsync($"{_baseUrl}/dramas/{drama.Id}", content);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteDramaAsync(string id, string userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/dramas/{id}?userId={userId}");
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LikeDramaAsync(string id, string userId)
        {
            try
            {
                var content = JsonContent.Create(new { userId });
                var response = await _httpClient.PostAsync($"{_baseUrl}/dramas/{id}/like", content);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UnlikeDramaAsync(string id, string userId)
        {
            try
            {
                var content = JsonContent.Create(new { userId });
                var response = await _httpClient.PostAsync($"{_baseUrl}/dramas/{id}/unlike", content);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Comment>> GetCommentsForDramaAsync(string dramaId, int page = 1, int pageSize = 20)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/dramas/{dramaId}/comments?page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Comment>>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new List<Comment>();
            }
        }

        public async Task<bool> AddCommentAsync(string dramaId, Comment comment, string userId)
        {
            // Implementación para agregar un comentario a la API
            // Ejemplo:
            try
            {
                comment.UserId = userId;
                comment.DramaId = dramaId;
                comment.CreatedAt = DateTime.UtcNow;

                // Lógica para enviar el comentario a la API o base de datos
                // Por ejemplo:
                // var response = await _httpClient.PostAsJsonAsync($"api/dramas/{dramaId}/comments", comment);
                // return response.IsSuccessStatusCode;

                // Implementación simulada:
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Drama>> GetAllDramasAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/dramas?page={page}&pageSize={pageSize}");
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Drama>>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new List<Drama>();
            }
        }

        public async Task<List<Drama>> SearchDramasAsync(string query, DramaCategory? category = null, int page = 1, int pageSize = 20)
        {
            try
            {
                string url = $"{_baseUrl}/dramas/search?query={Uri.EscapeDataString(query)}&page={page}&pageSize={pageSize}";

                if (category.HasValue)
                {
                    url += $"&category={category.Value}";
                }

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Drama>>(jsonResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new List<Drama>();
            }
        }
    }
}
