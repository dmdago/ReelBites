using ReelBites.Services;
using ReelBites.ViewModels;

namespace MicroDrama.Views
{
    [QueryProperty(nameof(UserId), "id")]
    public partial class ProfilePage : ContentPage
    {
        private ProfileViewModel _viewModel;
        private IAuthService _authService;

        public string UserId
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _viewModel.UserId = value;
                }
                else
                {
                    // If no UserId is provided, load the current user's profile
                    _viewModel.UserId = _authService.GetCurrentUserId();
                }
            }
        }

        public ProfilePage(ProfileViewModel viewModel, IAuthService authService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _authService = authService;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // When navigating from the tab bar, we need to load the current user
            if (string.IsNullOrEmpty(_viewModel.UserId) && _authService.IsAuthenticated())
            {
                _viewModel.UserId = _authService.GetCurrentUserId();
            }

            // If we're not authenticated and on the profile tab, redirect to login
            if (!_authService.IsAuthenticated() && string.IsNullOrEmpty(_viewModel.UserId))
            {
                Shell.Current.GoToAsync("//login");
            }
        }
    }
}