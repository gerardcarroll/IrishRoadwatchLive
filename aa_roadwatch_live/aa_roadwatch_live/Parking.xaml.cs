using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using aa_roadwatch_live.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json;

namespace aa_roadwatch_live
{
    public partial class Parking : PhoneApplicationPage
    {
        private bool _citiesLoaded;
        private bool carparksLoaded;
        List<ParkingCity>  cities = new List<ParkingCity>();
        List<Carpark> carparks = new List<Carpark>(); 
        public Parking()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!_citiesLoaded)
            {
                SystemTray.ProgressIndicator = new ProgressIndicator { Text = "Loading" };
                SetProgressIndicator(true);
                LoadCities();
            }
        }

        private void LoadCities()
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers["Accept"] = "application/json";
                webClient.DownloadStringCompleted += WebClientDownloadStringCompleted;
                webClient.DownloadStringAsync(new Uri("http://selectunes.eu/api/parking"));
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
                cities.Clear();
                cities.Add(new ParkingCity
                           {
                               Id = 0,
                               Name = "Select City..."
                           });

                if (e.Result == null) return;
                var places = JsonConvert.DeserializeObject<List<ParkingCity>>(e.Result);
                foreach (var p in places)
                {
                    cities.Add(new ParkingCity
                               {
                                   Id = p.Id,
                                   Name = p.Name
                               });

                }
                ToolkitListPicker.SetValue(ListPicker.ItemCountThresholdProperty, 10);
                ToolkitListPicker.ItemsSource = cities;
                _citiesLoaded = true;
                SetProgressIndicator(false);
            }
            catch (Exception ex)
            {
                StackFrame stackFrame = new StackFrame();
                MethodBase methodBase = stackFrame.GetMethod();
                Error.PostToDB(methodBase.Name, ex.Message, ex.ToString());
            }
        }

        private static void SetProgressIndicator(bool value)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = value;
            SystemTray.ProgressIndicator.IsVisible = value;
        }

        private void ToolkitListPicker_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParkingCity pc = ToolkitListPicker.SelectedItem as ParkingCity;
            if (pc == null) return;
            if (pc.Id == 0) return;
            CarParkList.ItemsSource = null;
            SetProgressIndicator(true);
            SystemTray.ProgressIndicator.Text = "Loading Car Parks";
            GetCarParks(pc.Id, pc.Name);
        }

        private void GetCarParks(int p1, string p2)
        {
            try
            {
                WebClient webClient2 = new WebClient();
                webClient2.Headers["Accept"] = "application/json";
                webClient2.DownloadStringCompleted += WebClient2DownloadStringCompleted;
                webClient2.DownloadStringAsync(new Uri("http://selectunes.eu/api/parking?name=" + p2 + "&id=" + p1 ));
            }
            catch (Exception ex)
            {
                StackFrame stackFrame = new StackFrame();
                MethodBase methodBase = stackFrame.GetMethod();
                Error.PostToDB(methodBase.Name, ex.Message, ex.ToString());
            }
        }

        private void WebClient2DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                carparks.Clear();
                
                if (e.Result == null) return;
                var places = JsonConvert.DeserializeObject<List<Carpark>>(e.Result);
                foreach (var cp in places)
                {
                    carparks.Add(new Carpark
                    {
                        Id = cp.Id,
                        Name = cp.Name,
                        Access = CleanString(cp.Access),
                        City =  cp.City,
                        CityId = cp.CityId,
                        District = cp.District,
                        Hours = CleanString(cp.Hours),
                        Location = CleanString(cp.Location),
                        Tarrifs = CleanString(cp.Tarrifs),
                        Url =  cp.Url
                    });

                }

                CarParkList.ItemsSource = carparks;
                carparksLoaded = true;
                SetProgressIndicator(false);
            }
            catch (Exception ex)
            {
                StackFrame stackFrame = new StackFrame();
                MethodBase methodBase = stackFrame.GetMethod();
                Error.PostToDB(methodBase.Name, ex.Message, ex.ToString());
            }
        }

        public string CleanString(string p)
        {
            string clean = "";
            clean = p.Replace("\n ", "\n");

            return clean;
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