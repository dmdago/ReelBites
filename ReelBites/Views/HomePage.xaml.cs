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

        // Si el usuario no est� autenticado, redirigir a la p�gina de login
        if (!_viewModel.IsUserLoggedIn())
        {
            Shell.Current.GoToAsync("//login");
        }
    }
}