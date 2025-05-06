using ReelBites.Services;
using ReelBites.ViewModels;

namespace ReelBites.Views
{
    public partial class CreateDramaPage : ContentPage
    {
        private CreateDramaViewModel _viewModel;
        private readonly IAuthService _authService;

        public CreateDramaPage(CreateDramaViewModel viewModel, IAuthService authService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _authService = authService;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Check if user is authenticated, if not redirect to login
            if (!_authService.IsAuthenticated())
            {
                Shell.Current.GoToAsync("//login");
                return;
            }

            // Initialize view model if needed
            if (_viewModel.Categories == null || _viewModel.Categories.Count == 0)
            {
                _viewModel.InitializeCommand.Execute(null);
            }
        }

        private async void OnPreviewClicked(object sender, EventArgs e)
        {
            // Additional preview logic if needed beyond what's in the view model
        }

        private async void OnPublishClicked(object sender, EventArgs e)
        {
            // Additional publish logic if needed beyond what's in the view model

            if (await _viewModel.PublishDrama())
            {
                // Show success message
                await DisplayAlert("Success", "Your drama has been published successfully!", "OK");

                // Navigate back or to the drama details
                await Shell.Current.GoToAsync($"//home");
            }
            else
            {
                // Show error message
                await DisplayAlert("Error", "There was a problem publishing your drama. Please try again.", "OK");
            }
        }
    }
}