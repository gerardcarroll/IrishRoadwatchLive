using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using aa_roadwatch_live.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json;

namespace aa_roadwatch_live
{
    public partial class DublinCams : PhoneApplicationPage
    {
        private bool camsGot = false;
        List<Camera> cameras = new List<Camera>(); 
        public DublinCams()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!camsGot)
            {
                GetCams();
            }
        }

        private void GetCams()
        {
            SystemTray.ProgressIndicator = new ProgressIndicator { Text = "Loading" };
            SetProgressIndicator(true);
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers["Accept"] = "application/json";
                webClient.DownloadStringCompleted += WebClientDownloadStringCompleted;
                webClient.DownloadStringAsync(new Uri("http://selectunes.eu/api/trafficcam?id=" + GetDeviceId()));
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
            string sl = "";
            try
            {
                CameraList.ItemsSource = null;
                cameras.Clear();
                if (e.Result == null) return;
                var camList = JsonConvert.DeserializeObject<List<Camera>>(e.Result);
                foreach (var camera in camList)
                {
                    camera.CamImage = new BitmapImage(new Uri(camera.Url, UriKind.Absolute));
                    cameras.Add(camera);
                }
                CameraList.ItemsSource = GroupedCameraItems(cameras);
                SetProgressIndicator(false);
                camsGot = true;
            }
            catch (Exception ex)
            {
                StackFrame stackFrame = new StackFrame();
                MethodBase methodBase = stackFrame.GetMethod();
                Error.PostToDB(methodBase.Name, ex.Message, ex.ToString());
            }
        }
        
        private static string GetDeviceId()
        {
            var id = (byte[])DeviceExtendedProperties.GetValue("DeviceUniqueId");
            return BitConverter.ToString(id).Replace("-", string.Empty);
        }

        private static List<StringKeyGroup<Camera>> GroupedCameraItems(IEnumerable<Camera> cameras)
        {
            return StringKeyGroup<Camera>.CreateGroups(cameras,
                Thread.CurrentThread.CurrentUICulture,
                v => v.Area, true);
        }

        private static void SetProgressIndicator(bool value)
        {
            SystemTray.ProgressIndicator.IsIndeterminate = value;
            SystemTray.ProgressIndicator.IsVisible = value;
        }

        private void Dublin(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Dublin.xaml", UriKind.Relative));
        }

        private void County(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/County.xaml", UriKind.Relative));
        }

        private void Parking(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Parking.xaml", UriKind.Relative));
        }

        private void National(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/National.xaml", UriKind.Relative));
        }
        private void RateReview(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }

        private void Refresh(object sender, EventArgs e)
        {
            GetCams();
        }

        private void Main(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void About(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void CameraLongListSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected item is null (no selection) do nothing
            if (CameraList.SelectedItem == null)
                return;

            // Navigate to the new page
            Camera c = CameraList.SelectedItem as Camera;
            NavigationService.Navigate(new Uri("/CameraPage.xaml?area=" + c.Area + "&junction=" + c.Junction + "&image=" + c.Url + "&id=" + c.Id + "&fav=" + c.Fav, UriKind.Relative));

            // Reset selected item to null (no selection)
            CameraList.SelectedItem = null;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            GetCams();
        }
    }
}