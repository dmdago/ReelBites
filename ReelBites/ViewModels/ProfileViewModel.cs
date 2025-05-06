using ReelBites.Models;
using ReelBites.Services;
using System.Collections.ObjectModel;

namespace ReelBites.ViewModels
{
    [QueryProperty(nameof(UserId), "id")]
    public class ProfileViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IDramaService _dramaService;
        private readonly IAuthService _authService;

        private string _userId;
        private User _user;
        private bool _isCurrentUser;
        private bool _isFollowing;
        private int _currentPage = 1;
        private bool _isLoadingMore = false;
        private bool _hasMoreItems = true;

        public string UserId
        {
            get => _userId;
            set
            {
                SetProperty(ref _userId, value);
                LoadUserCommand.Execute(null);
            }
        }

        public User User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public bool IsCurrentUser
        {
            get => _isCurrentUser;
            set => SetProperty(ref _isCurrentUser, value);
        }

        public bool IsFollowing
        {
            get => _isFollowing;
            set => SetProperty(ref _isFollowing, value);
        }

        public ObservableCollection<Drama> UserDramas { get; }

        public Command LoadUserCommand { get; }
        public Command LoadDramasCommand { get; }
        public Command LoadMoreDramasCommand { get; }
        public Command FollowCommand { get; }
        public Command EditProfileCommand { get; }
        public Command<Drama> DramaTappedCommand { get; }
        public Command ViewFollowersCommand { get; }
        public Command ViewFollowingCommand { get; }

        public ProfileViewModel(IUserService userService, IDramaService dramaService, IAuthService authService)
        {
            _userService = userService;
            _dramaService = dramaService;
            _authService = authService;

            UserDramas = new ObservableCollection<Drama>();

            LoadUserCommand = new Command(async () => await LoadUser());
            LoadDramasCommand = new Command(async () => await LoadDramas());
            LoadMoreDramasCommand = new Command(async () => await LoadMoreDramas());
            FollowCommand = new Command(async () => await ToggleFollow());
            EditProfileCommand = new Command(OnEditProfile);
            DramaTappedCommand = new Command<Drama>(OnDramaTapped);
            ViewFollowersCommand = new Command(OnViewFollowers);
            ViewFollowingCommand = new Command(OnViewFollowing);
        }

        async Task LoadUser()
        {
            if (IsBusy || string.IsNullOrEmpty(UserId))
                return;

            IsBusy = true;

            try
            {
                User = await _userService.GetUserByIdAsync(UserId);
                Title = User?.Username ?? "Profile";

                IsCurrentUser = _authService.IsAuthenticated() &&
                                _authService.GetCurrentUserId() == UserId;

                if (!IsCurrentUser && _authService.IsAuthenticated())
                {
                    IsFollowing = await _userService.IsFollowingAsync(UserId);
                }

                await LoadDramas();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to load profile.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task LoadDramas()
        {
            if (string.IsNullOrEmpty(UserId))
                return;

            try
            {
                _currentPage = 1;
                UserDramas.Clear();
                var dramas = await _dramaService.GetDramasByAuthorAsync(UserId, _currentPage);

                foreach (var drama in dramas)
                {
                    UserDramas.Add(drama);
                }

                _hasMoreItems = dramas.Count == 20;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user dramas: {ex.Message}");
            }
        }

        async Task LoadMoreDramas()
        {
            if (_isLoadingMore || !_hasMoreItems || string.IsNullOrEmpty(UserId))
                return;

            _isLoadingMore = true;

            try
            {
                _currentPage++;
                var dramas = await _dramaService.GetDramasByAuthorAsync(UserId, _currentPage);

                foreach (var drama in dramas)
                {
                    UserDramas.Add(drama);
                }

                _hasMoreItems = dramas.Count == 20;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading more dramas: {ex.Message}");
            }
            finally
            {
                _isLoadingMore = false;
            }
        }

        async Task ToggleFollow()
        {
            if (!_authService.IsAuthenticated())
            {
                await Application.Current.MainPage.DisplayAlert("Login Required", "Please login to follow users.", "OK");
                return;
            }

            try
            {
                bool success;

                if (IsFollowing)
                {
                    success = await _userService.UnfollowUserAsync(UserId);
                    if (success)
                    {
                        IsFollowing = false;
                        User.FollowersCount--;
                    }
                }
                else
                {
                    success = await _userService.FollowUserAsync(UserId);
                    if (success)
                    {
                        IsFollowing = true;
                        User.FollowersCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling follow: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to process your follow request.", "OK");
            }
        }

        void OnEditProfile()
        {
            if (!IsCurrentUser)
                return;

            // Navigate to edit profile page
            Shell.Current.GoToAsync("editprofile");
        }

        void OnDramaTapped(Drama drama)
        {
            if (drama == null)
                return;

            // Navigate to drama details page
            Shell.Current.GoToAsync($"dramadetails?id={drama.Id}");
        }

        void OnViewFollowers()
        {
            // Navigate to followers page
            Shell.Current.GoToAsync($"followers?id={UserId}");
        }

        void OnViewFollowing()
        {
            // Navigate to following page
            Shell.Current.GoToAsync($"following?id={UserId}");
        }
    }
}
