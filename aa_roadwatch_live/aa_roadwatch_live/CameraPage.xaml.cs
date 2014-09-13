using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using aa_roadwatch_live.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Tasks;


namespace aa_roadwatch_live
{
    public partial class CameraPage : PhoneApplicationPage
    {
        private string imgUrl = "";
        private string cam = "";
        private string fav = "";
        private bool loading = true;

        public CameraPage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var selectedIndex = "";
            if (!NavigationContext.QueryString.TryGetValue("area", out selectedIndex)) return;
            TbkArea.Text = selectedIndex;
            TbkJunction.Text = NavigationContext.QueryString["junction"];
            cam = NavigationContext.QueryString["id"];
            fav = NavigationContext.QueryString["fav"];
            if (fav == "True")
            {
                tglFav.IsChecked = true;
            }
            imgUrl = NavigationContext.QueryString["image"];
            CamImage.Source = new BitmapImage(new Uri(imgUrl, UriKind.Absolute));
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

        private void TglFav_OnChecked(object sender, RoutedEventArgs e)
        {
            if (!loading)
            {
                //Add Favourite
                AddFav(cam, GetDeviceId());
            }

        }

        private void AddFav(string ca, string p)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers["Accept"] = "application/json";
                webClient.DownloadStringAsync(new Uri("http://selectunes.eu/api/trafficcam?cam=" + ca + "&unId=" + p));
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

        private void TglFav_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (!loading)
            {
                //Remove Favourite
                AddFav(cam, GetDeviceId());
            }

        }

        private void CamImage_OnLoaded(object sender, RoutedEventArgs e)
        {
            loading = false;
        }
    }
}