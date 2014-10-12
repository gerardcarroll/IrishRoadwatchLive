using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using aa_roadwatch_live.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json;

namespace aa_roadwatch_live
{
    public partial class National : PhoneApplicationPage
    {
        List<SummaryBlock> blocks = new List<SummaryBlock>();
        private bool _blocksLoaded;
        public National()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!_blocksLoaded)
            {
                GetNationalSummary();
            }
        }

        private void GetNationalSummary()
        {
            SystemTray.ProgressIndicator = new ProgressIndicator { Text = "Loading" };
            SetProgressIndicator(true);
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers["Accept"] = "application/json";
                webClient.DownloadStringCompleted += WebClientDownloadStringCompleted;
                //webClient.DownloadStringAsync(new Uri("http://selectunes.eu/api/nationalsummary"));
                webClient.DownloadStringAsync(new Uri("http://selectunes.eu/api/testnationalsum"));
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
                bool maxed = false;
                DateTime date;
                blocks.Clear();
                if (e.Result == null) return;
                var blockList = JsonConvert.DeserializeObject<List<SummaryBlock>>(e.Result);
                DateTime.TryParse(blockList[0].Title, out date);
                TbkUpdated.Text = "Last Updated: " + date.ToString("dddd dd-MMM HH:mm");
                blockList.RemoveAt(0);
                foreach (SummaryBlock b in blockList)
                {
                    //if (DateTime.TryParse(b.Title, out date))
                    //{
                    //    TbkUpdated.Text = "Last Updated: " + date.ToString("");
                    //    continue;
                    //}
                    bool empty = true;
                    int CharCount = 0;
                    int blockCount = 0;
                    foreach (String s in b.Paragraph)
                    {
                        if (!maxed)
                        {
                            blockCount++;
// ReSharper disable once UnusedVariable
                            foreach (char c in s)
                            {
                                if (CharCount < 2700)
                                {
                                    CharCount++;
                                }
                                else
                                {
                                    maxed = true;
                                }

                            }
                        }
                        else
                        {
                            break;
                        }
                        
                    }
                    if (CharCount == 2700)
                    {
                        if (blockCount % 2 != 0)
                        {
                            blockCount--;
                        }
                        List<String> firstGroup = new List<string>();
                        for (int i = 0; i < blockCount; i++)
                        {
                            firstGroup.Add(b.Paragraph[i]);
                        }
                        blocks.Add(new SummaryBlock
                                   {
                                       Title = b.Title,
                                       Line = GetLines(firstGroup)
                                   });
                        List<String> secondGroup = new List<string>();
                        for (int i = blockCount; i < b.Paragraph.Count; i++)
                        {
                            empty = false;
                            secondGroup.Add(b.Paragraph[i]);
                        }
                        if (!empty)
                        {
                            blocks.Add(new SummaryBlock
                            {
                                Title = b.Title + " (contd)",
                                Line = GetLines(secondGroup)
                            });
                        }
                        maxed = false;
                    }
                    else
                    {
                        blocks.Add(new SummaryBlock
                        {
                            Title = b.Title,
                            Line = GetLines(b.Paragraph)
                        });
                    }
                    
                }

                _blocksLoaded = true;
                NationalListSelector.ItemsSource = blocks;
            }
            catch (Exception ex)
            {
                StackFrame stackFrame = new StackFrame();
                MethodBase methodBase = stackFrame.GetMethod();
                Error.PostToDB(methodBase.Name, ex.Message, ex.ToString());
            }
            SetProgressIndicator(false);
        }

        private string GetLines(List<string> list)
        {
            string line = "";
            foreach (var str in list)
            {
                if (IsAllUpper(str))
                {
                    line += str + "\n";
                }
                else
                {
                    line += str + "\n\n";
                }
            }

            return line;
        }
        private bool IsAllUpper(string input)
        {
            return input.All(t => !Char.IsLetter(t) || Char.IsUpper(t));
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

        private void RateReview(object sender, EventArgs e)
        {
            MarketplaceReviewTask marketplaceReviewTask = new MarketplaceReviewTask();

            marketplaceReviewTask.Show();
        }

        private void Refresh(object sender, EventArgs e)
        {
            GetNationalSummary();
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