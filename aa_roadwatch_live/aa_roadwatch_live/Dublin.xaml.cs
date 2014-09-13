using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using aa_roadwatch_live.Models;
using aa_roadwatch_live.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace aa_roadwatch_live
{
    public partial class Dublin : PhoneApplicationPage
    {
        List<ItemViewModel> places = new List<ItemViewModel>();
        private bool placesGot = false;
        public Dublin()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!placesGot)
            {
                foreach (var itemViewModel in App.ViewModel.Items)
                {
                    places.Add(itemViewModel);
                }
                List<ItemViewModel> dublinList = places.Where(p => p.Area == "Dublin").ToList();
                foreach (var place in dublinList)
                {
                    place.Icon = GetSource(place);
                    string s = place.Title.Substring(0, Math.Min(place.Title.Length, 30));
                    place.TitleSelection = s + "... >>";
                }
                dublinList.OrderBy(p => p.UpdatedAt);
                DublinListSelector.ItemsSource = dublinList;
                placesGot = true;
            }
            
        }

        private BitmapImage GetSource(ItemViewModel p)
        {
            switch (p.IncidentTypeID)
            {
                case 1:
                    return new BitmapImage(new Uri("/Assets/mapControlIconWarn.png", UriKind.Relative));
                case 2:
                    return new BitmapImage(new Uri("/Assets/mapControlIconCar.png", UriKind.Relative));
                case 3:
                    return new BitmapImage(new Uri("/Assets/mapControlIconWork.png", UriKind.Relative));
                case 4:
                    return new BitmapImage(new Uri("/Assets/mapControlIconFlood.png", UriKind.Relative));
            }
            return null;
        }

        private void DublinListSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (DublinListSelector.SelectedItem == null)
                return;

            // Navigate to the new page
            ItemViewModel p = DublinListSelector.SelectedItem as ItemViewModel;
            NavigationService.Navigate(new Uri("/PlaceMap.xaml?selectedPlace=" + p.ID, UriKind.Relative));

            // Reset selected item to null (no selection)
            DublinListSelector.SelectedItem = null;
        }

        private void National(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/National.xaml", UriKind.Relative));
        }

        private void County(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/County.xaml", UriKind.Relative));
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