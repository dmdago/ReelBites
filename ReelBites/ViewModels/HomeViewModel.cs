using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ReelBites.Models;
using ReelBites.Services;

namespace ReelBites.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private readonly IDramaService _dramaService;
        private int _currentPage = 1;
        private bool _isLoadingMore = false;
        private bool _hasMoreItems = true;

        public ObservableCollection<Drama> TrendingDramas { get; }
        public ObservableCollection<Drama> RecommendedDramas { get; }

        public Command LoadTrendingCommand { get; }
        public Command LoadRecommendedCommand { get; }
        public Command LoadMoreTrendingCommand { get; }
        public Command LoadMoreRecommendedCommand { get; }
        public Command RefreshCommand { get; }
        public Command<Drama> DramaTappedCommand { get; }

        public HomeViewModel(IDramaService dramaService)
        {
            Title = "Home";
            _dramaService = dramaService;

            TrendingDramas = new ObservableCollection<Drama>();
            RecommendedDramas = new ObservableCollection<Drama>();

            LoadTrendingCommand = new Command(async () => await LoadTrendingDramas());
            LoadRecommendedCommand = new Command(async () => await LoadRecommendedDramas());
            LoadMoreTrendingCommand = new Command(async () => await LoadMoreTrendingDramas());
            LoadMoreRecommendedCommand = new Command(async () => await LoadMoreRecommendedDramas());
            RefreshCommand = new Command(async () => await RefreshData());
            DramaTappedCommand = new Command<Drama>(OnDramaTapped);
        }

        async Task LoadTrendingDramas()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                _currentPage = 1;
                TrendingDramas.Clear();
                var dramas = await _dramaService.GetTrendingDramasAsync(_currentPage);

                foreach (var drama in dramas)
                {
                    TrendingDramas.Add(drama);
                }

                _hasMoreItems = dramas.Count == 20; // Assuming page size is 20
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading trending dramas: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to load trending dramas.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task LoadRecommendedDramas()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                _currentPage = 1;
                RecommendedDramas.Clear();
                var dramas = await _dramaService.GetRecommendedDramasAsync(_currentPage);

                foreach (var drama in dramas)
                {
                    RecommendedDramas.Add(drama);
                }

                _hasMoreItems = dramas.Count == 20; // Assuming page size is 20
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading recommended dramas: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to load recommended dramas.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task LoadMoreTrendingDramas()
        {
            if (IsBusy || _isLoadingMore || !_hasMoreItems)
                return;

            _isLoadingMore = true;

            try
            {
                _currentPage++;
                var dramas = await _dramaService.GetTrendingDramasAsync(_currentPage);

                foreach (var drama in dramas)
                {
                    TrendingDramas.Add(drama);
                }

                _hasMoreItems = dramas.Count == 20;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading more trending dramas: {ex.Message}");
            }
            finally
            {
                _isLoadingMore = false;
            }
        }

        async Task LoadMoreRecommendedDramas()
        {
            if (IsBusy || _isLoadingMore || !_hasMoreItems)
                return;

            _isLoadingMore = true;

            try
            {
                _currentPage++;
                var dramas = await _dramaService.GetRecommendedDramasAsync(_currentPage);

                foreach (var drama in dramas)
                {
                    RecommendedDramas.Add(drama);
                }

                _hasMoreItems = dramas.Count == 20;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading more recommended dramas: {ex.Message}");
            }
            finally
            {
                _isLoadingMore = false;
            }
        }

        async Task RefreshData()
        {
            await LoadTrendingDramas();
            await LoadRecommendedDramas();
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
