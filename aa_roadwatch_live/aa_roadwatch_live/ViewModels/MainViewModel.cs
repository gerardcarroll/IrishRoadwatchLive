using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using aa_roadwatch_live.Models;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;

namespace aa_roadwatch_live.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private const string apiUrl = "http://selectunes.eu/api/test";

        public MainViewModel()
        {
            Items = new ObservableCollection<ItemViewModel>();
        }

        public ObservableCollection<ItemViewModel> Items { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData()
        {
            if (IsDataLoaded == false)
            {
                SystemTray.ProgressIndicator = new ProgressIndicator { Text = "Loading" };
                SetProgressIndicator(true);
                Items.Clear();
                try
                {
                    WebClient webClient = new WebClient();
                    webClient.Headers["Accept"] = "application/json";
                    webClient.DownloadStringCompleted += WebClientDownloadStringCompleted;
                    webClient.DownloadStringAsync(new Uri(apiUrl));
                }
                catch (Exception ex)
                {
                    StackFrame stackFrame = new StackFrame();
                    MethodBase methodBase = stackFrame.GetMethod();
                    Error.PostToDB(methodBase.Name, ex.Message, ex.ToString());
                }
            }
        }

        private static void SetProgressIndicator(bool value)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = value;
            SystemTray.ProgressIndicator.IsVisible = value;
        }

        private void WebClientDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Result == null) return;
                var places = JsonConvert.DeserializeObject<List<ItemViewModel>>(e.Result);
                foreach (var p in places)
                {
                    Items.Add(new ItemViewModel()
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

                foreach (var place in Items)
                {
                    place.LastUpdate = "Updated: " + place.UpdatedAt.ToString("f");
                    place.Icon = GetSource(place);
                    place.Coordinate = new GeoCoordinate(place.Latitude, place.Longitude);
                    
                }
                //myMap.SetView(new GeoCoordinate(53.510138, -7.865643), 7.2);
            }
            catch (Exception ex)
            {
                StackFrame stackFrame = new StackFrame();
                MethodBase methodBase = stackFrame.GetMethod();
                Error.PostToDB(methodBase.Name, ex.Message, ex.ToString());
            }
            
            SetProgressIndicator(false);
            IsDataLoaded = true;
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
