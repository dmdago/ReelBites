using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReelBites.Models;
using ReelBites.Services;
using System.Collections.ObjectModel;

namespace ReelBites.ViewModels
{
    [QueryProperty(nameof(DramaId), "id")]
    public class DramaDetailsViewModel : BaseViewModel
    {
        private readonly IDramaService _dramaService;
        private readonly IAuthService _authService;

        private string _dramaId;
        private Drama _drama;
        private bool _isLiked;
        private int _commentsPage = 1;
        private bool _isLoadingMoreComments = false;
        private bool _hasMoreComments = true;
        private string _newCommentText;

        public string DramaId
        {
            get => _dramaId;
            set
            {
                SetProperty(ref _dramaId, value);
                LoadDramaCommand.Execute(null);
            }
        }

        public Drama Drama
        {
            get => _drama;
            set => SetProperty(ref _drama, value);
        }

        public bool IsLiked
        {
            get => _isLiked;
            set => SetProperty(ref _isLiked, value);
        }

        public string NewCommentText
        {
            get => _newCommentText;
            set => SetProperty(ref _newCommentText, value);
        }

        public ObservableCollection<Comment> Comments { get; }

        public Command LoadDramaCommand { get; }
        public Command LoadCommentsCommand { get; }
        public Command LoadMoreCommentsCommand { get; }
        public Command LikeCommand { get; }
        public Command AddCommentCommand { get; }
        public Command ViewProfileCommand { get; }
        public Command ShareCommand { get; }

        public DramaDetailsViewModel(IDramaService dramaService, IAuthService authService)
        {
            _dramaService = dramaService;
            _authService = authService;

            Comments = new ObservableCollection<Comment>();

            LoadDramaCommand = new Command(async () => await LoadDrama());
            LoadCommentsCommand = new Command(async () => await LoadComments());
            LoadMoreCommentsCommand = new Command(async () => await LoadMoreComments());
            LikeCommand = new Command(async () => await ToggleLike());
            AddCommentCommand = new Command(async () => await AddComment());
            ViewProfileCommand = new Command(OnViewProfile);
            ShareCommand = new Command(OnShare);
        }

        async Task LoadDrama()
        {
            if (IsBusy || string.IsNullOrEmpty(DramaId))
                return;

            IsBusy = true;

            try
            {
                Drama = await _dramaService.GetDramaByIdAsync(DramaId);
                Title = Drama?.Title ?? "Drama Details";

                // Check if user liked this drama
                // This would require additional API call or data in the Drama model

                await LoadComments();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading drama: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to load drama details.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task LoadComments()
        {
            if (string.IsNullOrEmpty(DramaId))
                return;

            try
            {
                _commentsPage = 1;
                Comments.Clear();
                var comments = await _dramaService.GetCommentsForDramaAsync(DramaId, _commentsPage);

                foreach (var comment in comments)
                {
                    Comments.Add(comment);
                }

                _hasMoreComments = comments.Count == 20;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading comments: {ex.Message}");
            }
        }

        async Task LoadMoreComments()
        {
            if (_isLoadingMoreComments || !_hasMoreComments || string.IsNullOrEmpty(DramaId))
                return;

            _isLoadingMoreComments = true;

            try
            {
                _commentsPage++;
                var comments = await _dramaService.GetCommentsForDramaAsync(DramaId, _commentsPage);

                foreach (var comment in comments)
                {
                    Comments.Add(comment);
                }

                _hasMoreComments = comments.Count == 20;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading more comments: {ex.Message}");
            }
            finally
            {
                _isLoadingMoreComments = false;
            }
        }

        async Task ToggleLike()
        {
            if (!_authService.IsAuthenticated())
            {
                await Application.Current.MainPage.DisplayAlert("Login Required", "Please login to like dramas.", "OK");
                return;
            }

            try
            {
                bool success;

                if (IsLiked)
                {
                    success = await _dramaService.UnlikeDramaAsync(DramaId);
                    if (success)
                    {
                        IsLiked = false;
                        Drama.LikesCount--;
                    }
                }
                else
                {
                    success = await _dramaService.LikeDramaAsync(DramaId);
                    if (success)
                    {
                        IsLiked = true;
                        Drama.LikesCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling like: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to process your like.", "OK");
            }
        }

        async Task AddComment()
        {
            if (!_authService.IsAuthenticated())
            {
                await Application.Current.MainPage.DisplayAlert("Login Required", "Please login to comment.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewCommentText))
                return;

            try
            {
                var comment = new Comment
                {
                    Content = NewCommentText,
                    DramaId = DramaId,
                    CreatedAt = DateTime.Now
                };

                // This would typically be handled by the API which knows the current user
                var success = await _dramaService.AddCommentAsync(comment);

                if (success)
                {
                    NewCommentText = string.Empty;
                    await LoadComments(); // Reload comments to include the new one
                    Drama.CommentsCount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding comment: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to post your comment.", "OK");
            }
        }

        void OnViewProfile()
        {
            if (Drama?.Author == null)
                return;

            // Navigate to author profile
            Shell.Current.GoToAsync($"profile?id={Drama.Author.Id}");
        }

        async void OnShare()
        {
            try
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Title = Drama?.Title,
                    Text = $"Check out this amazing micro drama: {Drama?.Title}",
                    Uri = $"https://microdrama.com/dramas/{DramaId}"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sharing: {ex.Message}");
            }
        }
    }
}
