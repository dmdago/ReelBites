﻿using ReelBites.Models;


namespace ReelBites.Data
{
    public interface IDramaApi
    {
        Task<List<Drama>> GetTrendingDramasAsync(int page = 1, int pageSize = 20);
        Task<List<Drama>> GetRecommendedDramasAsync(string userId, int page = 1, int pageSize = 20);
        Task<List<Drama>> GetDramasByCategoryAsync(DramaCategory category, int page = 1, int pageSize = 20);
        Task<List<Drama>> GetDramasByAuthorAsync(string authorId, int page = 1, int pageSize = 20);
        Task<Drama> GetDramaByIdAsync(string id);
        Task<bool> CreateDramaAsync(Drama drama);
        Task<bool> UpdateDramaAsync(Drama drama);
        Task<bool> DeleteDramaAsync(string id, string userId);
        Task<bool> LikeDramaAsync(string id, string userId);
        Task<bool> UnlikeDramaAsync(string id, string userId);
        Task<List<Comment>> GetCommentsForDramaAsync(string dramaId, int page = 1, int pageSize = 20);
        Task<bool> AddCommentAsync(string dramaId, Comment comment, string userId);
        Task<List<Drama>> GetAllDramasAsync(int page = 1, int pageSize = 20);
        Task<List<Drama>> SearchDramasAsync(string query, DramaCategory? category = null, int page = 1, int pageSize = 20);
    }
}
