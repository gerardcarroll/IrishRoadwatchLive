using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Windows.Devices.Geolocation;
using aa_roadwatch_live.Models;
using aa_roadwatch_live.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json;
using GestureEventArgs = System.Windows.Input.GestureEventArgs;

namespace aa_roadwatch_live
{
    public partial class MainPage : PhoneApplicationPage
    {
        private bool _firstOpened = true;
        List<Place> placeList = new List<Place>();
        private DispatcherTimer clock;
        TimeSpan t = new TimeSpan();
        private double userLat;
        private double userLong;
        bool locationNotSet = false;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
           // myMap.DataContext = App.ViewModel;
            ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(myMap);
            var obj = children.FirstOrDefault(x => x is MapItemsControl) as MapItemsControl;

            if (obj != null) obj.ItemsSource = App.ViewModel.Items;
            //myMap.SetView(new GeoCoordinate(53.510138, -7.865643), 7.2);

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (App.ViewModel.IsDataLoaded) return;
                
                SystemTray.ProgressIndicator = new ProgressIndicator {Text = "Connecting"};
                SetProgressIndicator(true);
                var isNetwork = NetworkInterface.GetIsNetworkAvailable();
                if (isNetwork)
                {
                    StartDownload();
                }
                else
                {
                    clock = new DispatcherTimer();
                    clock.Interval = new TimeSpan(0, 0, 0, 1);
                    clock.Tick += clock_Tick;
                    t = TimeSpan.Parse("00:00:10");
                    clock.Start();
                }

            }
            catch (Exception ex)
            {
                var stackFrame = new StackFrame();
                var methodBase = stackFrame.GetMethod();
                Error.PostToDB(methodBase.Name, ex.Message, ex.ToString());
            }
        }

        private void clock_Tick(object sender, EventArgs e)
        {
            t = t.Subtract(new TimeSpan(0, 0, 0, 1));

            if (NetworkInterface.GetIsNetworkAvailable())
            {
                SetProgressIndicator(false);
                clock.Stop();
                StartDownload();
            }
            else if (t == TimeSpan.Parse("00:00:00"))
            {
                SetProgressIndicator(false);
                clock.Stop();
                MessageBox.Show("Internet Connection Unavailable");
                Application.Current.Terminate();
            }
        }

        private void StartDownload()
        {
            App.ViewModel.LoadData();
            //LoadPlaces();
            if (!_firstOpened) return;
            var deviceId = GetDeviceId();
            string s = DeviceStatus.DeviceManufacturer + " " + DeviceStatus.DeviceName;
            Error.UserID(deviceId, s);
            _firstOpened = false;
            //ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(myMap);
            //var obj = children.FirstOrDefault(x => x is MapItemsControl) as MapItemsControl;

            //if (obj != null) obj.ItemsSource = App.ViewModel.Items;
            //myMap.SetView(new GeoCoordinate(53.510138, -7.865643), 7.2);
            GetLocation();
        }
        private void myMapControl_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "7ce2bde2-f7f8-4127-8e4b-c622b9383d08";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "Ed8A3ht3sMcbTtfzeubVKA";
        }

        private static string GetDeviceId()
        {
            var id = (byte[])DeviceExtendedProperties.GetValue("DeviceUniqueId");
            return BitConverter.ToString(id).Replace("-", string.Empty);
        }

        private static void SetProgressIndicator(bool value)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = value;
            SystemTray.ProgressIndicator.IsVisible = value;
        }

        private void LoadPlaces()
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers["Accept"] = "application/json";
                webClient.DownloadStringCompleted += WebClientDownloadStringCompleted;
                webClient.DownloadStringAsync(new Uri("http://selectunes.eu/api/test"));
            }
            catch (Exception ex)
            {
                StackFrame stackFrame = new StackFrame();
                MethodBase methodBase = stackFrame.GetMethod();
                Error.PostToDB(methodBase.Name, ex.Message, ex.ToString());
            }
        }

        private void WebClientDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                placeList.Clear();
                if (e.Result == null) return;
                var places = JsonConvert.DeserializeObject<List<Place>>(e.Result);
                foreach (Place p in places)
                {
                    placeList.Add(new Place
                                  {
                                      Area = p.Area,
                                      IncidentTypeID = p.IncidentTypeID,
                                      ID = p.ID,
                                      Latitude = p.Latitude,
                                      Location = p.Location,
                                      Longitude = p.Longitude,
                                      Report = p.Report,
                                      Title = p.Title,
                                      UpdatedAt = p.UpdatedAt,
                                      ZoomLevel = p.ZoomLevel
                                  });
                    
                }
                PopulateMap(placeList);
                //_placesLoaded = true;
                SetProgressIndicator(false);
                
            }
            catch (Exception ex)
            {
                StackFrame stackFrame = new StackFrame();
                MethodBase methodBase = stackFrame.GetMethod();
                Error.PostToDB(methodBase.Name, ex.Message, ex.ToString());
            }
        }

        private async void GetLocation()
        {
            SystemTray.ProgressIndicator = new ProgressIndicator { Text = "Getting Location" };
            SetProgressIndicator(true);

            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                userLat = geoposition.Coordinate.Latitude;
                userLong = geoposition.Coordinate.Longitude;
                ZoomToLocation.IsEnabled = true;
                // Create a small circle to mark the current location.
                Ellipse myCircle = new Ellipse();
                myCircle.Fill = new SolidColorBrush(Colors.Blue);
                myCircle.Height = 20;
                myCircle.Width = 20;
                myCircle.Opacity = 50;
                // Create a MapOverlay to contain the circle.
                MapOverlay myLocationOverlay = new MapOverlay();
                myLocationOverlay.Content = myCircle;
                myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
                myLocationOverlay.GeoCoordinate = new GeoCoordinate(userLat, userLong);
                // Create a MapLayer to contain the MapOverlay.
                MapLayer myLocationLayer = new MapLayer();
                myLocationLayer.Add(myLocationOverlay);
                // Add the MapLayer to the Map.
                myMap.Layers.Add(myLocationLayer);
                SetProgressIndicator(false);
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    //MessageBox.Show("Location is disabled in phone settings.");
                    locationNotSet = true;
                }
                //else
                {
                    // something else happened acquring the location
                }
            }
            
        }

        private void PopulateMap(List<Place> placeList)
        {
            //var pushpinTemplate = Resources["PushpinTemplate"] as DataTemplate;
            //var pushpins = new List<Pushpin>();

            foreach (var place in placeList)
            {
                place.LastUpdate = "Updated: " + place.UpdatedAt.ToString("f");
                place.Icon = GetSource(place);
                place.Coordinate = new GeoCoordinate(place.Latitude, place.Longitude);
                //Pushpin pp = new Pushpin
                //             {
                //                 GeoCoordinate = new GeoCoordinate(place.Latitude, place.Longitude),
                //                 ContentTemplate = pushpinTemplate,
                //                 Content = place.Icon
                //             };
                //pushpins.Add(pp);
            }

            ObservableCollection<DependencyObject> children = MapExtensions.GetChildren(myMap);
            var obj = children.FirstOrDefault(x => x is MapItemsControl) as MapItemsControl;

            if (obj != null) obj.ItemsSource = App.ViewModel.Items;
            //var clusterer = new ClustersGenerator(myMap, pushpins, Resources["ClusterTemplate"] as DataTemplate);
            myMap.SetView(new GeoCoordinate(53.510138, -7.865643), 7.0);
        }

        private BitmapImage GetSource(Place p)
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

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
        private void Pushpin_OnTap(object sender, GestureEventArgs e)
        {
            var ppmodel = sender as Pushpin;
            //Place p = placeList.FirstOrDefault(pl => ppmodel != null && pl.Coordinate == ppmodel.GeoCoordinate);
            ItemViewModel p = App.ViewModel.Items.FirstOrDefault(pl => ppmodel != null && pl.Coordinate == ppmodel.GeoCoordinate);
            //myMap.SetView(new GeoCoordinate(p.Latitude, p.Longitude), 12);
            ContextMenu contextMenu = ContextMenuService.GetContextMenu(ppmodel);
            contextMenu.DataContext = App.ViewModel.Items.FirstOrDefault(c => ppmodel != null && (c.Coordinate == ppmodel.GeoCoordinate));

            if (contextMenu.Parent == null)
            {
                contextMenu.IsOpen = true;
            }
        }

        private void Refresh(object sender, EventArgs e)
        {
            LoadPlaces();
        }

        private void National(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/National.xaml", UriKind.Relative));
        }
        private void County(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/County.xaml", UriKind.Relative));
        }
        private void Dublin(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Dublin.xaml", UriKind.Relative));
        }
        private void Parking(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Parking.xaml", UriKind.Relative));
        }

        private void MyMap_OnTap(object sender, GestureEventArgs e)
        {
            //myMap.SetView(new GeoCoordinate(53.510138, -7.865643), 7.2);
        }

        private void Route(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Route.xaml", UriKind.Relative));
        }

        private void RateReview(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }
        private void DublinCams(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/DublinCams.xaml", UriKind.Relative));
        }

        private void About(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void ZoomToLocation_OnClick(object sender, RoutedEventArgs e)
        {
            if (ZoomToLocation.Content.ToString() == "My Location")
            {
                if (!locationNotSet)
                {
                    //// Create a small circle to mark the current location.
                    //Ellipse myCircle = new Ellipse();
                    //myCircle.Fill = new SolidColorBrush(Colors.Blue);
                    //myCircle.Height = 20;
                    //myCircle.Width = 20;
                    //myCircle.Opacity = 50;
                    //// Create a MapOverlay to contain the circle.
                    //MapOverlay myLocationOverlay = new MapOverlay();
                    //myLocationOverlay.Content = myCircle;
                    //myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
                    //myLocationOverlay.GeoCoordinate = new GeoCoordinate(userLat, userLong);
                    //// Create a MapLayer to contain the MapOverlay.
                    //MapLayer myLocationLayer = new MapLayer();
                    //myLocationLayer.Add(myLocationOverlay);
                    //// Add the MapLayer to the Map.
                    //myMap.Layers.Add(myLocationLayer);
                    myMap.SetView(new GeoCoordinate(userLat, userLong), 12);
                    ZoomToLocation.Content = "View All";
                }
            }
            else
            {
                myMap.SetView(new GeoCoordinate(53.510138, -7.865643), 7.0);
                ZoomToLocation.Content = "My Location";
            }
            
        }
    }
}