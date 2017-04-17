using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Audio;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using JobSearch.SearchModels;
using JobSearch.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace JobSearch.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Search : Page
    {
        public Search()
        {
            this.InitializeComponent();
            this.Searcher = new SearchViewModel(MapControl);
        }

        public SearchViewModel Searcher
        {
            get; set;
        }

        private async void SearchBox_OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                if (sender.Text.Length > 2)
                {
                    sender.ItemsSource = await Searcher.ExecuteSuggest(sender.Text);
                }
            }
        }

        private void SearchBox_OnSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            sender.Text = $"\"{args.SelectedItem}\"";
            Searcher.ExecuteSearch();
        }

        private void SearchBox_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Searcher.ExecuteSearch();
            }
        }

        private async void MapControl_OnMapTapped(object sender, MapInputEventArgs args)
        {
            await Searcher.UpdateUserLocation(args.Location);
        }

        private async void MapControl_OnMapDoubleTapped(object sender, MapInputEventArgs args)
        {
            await Searcher.UpdateUserLocation(args.Location);
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Searcher.KilometersSelected = (int) e.AddedItems[0];
            Searcher.UpdateSearchRadius();
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (!_stopBinding)
            {
                var checkBox = sender as CheckBox;
                var facet = checkBox?.DataContext as FacetSelection;
                if (facet != null)
                {
                    facet.IsSelected = true;
                    Searcher.ExecuteFilter();
                }
            }
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (!_stopBinding)
            {
                var checkBox = sender as CheckBox;
                var facet = checkBox?.DataContext as FacetSelection;
                if (facet != null)
                {
                    facet.IsSelected = false;
                    Searcher.ExecuteFilter();
                }
            }
        }

        private bool _stopBinding = false;

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            _stopBinding = true;
            try
            {
                var checkBox = sender as CheckBox;
                var facet = checkBox?.DataContext as FacetSelection;
                if (facet != null)
                {
                    checkBox.IsChecked = facet.IsSelected;
                }
            }
            finally
            {
                _stopBinding = false;
            }
        }
    }
}
