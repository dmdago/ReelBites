using ReelBites.Models;
using ReelBites.Services;
using System.Collections.ObjectModel;

namespace ReelBites.ViewModels
{
    public class FollowingViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        private string _userId;
        private string _searchQuery;
        private string _emptyMessage = "This user isn't following anyone yet";
        private int _currentPage = 1;
        private bool _isLoadingMore = false;
        private bool _hasMoreFollowing = true;

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

        public bool HasMoreFollowing
        {
            get => _hasMoreFollowing;
            set => SetProperty(ref _hasMoreFollowing, value);
        }

        // Colección principal que se vincula al UI
        public ObservableCollection<User> Following { get; }

        // Colección de respaldo para almacenar todos los elementos sin filtrar
        private List<User> _allFollowing;

        public Command LoadFollowingCommand { get; }
        public Command LoadMoreCommand { get; }
        public Command SearchCommand { get; }
        public Command<User> ViewProfileCommand { get; }
        public Command<User> UnfollowCommand { get; }

        public FollowingViewModel(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;

            Following = new ObservableCollection<User>();
            _allFollowing = new List<User>();

            LoadFollowingCommand = new Command(async () => await LoadFollowing());
            LoadMoreCommand = new Command(async () => await LoadMoreFollowing());
            SearchCommand = new Command(ExecuteSearch);
            ViewProfileCommand = new Command<User>(OnViewProfile);
            UnfollowCommand = new Command<User>(async (user) => await Unfollow(user));

            Title = "Following";
        }

        private async void LoadTitle()
        {
            if (string.IsNullOrEmpty(UserId))
                return;

            if (UserId == _authService.GetCurrentUserId())
            {
                Title = "Your Following";
                EmptyMessage = "You aren't following anyone yet";
            }
            else
            {
                try
                {
                    var user = await _userService.GetUserByIdAsync(UserId);
                    if (user != null)
                    {
                        Title = $"{user.Username}'s Following";
                        EmptyMessage = $"{user.Username} isn't following anyone yet";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading user: {ex.Message}");
                }
            }
        }

        async Task LoadFollowing()
        {
            if (IsBusy || string.IsNullOrEmpty(UserId))
                return;

            IsBusy = true;

            try
            {
                _currentPage = 1;
                _allFollowing.Clear();
                Following.Clear();

                var following = await _userService.GetFollowingAsync(UserId, _currentPage);

                foreach (var user in following)
                {
                    // Todos los usuarios seguidos tienen IsFollowing = true
                    user.IsFollowing = true;
                    _allFollowing.Add(user);
                }

                // Aplicar filtro si hay una búsqueda activa, o mostrar todos
                ExecuteSearch();

                HasMoreFollowing = following.Count == 20; // Asumiendo que el tamaño de página es 20
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading following: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to load following. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task LoadMoreFollowing()
        {
            if (IsBusy || _isLoadingMore || !HasMoreFollowing || string.IsNullOrEmpty(UserId))
                return;

            _isLoadingMore = true;

            try
            {
                _currentPage++;

                var following = await _userService.GetFollowingAsync(UserId, _currentPage);

                foreach (var user in following)
                {
                    // Todos los usuarios seguidos tienen IsFollowing = true
                    user.IsFollowing = true;
                    _allFollowing.Add(user);
                }

                HasMoreFollowing = following.Count == 20;

                // Aplicar el filtro actual a los nuevos elementos
                ExecuteSearch();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading more following: {ex.Message}");
            }
            finally
            {
                _isLoadingMore = false;
            }
        }

        void ExecuteSearch()
        {
            Following.Clear();

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                // Si no hay búsqueda, mostrar todos los usuarios seguidos
                foreach (var user in _allFollowing)
                {
                    Following.Add(user);
                }
            }
            else
            {
                // Filtrar usuarios por nombre de usuario o nombre completo
                var lowercaseQuery = SearchQuery.ToLower();

                foreach (var user in _allFollowing)
                {
                    if (user.Username.ToLower().Contains(lowercaseQuery) ||
                        (user.FullName != null && user.FullName.ToLower().Contains(lowercaseQuery)))
                    {
                        Following.Add(user);
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

        async Task Unfollow(User user)
        {
            if (user == null || !_authService.IsAuthenticated())
                return;

            // Verificar que el usuario actual es el mismo que está viendo la lista
            if (UserId != _authService.GetCurrentUserId())
            {
                await Application.Current.MainPage.DisplayAlert("Error", "You can only unfollow users from your own following list.", "OK");
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Unfollow",
                $"Are you sure you want to unfollow {user.Username}?",
                "Unfollow", "Cancel");

            if (!confirm)
                return;

            try
            {
                bool success = await _userService.UnfollowUserAsync(user.Id);

                if (success)
                {
                    // Eliminar usuario de ambas colecciones
                    _allFollowing.Remove(user);

                    // Actualizar UI
                    ExecuteSearch();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Failed to unfollow user. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unfollowing user: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "An error occurred. Please try again.", "OK");
            }
        }
    }
}