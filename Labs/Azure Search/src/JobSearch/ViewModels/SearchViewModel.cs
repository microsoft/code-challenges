using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using JobSearch.Annotations;
using JobSearch.Controls;
using JobSearch.Extensions;
using JobSearch.SearchModels;
using JobSearch.Services;
using Microsoft.Azure.Search.Models;

namespace JobSearch.ViewModels
{
    public class SearchViewModel : INotifyPropertyChanged
    {
        private readonly JobSearchService _searchService;
        private readonly MapControl _map;
        private readonly LocationSearchIcon _searchLocationIcon;

        public ObservableCollection<JobResult> SearchResults { get; }
        public ObservableCollection<FacetGroup> SearchFacets { get; }

        public int[] KilometersSearch => new[] { 1, 2, 5, 10, 15, 20, 25, 50, 100, 150 };

        public Visibility ShowBusyRing => IsBusy ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ShowContent => IsBusy ? Visibility.Collapsed : Visibility.Visible;

        private bool _isBusy = false;
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
                OnPropertyChanged(nameof(ShowBusyRing));
                OnPropertyChanged(nameof(ShowContent));
            }
        }

        private int _kilometersSelected = 1;
        public int KilometersSelected
        {
            get { return _kilometersSelected; }
            set
            {
                _kilometersSelected = value;
                OnPropertyChanged(nameof(KilometersSelected));
            }
        }

        private string _searchQuery = "";
        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
            }
        }

        public SearchViewModel(MapControl mapControl)
        {
            _searchService = new JobSearchService();
            SearchResults = new ObservableCollection<JobResult>();
            SearchFacets = new ObservableCollection<FacetGroup>();
            _map = mapControl;
            _searchLocationIcon = new LocationSearchIcon();
        }

        public async Task ExecuteFilter()
        {
            IsBusy = true;
            try
            {
                var results = await _searchService.ExecuteSearch(SearchQuery, SearchFacets.ToList());
                var synonymMap = await GetSynonymMap(SearchQuery);
                await UpdateSearchResults(results.JobResults, synonymMap);
                UpdateFacets(results.Facets, true);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task ExecuteGeoFilter()
        {
            IsBusy = true;
            try
            {
                var results = await _searchService.ExecuteSearch(SearchQuery, SearchFacets.ToList(), new PositionDistanceSearch() { GeoPoint = _searchLocationIcon.GeoPoint, Radius = KilometersSelected });
                var synonymMap = await GetSynonymMap(SearchQuery);
                await UpdateSearchResults(results.JobResults, synonymMap);
                UpdateFacets(results.Facets, true);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public Visibility ShowFacets => (SearchResults.Count > 0 && SearchFacets.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

        public async Task<List<string>> ExecuteSuggest(string currentText)
        {
            return await _searchService.ExecuteSuggest(currentText);
        }

        public async void ExecuteSearch()
        {
            IsBusy = true;
            try
            {
                var results = await _searchService.ExecuteSearch(SearchQuery, null);
                var synonymMap = await GetSynonymMap(SearchQuery);
                await UpdateSearchResults(results.JobResults, synonymMap);
                UpdateFacets(results.Facets);
            }
            catch
            {
                //Ignore
                await UpdateSearchResults(null, null);
                UpdateFacets(null, false);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private List<string> SynonymMap { get; set; }
        private async Task<List<string>> GetSynonymMap(string queryString)
        {
            try
            {
                if (SynonymMap == null)
                {
                    var synonymMap = await _searchService.GetSynonymMap();
                    SynonymMap = synonymMap;
                }

                if (SynonymMap.Any(x => x.Equals(queryString, StringComparison.OrdinalIgnoreCase)))
                {
                    return SynonymMap;
                }
            }
            catch (Exception Ex)
            {
            }
            return new List<string>();
        }

        private async Task UpdateSearchResults(IList<JobResult> results, List<string> synonymMap)
        {
            SearchResults.Clear();
            _map.Children.Clear();

            if (results != null)
            {
                foreach (var result in results)
                {
                    var doc = result;
                    doc.HighlighWords = synonymMap.ToArray();
                    SearchResults.Add(doc);
                }
                var locations = new List<Geopoint>();

                foreach (var result in SearchResults)
                {
                    locations.Add(await AddPin(result.GeoLocation.Latitude, result.GeoLocation.Longitude));
                }
                if (_searchLocationIcon?.GeoPoint != null)
                {
                    await UpdateUserLocation(_searchLocationIcon.GeoPoint);
                }
            }
        }

        private void UpdateFacets(FacetResults facets, bool bindExistingFacets = false)
        {
            var existingSelectedFacets =
                    SearchFacets.Select(e => new FacetGroup()
                    {
                        FacetName = e.FacetName,
                        FacetValues = e.FacetValues.Where(f => f.IsSelected.HasValue && f.IsSelected.Value).ToList()
                    }).ToList();

            SearchFacets.Clear();

            if (facets != null)
            {
                foreach (var result in facets)
                {
                    SearchFacets.Add(new FacetGroup()
                    {
                        FacetName = result.Key,
                        FacetDisplayName = JobSearchService.FacetDefinitions.ContainsKey(result.Key) ? JobSearchService.FacetDefinitions[result.Key] : result.Key,
                        FacetValues =
                            result.Value.Select(e => new FacetSelection() { FacetValue = e.Value.ToString(), IsSelected = bindExistingFacets && existingSelectedFacets.Any(ef => ef.FacetName == result.Key && ef.FacetValues.Any(fv => fv.FacetValue == e.Value.ToString())), FacetCount = e.Count })
                                .ToList()
                    });
                }
            }
            OnPropertyChanged(nameof(ShowFacets));
        }

        private async Task UpdateElementLocation(DependencyObject dependentObject, Geopoint point, Point anchorPoint)
        {
            if (dependentObject != null)
            {
                await _map.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    MapControl.SetLocation(dependentObject, point);
                    MapControl.SetNormalizedAnchorPoint(dependentObject, anchorPoint);
                });
            }
        }
        private async Task<Geopoint> AddPin(double latitude, double longitude)
        {
            var location = new Geopoint(new BasicGeoposition { Longitude = longitude, Latitude = latitude });
            var icon = new CustomMapIcon();
            _map.Children.Add(icon);
            await UpdateElementLocation(icon, location, new Point(0.5, 1.0));
            return location;
        }

        private async Task ScopeToBoundingBox(List<Geopoint> coordinateSet)
        {
            var boundingBox = CreateBoundingBoxFromCoordiateSet(coordinateSet);
            await _map.TrySetViewBoundsAsync(boundingBox, new Thickness(25), MapAnimationKind.None);
        }

        private GeoboundingBox CreateBoundingBoxFromCoordiateSet(List<Geopoint> coordinateSet)
        {
            var first = coordinateSet.First();
            var northWestCorner = new BasicGeoposition();
            var southEastCorner = new BasicGeoposition();
            for (int i = 1; i < coordinateSet.Count; i++)
            {
                var coordinate = coordinateSet[i];
                if (coordinate.Position.Latitude > northWestCorner.Latitude)
                    northWestCorner.Latitude = coordinate.Position.Latitude;
                if (coordinate.Position.Latitude < southEastCorner.Latitude)
                    southEastCorner.Latitude = coordinate.Position.Latitude;

                if (coordinate.Position.Longitude < northWestCorner.Longitude)
                    northWestCorner.Longitude = coordinate.Position.Longitude;
                if (coordinate.Position.Longitude > southEastCorner.Longitude)
                    southEastCorner.Longitude = coordinate.Position.Longitude;
            }
            var box = new GeoboundingBox(northWestCorner, southEastCorner);
            return box;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task UpdateUserLocation(Geopoint position)
        {
            if (!_map.Children.Contains(_searchLocationIcon))
            {
                _map.Children.Add(_searchLocationIcon);
            }
            _searchLocationIcon.GeoPoint = position;
            await UpdateElementLocation(_searchLocationIcon, position, new Point(0.5, 0.5));
            UpdateSearchRadius();
        }

        public void UpdateSearchRadius()
        {
            if (_searchLocationIcon?.GeoPoint != null)
            {
                var mapPolygon = new MapPolygon()
                {
                    Path = new Geopath(_searchLocationIcon.GeoPoint.GetCirclePoints(KilometersSelected * 1000)),
                    ZIndex = -1,
                    FillColor = Color.FromArgb(128, 128, 128, 128),
                    StrokeThickness = 0
                };
                foreach (var element in _map.MapElements.Where(e => e.ZIndex == -1).ToList())
                {
                    _map.MapElements.Remove(element);
                }
                _map.MapElements.Add(mapPolygon);
            }

        }


    }
}
