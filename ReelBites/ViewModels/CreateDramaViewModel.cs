using IntelliJ.Lang.Annotations;
using ReelBites.Models;
using ReelBites.Services;
using ReelBites.ViewModels;
using System.Collections.ObjectModel;

namespace ReelBites.ViewModels
{
    public class CreateDramaViewModel : BaseViewModel
    {
        private readonly IDramaService _dramaService;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        private string _title;
        private string _content;
        private double _duration = 5; // Default to 5 minutes
        private string _tags;
        private bool _isPremium;
        private DramaCategory _selectedCategory = DramaCategory.Comedy; // Default category
        private string _coverImageUrl;
        private User _currentUser;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public double Duration
        {
            get => _duration;
            set => SetProperty(ref _duration, value);
        }

        public string Tags
        {
            get => _tags;
            set => SetProperty(ref _tags, value);
        }

        public bool IsPremium
        {
            get => _isPremium;
            set => SetProperty(ref _isPremium, value);
        }

        public DramaCategory SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public string CoverImageUrl
        {
            get => _coverImageUrl;
            set => SetProperty(ref _coverImageUrl, value);
        }

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public ObservableCollection<DramaCategory> Categories { get; } = new ObservableCollection<DramaCategory>();

        public bool CanPublish => !string.IsNullOrWhiteSpace(Title) &&
                                  !string.IsNullOrWhiteSpace(Content) &&
                                  !string.IsNullOrWhiteSpace(CoverImageUrl);

        public Command InitializeCommand { get; }
        public Command ChooseImageCommand { get; }
        public Command PreviewCommand { get; }
        public Command PublishCommand { get; }

        public CreateDramaViewModel(IDramaService dramaService, IUserService userService, IAuthService authService)
        {
            Title = "Create Drama";
            _dramaService = dramaService;
            _userService = userService;
            _authService = authService;

            InitializeCommand = new Command(async () => await Initialize());
            ChooseImageCommand = new Command(async () => await ChooseImage());
            PreviewCommand = new Command(async () => await PreviewDrama());
            PublishCommand = new Command(async () => await PublishDrama(), () => CanPublish);

            // Call Initialize by default
            InitializeCommand.Execute(null);
        }

        private async Task Initialize()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                // Load current user
                CurrentUser = await _userService.GetCurrentUserAsync();

                // Load categories
                if (Categories.Count == 0)
                {
                    // Get all enum values
                    var categoryValues = Enum.GetValues(typeof(DramaCategory)).Cast<DramaCategory>();
                    foreach (var category in categoryValues)
                    {
                        Categories.Add(category);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to initialize. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ChooseImage()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select Cover Image"
                });

                if (result != null)
                {
                    // Load the photo
                    var stream = await result.OpenReadAsync();

                    // In a real app, you would upload this to a server and get back the URL
                    // For this example, we'll just set a placeholder URL
                    CoverImageUrl = "https://api.microdrama.com/uploads/covers/placeholder.jpg";

                    // Simulate uploading the image
                    await Task.Delay(1000);

                    // Update the CanPublish property
                    OnPropertyChanged(nameof(CanPublish));
                    PublishCommand.ChangeCanExecute();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error choosing image: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to select image. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task PreviewDrama()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Content))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please provide a title and content for your drama before previewing.", "OK");
                return;
            }

            // Create a preview drama
            var previewDrama = CreateDramaObject();

            // Store the preview in a static property or service
            // StaticPreviewService.CurrentPreview = previewDrama;

            // Navigate to preview page
            await Shell.Current.GoToAsync("dramapreview");
        }

        public async Task<bool> PublishDrama()
        {
            if (IsBusy)
                return false;

            if (!CanPublish)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please complete all required fields before publishing.", "OK");
                return false;
            }

            IsBusy = true;

            try
            {
                // Create drama object
                var drama = CreateDramaObject();

                // Publish drama
                bool success = await _dramaService.CreateDramaAsync(drama);

                if (success)
                {
                    // Reset the form
                    Title = string.Empty;
                    Content = string.Empty;
                    Tags = string.Empty;
                    CoverImageUrl = string.Empty;
                    Duration = 5;
                    IsPremium = false;
                    SelectedCategory = DramaCategory.Comedy;

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing drama: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to publish your drama. Please try again.", "OK");
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private Drama CreateDramaObject()
        {
            var tagsList = new List<string>();

            if (!string.IsNullOrWhiteSpace(Tags))
            {
                tagsList = Tags.Split(',')
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .ToList();
            }

            return new Drama
            {
                Title = Title,
                Content = Content,
                Author = CurrentUser,
                CreatedAt = DateTime.Now,
                CoverImageUrl = CoverImageUrl,
                Category = SelectedCategory,
                Tags = tagsList,
                Duration = Duration,
                IsPremium = IsPremium,
                // IDs will be generated by the server
                LikesCount = 0,
                CommentsCount = 0,
                SharesCount = 0,
                ViewsCount = 0
            };
        }
    }
}