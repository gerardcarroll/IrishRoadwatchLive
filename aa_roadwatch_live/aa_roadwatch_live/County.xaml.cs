using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using aa_roadwatch_live.Models;
using aa_roadwatch_live.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace aa_roadwatch_live
{
    public partial class County : PhoneApplicationPage
    {
        List<ItemViewModel> placeList = new List<ItemViewModel>();
        bool placesGot = false;
         
        public County()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!placesGot)
            {
                foreach (var itemViewModel in App.ViewModel.Items)
                {
                    placeList.Add(itemViewModel);
                }
                foreach (var place in placeList)
                {
                    string s = place.Title.Substring(0, Math.Min(place.Title.Length, 30));
                    place.TitleSelection = s + "... >>";
                }
                IEnumerable<ItemViewModel> places = placeList.OrderBy(p => p.Area);
                CountyList.ItemsSource = GroupedCountyItems(places);
                placesGot = true;
            }
            
        }

        private static List<CountyKeyGroup<ItemViewModel>> GroupedCountyItems(IEnumerable<ItemViewModel> places)
        {
            return CountyKeyGroup<ItemViewModel>.CreateGroups(places,
                Thread.CurrentThread.CurrentUICulture,
                v => v.Area, true);
        }

        private void CountyLongListSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (CountyList.SelectedItem == null)
                return;

            // Navigate to the new page
            ItemViewModel p = CountyList.SelectedItem as ItemViewModel;
            NavigationService.Navigate(new Uri("/PlaceMap.xaml?selectedPlace=" + p.ID, UriKind.Relative));
           
            // Reset selected item to null (no selection)
            CountyList.SelectedItem = null;
        }

        
        private void National(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/National.xaml", UriKind.Relative));
        }

        private void Dublin(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Dublin.xaml", UriKind.Relative));
        }

        private void Parking(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Parking.xaml", UriKind.Relative));
        }

        private void RateReview(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }

        private void Main(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void About(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }
        private void DublinCams(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/DublinCams.xaml", UriKind.Relative));
        }
    }
}