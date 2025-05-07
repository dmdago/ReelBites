using ReelBites.Models;
using ReelBites.Services;
using System.Collections.ObjectModel;

namespace ReelBites.ViewModels
{
    public class NotificationsViewModel : BaseViewModel
    {
        private readonly INotificationService _notificationService;
        private readonly IAuthService _authService;

        private int _currentPage = 1;
        private bool _isLoadingMore = false;
        private bool _hasMoreNotifications = true;
        private bool _hasUnreadNotifications = false;

        public bool HasMoreNotifications
        {
            get => _hasMoreNotifications;
            set => SetProperty(ref _hasMoreNotifications, value);
        }

        public bool HasUnreadNotifications
        {
            get => _hasUnreadNotifications;
            set => SetProperty(ref _hasUnreadNotifications, value);
        }

        public ObservableCollection<Notification> Notifications { get; }

        public Command LoadNotificationsCommand { get; }
        public Command RefreshNotificationsCommand { get; }
        public Command LoadMoreCommand { get; }
        public Command MarkAllAsReadCommand { get; }
        public Command<Notification> NotificationTappedCommand { get; }

        public NotificationsViewModel(INotificationService notificationService, IAuthService authService)
        {
            Title = "Notifications";
            _notificationService = notificationService;
            _authService = authService;

            Notifications = new ObservableCollection<Notification>();

            LoadNotificationsCommand = new Command(async () => await LoadNotifications());
            RefreshNotificationsCommand = new Command(async () => await RefreshNotifications());
            LoadMoreCommand = new Command(async () => await LoadMoreNotifications());
            MarkAllAsReadCommand = new Command(async () => await MarkAllAsRead());
            NotificationTappedCommand = new Command<Notification>(OnNotificationTapped);
        }

        async Task LoadNotifications()
        {
            if (IsBusy)
                return;

            if (!_authService.IsAuthenticated())
            {
                await Application.Current.MainPage.DisplayAlert("Login Required", "Please login to view notifications.", "OK");
                await Shell.Current.GoToAsync("//login");
                return;
            }

            IsBusy = true;

            try
            {
                _currentPage = 1;
                Notifications.Clear();

                var userId = _authService.GetCurrentUserId();
                var notifications = await _notificationService.GetNotificationsAsync(userId, _currentPage);

                foreach (var notification in notifications)
                {
                    Notifications.Add(notification);
                }

                HasMoreNotifications = notifications.Count == 20; // Asumiendo que el tamaño de página es 20
                HasUnreadNotifications = Notifications.Any(n => !n.IsRead);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading notifications: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to load notifications. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task RefreshNotifications()
        {
            if (!_authService.IsAuthenticated())
                return;

            try
            {
                var userId = _authService.GetCurrentUserId();
                List<Notification> newNotifications = await _notificationService.GetNewNotificationsAsync(userId);

                // Verificamos que tengamos notificaciones para agregar
                if (newNotifications != null && newNotifications.Count > 0)
                {
                    // Iteramos manualmente en orden inverso para insertar las más recientes primero
                    for (int i = newNotifications.Count - 1; i >= 0; i--)
                    {
                        Notifications.Insert(0, newNotifications[i]);
                    }
                }

                // Actualizamos la bandera de notificaciones no leídas
                HasUnreadNotifications = Notifications.Any(n => !n.IsRead);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error refreshing notifications: {ex.Message}");
            }
        }

        async Task LoadMoreNotifications()
        {
            if (IsBusy || _isLoadingMore || !HasMoreNotifications)
                return;

            _isLoadingMore = true;

            try
            {
                _currentPage++;

                var userId = _authService.GetCurrentUserId();
                var notifications = await _notificationService.GetNotificationsAsync(userId, _currentPage);

                foreach (var notification in notifications)
                {
                    Notifications.Add(notification);
                }

                HasMoreNotifications = notifications.Count == 20;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading more notifications: {ex.Message}");
            }
            finally
            {
                _isLoadingMore = false;
            }
        }

        async Task MarkAllAsRead()
        {
            if (!_authService.IsAuthenticated())
                return;

            try
            {
                var userId = _authService.GetCurrentUserId();
                bool success = await _notificationService.MarkAllAsReadAsync(userId);

                if (success)
                {
                    // Update local notification objects
                    foreach (var notification in Notifications)
                    {
                        notification.IsRead = true;
                    }

                    HasUnreadNotifications = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error marking notifications as read: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to mark notifications as read. Please try again.", "OK");
            }
        }

        async void OnNotificationTapped(Notification notification)
        {
            if (notification == null)
                return;

            try
            {
                // Mark notification as read if it's not already read
                if (!notification.IsRead)
                {
                    var success = await _notificationService.MarkAsReadAsync(notification.Id);

                    if (success)
                    {
                        notification.IsRead = true;

                        // Check if there are still unread notifications
                        HasUnreadNotifications = Notifications.Any(n => !n.IsRead);
                    }
                }

                // Navigate based on notification type and related item
                switch (notification.Type)
                {
                    case NotificationType.Like:
                    case NotificationType.Comment:
                        await Shell.Current.GoToAsync($"dramadetails?id={notification.RelatedItemId}");
                        break;

                    case NotificationType.Follow:
                        await Shell.Current.GoToAsync($"profile?id={notification.RelatedItemId}");
                        break;

                    case NotificationType.NewDrama:
                        await Shell.Current.GoToAsync($"dramadetails?id={notification.RelatedItemId}");
                        break;

                    case NotificationType.Mention:
                        await Shell.Current.GoToAsync($"dramadetails?id={notification.RelatedItemId}");
                        break;

                    case NotificationType.System:
                        // System notifications might not have navigation
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling notification tap: {ex.Message}");
            }
        }
    }
}