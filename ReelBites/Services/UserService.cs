using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReelBites.Models;
using ReelBites.Data;

namespace ReelBites.Services
{
    public class UserService : IUserService
    {
        private readonly IUserApi _userApi;
        private readonly IAuthService _authService;

        public UserService(IUserApi userApi, IAuthService authService)
        {
            _userApi = userApi;
            _authService = authService;
        }

        public async Task<User> GetCurrentUserAsync()
        {
            if (!_authService.IsAuthenticated())
                return null;

            string userId = _authService.GetCurrentUserId();
            return await GetUserByIdAsync(userId);
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            try
            {
                return await _userApi.GetUserByIdAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateUserProfileAsync(User user)
        {
            if (!_authService.IsAuthenticated())
                return false;

            try
            {
                return await _userApi.UpdateUserProfileAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating profile: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> FollowUserAsync(string userId)
        {
            if (!_authService.IsAuthenticated())
                return false;

            string currentUserId = _authService.GetCurrentUserId();

            try
            {
                return await _userApi.FollowUserAsync(currentUserId, userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error following user: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UnfollowUserAsync(string userId)
        {
            if (!_authService.IsAuthenticated())
                return false;

            string currentUserId = _authService.GetCurrentUserId();

            try
            {
                return await _userApi.UnfollowUserAsync(currentUserId, userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unfollowing user: {ex.Message}");
                return false;
            }
        }

        public async Task<List<User>> GetFollowersAsync(string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                return await _userApi.GetFollowersAsync(userId, page, pageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting followers: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<List<User>> GetFollowingAsync(string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                return await _userApi.GetFollowingAsync(userId, page, pageSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting following: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<bool> IsFollowingAsync(string userId)
        {
            if (!_authService.IsAuthenticated())
                return false;

            string currentUserId = _authService.GetCurrentUserId();

            try
            {
                return await _userApi.IsFollowingAsync(currentUserId, userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking follow status: {ex.Message}");
                return false;
            }
        }
    }
}
