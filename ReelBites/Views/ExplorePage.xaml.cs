using ReelBites.ViewModels;

namespace ReelBites.Views
{
    public partial class ExplorePage : ContentPage
    {
        private ExploreViewModel _viewModel;

        public ExplorePage(ExploreViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Load data when the page appears if it's not already loaded
            if (_viewModel.Dramas.Count == 0)
            {
                _viewModel.LoadDramasCommand.Execute(null);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // If user has performed a search, clear it and return to default state instead of navigating back
            if (!string.IsNullOrEmpty(_viewModel.SearchQuery))
            {
                _viewModel.SearchQuery = string.Empty;
                _viewModel.SelectedCategory = "All";
                _viewModel.LoadDramasCommand.Execute(null);
                return true; // Handled
            }

            return base.OnBackButtonPressed();
        }
    }
}