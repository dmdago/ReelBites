namespace ReelBites.Views;
using ReelBites.ViewModels;

public partial class HomePage : ContentPage
{
    private HomeViewModel _viewModel;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Load data when the page appears if it's not already loaded
        if (_viewModel.TrendingDramas.Count == 0)
        {
            _viewModel.LoadTrendingCommand.Execute(null);
        }

        if (_viewModel.RecommendedDramas.Count == 0)
        {
            _viewModel.LoadRecommendedCommand.Execute(null);
        }
    }
}