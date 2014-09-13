using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using aa_roadwatch_live.Models;
using aa_roadwatch_live.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace aa_roadwatch_live
{
    public partial class PlaceMap : PhoneApplicationPage
    {
        ItemViewModel p = new ItemViewModel();
        
        private bool settingView;
        public PlaceMap()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataContext != null) return;
            
            var selectedIndex = "";
            if (!NavigationContext.QueryString.TryGetValue("selectedPlace", out selectedIndex)) return;
            int i = Convert.ToInt32(selectedIndex);
            p = (from pl in App.ViewModel.Items
                                    where pl.ID == i
                                    select pl).FirstOrDefault();
            PopulateMap(p);
        }

        private void PopulateMap(ItemViewModel pl)
        {
            pl.LastUpdate = "Updated: " + pl.UpdatedAt.ToString("f");
            pl.Icon = GetSource(pl);
            pl.Coordinate = new GeoCoordinate(pl.Latitude, pl.Longitude);

            List<ItemViewModel> places = new List<ItemViewModel>();
            places.Add(pl);
            ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(myMap);
            MapItemsControl obj = new MapItemsControl();
            obj.Items.Clear();
            obj.ItemsSource = null;
            obj = children.FirstOrDefault(x => x is MapItemsControl) as MapItemsControl;

            if (obj != null && obj.Items.Count == 0)
            {
                obj.ItemsSource = places;
                TbkTitle.Text = pl.Title;
                TbkReport.Text = pl.Report;
                TbkUpdated.Text = "Updated: " + pl.UpdatedAt.ToString("f");
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

        private void myMapControl_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "7ce2bde2-f7f8-4127-8e4b-c622b9383d08";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "Ed8A3ht3sMcbTtfzeubVKA";
        }
        
        private void MyMap_OnZoomLevelChanged(object sender, MapZoomLevelChangedEventArgs e)
        {
            if (settingView) return;
            myMap.SetView(new GeoCoordinate(p.Latitude, p.Longitude), 14);
            settingView = true;
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

        private void Dublin(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Dublin.xaml", UriKind.Relative));
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