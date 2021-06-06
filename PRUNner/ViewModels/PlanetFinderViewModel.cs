﻿using System.Collections.Generic;
using System.Linq;
using PRUNner.Backend.Data;
using PRUNner.Backend.PlanetFinder;
using ReactiveUI;

namespace PRUNner.ViewModels
{
    public class PlanetFinderViewModel : ViewModelBase
    {
        public string Item1 { get; set; } = "";
        public string Item2 { get; set; } = "";
        public string Item3 { get; set; } = "";
        public string Item4 { get; set; } = "";

        public bool DisplayItem1 => !string.IsNullOrWhiteSpace(Item1);
        public bool DisplayItem2 => !string.IsNullOrWhiteSpace(Item2);
        public bool DisplayItem3 => !string.IsNullOrWhiteSpace(Item3);
        public bool DisplayItem4 => !string.IsNullOrWhiteSpace(Item4);
        
        public bool DisplayFertile { get; set; }
        public bool DisplayRocky { get; set; } = true;
        public bool DisplayGaseous { get; set; }
        public bool DisplayLowGravity { get; set; }
        public bool DisplayLowPressure { get; set; }
        public bool DisplayLowTemperature { get; set; }
        public bool DisplayHighGravity { get; set; }
        public bool DisplayHighPressure { get; set; }
        public bool DisplayHighTemperature { get; set; }

        private bool _showPaginationAndHeaders;
        public bool ShowPaginationAndHeaders
        {
            get => _showPaginationAndHeaders;
            set => this.RaiseAndSetIfChanged(ref _showPaginationAndHeaders, value);
        }

        private bool _noResultsFound;
        public bool NoResultsFound
        {
            get => _noResultsFound;
            set =>  this.RaiseAndSetIfChanged(ref _noResultsFound, value);
        }

        private int _currentPage;
        public int CurrentPage
        {
            get => _currentPage;
            set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }

        private int _totalPages;
        public int TotalPages
        {
            get => _totalPages;
            set => this.RaiseAndSetIfChanged(ref _totalPages, value);
        }
        
        private List<PlanetFinderSearchResult> _allResults = new();
        private IEnumerable<PlanetFinderSearchResult> _searchResults = new List<PlanetFinderSearchResult>();
        public IEnumerable<PlanetFinderSearchResult> SearchResults
        {
            get => _searchResults;
            private set => this.RaiseAndSetIfChanged(ref _searchResults, value);
        }

        public int ItemsPerPage { get; set; } = 15;

        public void Search()
        {
            var filterCriteria = new FilterCriteria()
            {
                ExcludeGaseous = !DisplayGaseous,
                ExcludeRocky = !DisplayRocky,
                ExcludeInfertile = DisplayFertile,
                ExcludeLowGravity = !DisplayLowGravity,
                ExcludeLowPressure = !DisplayLowPressure,
                ExcludeLowTemperature = !DisplayLowTemperature,
                ExcludeHighGravity = !DisplayHighGravity,
                ExcludeHighPressure = !DisplayHighPressure,
                ExcludeHighTemperature = !DisplayHighTemperature
            };

            var tickers = new List<string>() {Item1, Item2, Item3, Item4}
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => MaterialData.AllItems[x])
                .ToArray();
            
            this.RaisePropertyChanged(nameof(DisplayItem1));
            this.RaisePropertyChanged(nameof(DisplayItem2));
            this.RaisePropertyChanged(nameof(DisplayItem3));
            this.RaisePropertyChanged(nameof(DisplayItem4));         
            this.RaisePropertyChanged(nameof(Item1));
            this.RaisePropertyChanged(nameof(Item2));
            this.RaisePropertyChanged(nameof(Item3));
            this.RaisePropertyChanged(nameof(Item4));

            _allResults = PlanetFinder.Find(filterCriteria, tickers).OrderBy(x => x.DistancePyrgos).ToList();
            CurrentPage = 1;
            TotalPages = _allResults.Count / ItemsPerPage + 1;
            ShowPaginationAndHeaders = _allResults.Count > 0;
            NoResultsFound = _allResults.Count == 0;
            SearchResults = _allResults.Take(ItemsPerPage);
        }
        
        public void NextPage()
        {
            CurrentPage++;
            if (CurrentPage > TotalPages)
            {
                CurrentPage = 1;
            }
            
            SearchResults = _allResults.Skip(ItemsPerPage * CurrentPage - ItemsPerPage).Take(ItemsPerPage);
        }
        
        public void PreviousPage()
        {
            CurrentPage--;
            if (CurrentPage < 1)
            {
                CurrentPage = TotalPages;
            }
            
            SearchResults = _allResults.Skip(ItemsPerPage * CurrentPage - ItemsPerPage).Take(ItemsPerPage);
        }
    }
}