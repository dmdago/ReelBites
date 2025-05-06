using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReelBites.Models;
using ReelBites.Data;

namespace ReelBites.Services
{
    public class DramaService : IDramaService
    {
        private readonly IDramaApi _dramaApi;
        private readonly IAuthService _authService;

        public DramaService(IDramaApi dramaApi, IAuthService authService)
        {
            _dramaApi = dramaApi;
            _authService = authService;
        }

        public async Task<List<Drama>> GetTrendingDramasAsync(int page = 1, int pageSize = 20)
        {
            try
            {
                return await _dramaApi.GetTrendingDramasAsync(page, pageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting trending dramas: {ex.Message}");
                return new List<Drama>();
            }
        }

        public async Task<List<Drama>> GetRecommendedDramasAsync(int page = 1, int pageSize = 20)
        {
            if (!_authService.IsAuthenticated())
                return await GetTrendingDramasAsync(page, pageSize);

            try
            {
                return await _dramaApi.GetRecommendedDramasAsync(_authService.GetCurrentUserId(), page, pageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting recommended dramas: {ex.Message}");
                return new List<Drama>();
            }
        }

        public async Task<List<Drama>> GetDramasByCategoryAsync(DramaCategory category, int page = 1, int pageSize = 20)
        {
            try
            {
                return await _dramaApi.GetDramasByCategoryAsync(category, page, pageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting dramas by category: {ex.Message}");
                return new List<Drama>();
            }
        }

        public async Task<List<Drama>> GetDramasByAuthorAsync(string authorId, int page = 1, int pageSize = 20)
        {
            try
            {
                return await _dramaApi.GetDramasByAuthorAsync(authorId, page, pageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting dramas by author: {ex.Message}");
                return new List<Drama>();
            }
        }

        public async Task<Drama> GetDramaByIdAsync(string id)
        {
            try
            {
                return await _dramaApi.GetDramaByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting drama by id: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateDramaAsync(Drama drama)
        {
            if (!_authService.IsAuthenticated())
                return false;

            try
            {
                return await _dramaApi.CreateDramaAsync(drama);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating drama: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateDramaAsync(Drama drama)
        {
            if (!_authService.IsAuthenticated())
                return false;

            try
            {
                return await _dramaApi.UpdateDramaAsync(drama);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating drama: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteDramaAsync(string id)
        {
            if (!_authService.IsAuthenticated())
                return false;

            try
            {
                return await _dramaApi.DeleteDramaAsync(id, _authService.GetCurrentUserId());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting drama: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LikeDramaAsync(string id)
        {
            if (!_authService.IsAuthenticated())
                return false;

            try
            {
                return await _dramaApi.LikeDramaAsync(id, _authService.GetCurrentUserId());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error liking drama: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UnlikeDramaAsync(string id)
        {
            if (!_authService.IsAuthenticated())
                return false;

            try
            {
                return await _dramaApi.UnlikeDramaAsync(id, _authService.GetCurrentUserId());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unliking drama: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Comment>> GetCommentsForDramaAsync(string dramaId, int page = 1, int pageSize = 20)
        {
            try
            {
                return await _dramaApi.GetCommentsForDramaAsync(dramaId, page, pageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting comments: {ex.Message}");
                return new List<Comment>();
            }
        }

        public async Task<bool> AddCommentAsync(Comment comment)
        {
            if (!_authService.IsAuthenticated())
                return false;

            try
            {
                // Extraer el ID del drama del objeto Comment
                string dramaId = comment.DramaId;
                string userId = _authService.GetCurrentUserId();

                // Asegurar que el userId esté establecido
                comment.UserId = userId;

                // Llamar a la API
                return await _dramaApi.AddCommentAsync(dramaId, comment, userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding comment: {ex.Message}");
                return false;
            }
        }
    }
}
