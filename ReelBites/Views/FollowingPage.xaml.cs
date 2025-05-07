using ReelBites.ViewModels;

namespace ReelBites.Views
{
    [QueryProperty(nameof(UserId), "id")]
    public partial class FollowingPage : ContentPage
    {
        private FollowingViewModel _viewModel;

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

        public FollowingPage(FollowingViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Cargar usuarios seguidos solo si no se han cargado ya o si se ha navegado a otra página
            if (_viewModel.Following.Count == 0 && !string.IsNullOrEmpty(_viewModel.UserId))
            {
                _viewModel.LoadFollowingCommand.Execute(null);
            }
        }
    }
}