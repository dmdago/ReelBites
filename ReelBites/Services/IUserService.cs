using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReelBites.Models;


namespace ReelBites.Services
{
    public interface IUserService
    {
        Task<User> GetCurrentUserAsync();
        Task<User> GetUserByIdAsync(string id);
        Task<bool> UpdateUserProfileAsync(User user);
        Task<bool> FollowUserAsync(string userId);
        Task<bool> UnfollowUserAsync(string userId);
        Task<List<User>> GetFollowersAsync(string userId, int page = 1, int pageSize = 20);
        Task<List<User>> GetFollowingAsync(string userId, int page = 1, int pageSize = 20);
        Task<bool> IsFollowingAsync(string userId);
    }
}
