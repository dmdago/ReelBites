using ReelBites.ViewModels;

namespace ReelBites.Views
{

    [QueryProperty(nameof(DramaId), "id")]
    public partial class DramaDetailsPage : ContentPage
    {
        private DramaDetailsViewModel _viewModel;

        public string DramaId
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _viewModel.DramaId = value;
                }
            }
        }

        public DramaDetailsPage(DramaDetailsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Ensure comments are loaded when the page appears
            if (_viewModel.Comments.Count == 0 && !string.IsNullOrEmpty(_viewModel.DramaId))
            {
                _viewModel.LoadCommentsCommand.Execute(null);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Custom back button behavior if needed
            return base.OnBackButtonPressed();
        }
    }
}