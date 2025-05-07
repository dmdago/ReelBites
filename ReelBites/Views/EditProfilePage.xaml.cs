using ReelBites.ViewModels;

namespace ReelBites.Views
{
    public partial class EditProfilePage : ContentPage
    {
        private EditProfileViewModel _viewModel;

        public EditProfilePage(EditProfileViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Verificar autenticación
            if (!_viewModel.IsUserLoggedIn())
            {
                await Shell.Current.GoToAsync("//login");
                return;
            }

            // Cargar información del perfil
            await _viewModel.LoadProfileAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            // Mostrar un diálogo si hay cambios sin guardar
            if (_viewModel.HasUnsavedChanges)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool leave = await DisplayAlert(
                        "Unsaved Changes",
                        "You have unsaved changes. Are you sure you want to leave?",
                        "Leave", "Stay");

                    if (leave)
                    {
                        await Navigation.PopAsync();
                    }
                });

                return true; // Manejamos el evento
            }

            return base.OnBackButtonPressed();
        }
    }
}