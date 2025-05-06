using ReelBites.Models;

namespace ReelBites.Services
{
    public interface IDramaService
    {
        Task<List<Drama>> GetTrendingDramasAsync(int page = 1, int pageSize = 20);
        Task<List<Drama>> GetRecommendedDramasAsync(int page = 1, int pageSize = 20);
        Task<List<Drama>> GetDramasByCategoryAsync(DramaCategory category, int page = 1, int pageSize = 20);
        Task<List<Drama>> GetDramasByAuthorAsync(string authorId, int page = 1, int pageSize = 20);
        Task<Drama> GetDramaByIdAsync(string id);
        Task<bool> CreateDramaAsync(Drama drama);
        Task<bool> UpdateDramaAsync(Drama drama);
        Task<bool> DeleteDramaAsync(string id);
        Task<bool> LikeDramaAsync(string id);
        Task<bool> UnlikeDramaAsync(string id);
        Task<List<Comment>> GetCommentsForDramaAsync(string dramaId, int page = 1, int pageSize = 20);
        Task<bool> AddCommentAsync(Comment comment);
        Task<List<Drama>> GetAllDramasAsync(int page = 1, int pageSize = 20);
        Task<List<Drama>> SearchDramasAsync(string query, string category = "All", int page = 1, int pageSize = 20);
    }
}
