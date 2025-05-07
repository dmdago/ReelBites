using ReelBites.Models;
using ReelBites.Services;
using System.Collections.ObjectModel;

namespace ReelBites.ViewModels
{
    public class FollowersViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        private string _userId;
        private string _searchQuery;
        private string _emptyMessage = "This user doesn't have any followers yet";
        private int _currentPage = 1;
        private bool _isLoadingMore = false;
        private bool _hasMoreFollowers = true;

        public string UserId
        {
            get => _userId;
            set
            {
                SetProperty(ref _userId, value);
                LoadTitle();
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                SetProperty(ref _searchQuery, value);
                // Ejecutar búsqueda cuando cambia el texto
                ExecuteSearch();
            }
        }

        public string EmptyMessage
        {
            get => _emptyMessage;
            set => SetProperty(ref _emptyMessage, value);
        }

        public bool HasMoreFollowers
        {
            get => _hasMoreFollowers;
            set => SetProperty(ref _hasMoreFollowers, value);
        }

        // Colección principal que se vincula al UI
        public ObservableCollection<User> Followers { get; }

        // Colección de respaldo para almacenar todos los elementos sin filtrar
        private List<User> _allFollowers;

        public Command LoadFollowersCommand { get; }
        public Command LoadMoreCommand { get; }
        public Command SearchCommand { get; }
        public Command<User> ViewProfileCommand { get; }
        public Command<User> ToggleFollowCommand { get; }

        public FollowersViewModel(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;

            Followers = new ObservableCollection<User>();
            _allFollowers = new List<User>();

            LoadFollowersCommand = new Command(async () => await LoadFollowers());
            LoadMoreCommand = new Command(async () => await LoadMoreFollowers());
            SearchCommand = new Command(ExecuteSearch);
            ViewProfileCommand = new Command<User>(OnViewProfile);
            ToggleFollowCommand = new Command<User>(async (user) => await ToggleFollow(user));

            Title = "Followers";
        }

        private async void LoadTitle()
        {
            if (string.IsNullOrEmpty(UserId))
                return;

            if (UserId == _authService.GetCurrentUserId())
            {
                Title = "Your Followers";
                EmptyMessage = "You don't have any followers yet";
            }
            else
            {
                try
                {
                    var user = await _userService.GetUserByIdAsync(UserId);
                    if (user != null)
                    {
                        Title = $"{user.Username}'s Followers";
                        EmptyMessage = $"{user.Username} doesn't have any followers yet";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading user: {ex.Message}");
                }
            }
        }

        async Task LoadFollowers()
        {
            if (IsBusy || string.IsNullOrEmpty(UserId))
                return;

            IsBusy = true;

            try
            {
                _currentPage = 1;
                _allFollowers.Clear();
                Followers.Clear();

                var followers = await _userService.GetFollowersAsync(UserId, _currentPage);

                foreach (var follower in followers)
                {
                    // Verificar si el usuario actual sigue a este seguidor
                    follower.IsFollowing = await _userService.IsFollowingAsync(follower.Id);
                    _allFollowers.Add(follower);
                }

                // Aplicar filtro si hay una búsqueda activa, o mostrar todos
                ExecuteSearch();

                HasMoreFollowers = followers.Count == 20; // Asumiendo que el tamaño de página es 20
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading followers: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to load followers. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task LoadMoreFollowers()
        {
            if (IsBusy || _isLoadingMore || !HasMoreFollowers || string.IsNullOrEmpty(UserId))
                return;

            _isLoadingMore = true;

            try
            {
                _currentPage++;

                var followers = await _userService.GetFollowersAsync(UserId, _currentPage);

                foreach (var follower in followers)
                {
                    // Verificar si el usuario actual sigue a este seguidor
                    follower.IsFollowing = await _userService.IsFollowingAsync(follower.Id);
                    _allFollowers.Add(follower);
                }

                HasMoreFollowers = followers.Count == 20;

                // Aplicar el filtro actual a los nuevos elementos
                ExecuteSearch();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading more followers: {ex.Message}");
            }
            finally
            {
                _isLoadingMore = false;
            }
        }

        void ExecuteSearch()
        {
            Followers.Clear();

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                // Si no hay búsqueda, mostrar todos los seguidores
                foreach (var follower in _allFollowers)
                {
                    Followers.Add(follower);
                }
            }
            else
            {
                // Filtrar seguidores por nombre de usuario o nombre completo
                var lowercaseQuery = SearchQuery.ToLower();

                foreach (var follower in _allFollowers)
                {
                    if (follower.Username.ToLower().Contains(lowercaseQuery) ||
                        (follower.FullName != null && follower.FullName.ToLower().Contains(lowercaseQuery)))
                    {
                        Followers.Add(follower);
                    }
                }
            }
        }

        void OnViewProfile(User user)
        {
            if (user == null)
                return;

            // Navegar al perfil del usuario
            Shell.Current.GoToAsync($"profile?id={user.Id}");
        }

        async Task ToggleFollow(User user)
        {
            if (user == null || !_authService.IsAuthenticated())
                return;

            // No permitir seguirse a uno mismo
            if (user.Id == _authService.GetCurrentUserId())
                return;

            try
            {
                bool success;

                if (user.IsFollowing)
                {
                    // Dejar de seguir
                    success = await _userService.UnfollowUserAsync(user.Id);

                    if (success)
                    {
                        user.IsFollowing = false;

                        // Actualizar la colección de respaldo
                        var backupUser = _allFollowers.FirstOrDefault(u => u.Id == user.Id);
                        if (backupUser != null)
                        {
                            backupUser.IsFollowing = false;
                        }
                    }
                }
                else
                {
                    // Seguir
                    success = await _userService.FollowUserAsync(user.Id);

                    if (success)
                    {
                        user.IsFollowing = true;

                        // Actualizar la colección de respaldo
                        var backupUser = _allFollowers.FirstOrDefault(u => u.Id == user.Id);
                        if (backupUser != null)
                        {
                            backupUser.IsFollowing = true;
                        }
                    }
                }

                if (!success)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to update follow status. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling follow: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "An error occurred. Please try again.", "OK");
            }
        }
    }
}