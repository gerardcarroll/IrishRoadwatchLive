using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Phone.Info;

namespace aa_roadwatch_live.Models
{
    class Error
    {
        public static void PostToDB(string p1, string p2, string p3)
        {
            WebClient client = new WebClient();

            client.DownloadStringAsync(new Uri("http://selectunes.eu/api/roadwatcherror?method=" + p1 + "&exception=" + p2 + "&full_ex=" + p3 + "&model=" + DeviceStatus.DeviceName));
            
            client.DownloadStringCompleted +=client_DownloadStringCompleted;
        }

        private static void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            MessageBox.Show("Error Occurred. Information on this error has been sent to the Developer");
        }

        public static void UserID(string p1, string p2)
        {
            WebClient client = new WebClient();

            client.DownloadStringAsync(new Uri("http://selectunes.eu/api/roadwatchuser?id=" + p1 + "&model=" + p2));

            client.DownloadStringCompleted += client_UserDownloadStringCompleted;
        }

        private static void client_UserDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            string version = "\"1120\"";
            try
            {
                if (e.Result != version)
                {
                    MessageBoxResult res = MessageBox.Show("Update Available.\r\nUpdate Now?", "Update Available", MessageBoxButton.OKCancel);
                    if (res == MessageBoxResult.OK)
                    {
                        Microsoft.Phone.Tasks.WebBrowserTask wbt = new Microsoft.Phone.Tasks.WebBrowserTask();
                        wbt.Uri = new Uri("http://www.windowsphone.com/s?appid=7ce2bde2-f7f8-4127-8e4b-c622b9383d08");
                        wbt.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                var stackFrame = new StackFrame();
                var methodBase = stackFrame.GetMethod();
                PostToDB(methodBase.Name, ex.Message, ex.ToString());
            }
            
        }

        
        
    }
}
