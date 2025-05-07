using ReelBites.ViewModels;

namespace ReelBites.Views
{
    [QueryProperty(nameof(UserId), "id")]
    public partial class FollowersPage : ContentPage
    {
        private FollowersViewModel _viewModel;

        public string UserId
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _viewModel.UserId = value;
                }
            }
        }

        public FollowersPage(FollowersViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Cargar seguidores solo si no se han cargado ya o si se ha navegado a otra página
            if (_viewModel.Followers.Count == 0 && !string.IsNullOrEmpty(_viewModel.UserId))
            {
                _viewModel.LoadFollowersCommand.Execute(null);
            }
        }
    }
}