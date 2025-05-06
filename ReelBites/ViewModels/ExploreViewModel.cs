using ReelBites.Models;
using ReelBites.Services;
using System.Collections.ObjectModel;

namespace ReelBites.ViewModels
{
    public class ExploreViewModel : BaseViewModel
    {
        private readonly IDramaService _dramaService;

        private string _searchQuery;
        private string _selectedCategory = "All";
        private int _currentPage = 1;
        private bool _isLoadingMore = false;
        private bool _hasMoreItems = true;
        private string _emptyMessage = "Search for dramas or select a category to explore";

        public string SearchQuery
        {
            get => _searchQuery;
            set => SetProperty(ref _searchQuery, value);
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public string EmptyMessage
        {
            get => _emptyMessage;
            set => SetProperty(ref _emptyMessage, value);
        }

        public bool HasMoreItems
        {
            get => _hasMoreItems;
            set => SetProperty(ref _hasMoreItems, value);
        }

        public ObservableCollection<Drama> Dramas { get; }

        public Command LoadDramasCommand { get; }
        public Command LoadMoreCommand { get; }
        public Command SearchCommand { get; }
        public Command<string> SelectCategoryCommand { get; }
        public Command<Drama> DramaTappedCommand { get; }

        public ExploreViewModel(IDramaService dramaService)
        {
            Title = "Explore";
            _dramaService = dramaService;

            Dramas = new ObservableCollection<Drama>();

            LoadDramasCommand = new Command(async () => await LoadDramas());
            LoadMoreCommand = new Command(async () => await LoadMoreDramas());
            SearchCommand = new Command(async () => await PerformSearch());
            SelectCategoryCommand = new Command<string>(OnCategorySelected);
            DramaTappedCommand = new Command<Drama>(OnDramaTapped);
        }

        async Task LoadDramas()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                _currentPage = 1;
                Dramas.Clear();

                List<Drama> dramaList;

                if (!string.IsNullOrEmpty(SearchQuery))
                {
                    // Search with query
                    dramaList = await _dramaService.SearchDramasAsync(SearchQuery, SelectedCategory, _currentPage);
                    EmptyMessage = "No dramas found for your search";
                }
                else if (SelectedCategory != "All")
                {
                    // Filter by category
                    DramaCategory category = (DramaCategory)Enum.Parse(typeof(DramaCategory), SelectedCategory);
                    dramaList = await _dramaService.GetDramasByCategoryAsync(category, _currentPage);
                    EmptyMessage = $"No dramas found in the {SelectedCategory} category";
                }
                else
                {
                    // Get all dramas
                    dramaList = await _dramaService.GetAllDramasAsync(_currentPage);
                    EmptyMessage = "No dramas available at the moment";
                }

                foreach (var drama in dramaList)
                {
                    Dramas.Add(drama);
                }

                HasMoreItems = dramaList.Count == 20; // Assuming page size is 20
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading dramas: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to load dramas. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task LoadMoreDramas()
        {
            if (IsBusy || _isLoadingMore || !HasMoreItems)
                return;

            _isLoadingMore = true;

            try
            {
                _currentPage++;

                List<Drama> dramaList;

                if (!string.IsNullOrEmpty(SearchQuery))
                {
                    // Search with query
                    dramaList = await _dramaService.SearchDramasAsync(SearchQuery, SelectedCategory, _currentPage);
                }
                else if (SelectedCategory != "All")
                {
                    // Filter by category
                    DramaCategory category = (DramaCategory)Enum.Parse(typeof(DramaCategory), SelectedCategory);
                    dramaList = await _dramaService.GetDramasByCategoryAsync(category, _currentPage);
                }
                else
                {
                    // Get all dramas
                    dramaList = await _dramaService.GetAllDramasAsync(_currentPage);
                }

                foreach (var drama in dramaList)
                {
                    Dramas.Add(drama);
                }

                HasMoreItems = dramaList.Count == 20;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading more dramas: {ex.Message}");
            }
            finally
            {
                _isLoadingMore = false;
            }
        }

        async Task PerformSearch()
        {
            await LoadDramas();
        }

        void OnCategorySelected(string category)
        {
            if (SelectedCategory == category)
                return;

            SelectedCategory = category;
            LoadDramasCommand.Execute(null);
        }

        void OnDramaTapped(Drama drama)
        {
            if (drama == null)
                return;

            // Navigate to drama details page
            Shell.Current.GoToAsync($"dramadetails?id={drama.Id}");
        }
    }
}