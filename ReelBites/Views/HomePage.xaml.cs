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

        // Si el usuario no está autenticado, redirigir a la página de login
        if (!_viewModel.IsUserLoggedIn())
        {
            Shell.Current.GoToAsync("//login");
        }
    }
}