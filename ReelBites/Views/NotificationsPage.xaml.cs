using ReelBites.ViewModels;

namespace ReelBites.Views
{
    public partial class NotificationsPage : ContentPage
    {
        private NotificationsViewModel _viewModel;

        public NotificationsPage(NotificationsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Load notifications when the page appears if it's not already loaded
            if (_viewModel.Notifications.Count == 0)
            {
                _viewModel.LoadNotificationsCommand.Execute(null);
            }
            else
            {
                // Refresh to check for new notifications
                _viewModel.RefreshNotificationsCommand.Execute(null);
            }
        }
    }
}