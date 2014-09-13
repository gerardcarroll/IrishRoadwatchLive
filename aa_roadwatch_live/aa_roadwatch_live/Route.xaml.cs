using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Shell;

namespace aa_roadwatch_live
{
    public partial class Route : PhoneApplicationPage
    {
        GeoCoordinate MyCoordinate = null;
        GeocodeQuery MyGeocodeQuery = null;
        private List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();
        bool _isRouteSearch = false; // True when route is being searched, otherwise false
        public Route()
        {
            InitializeComponent();
            SearchForTerm("Vicarstown Laois");
        }

        private void SearchForTerm(String searchTerm)
        {

            MyGeocodeQuery = new GeocodeQuery();
            MyGeocodeQuery.SearchTerm = searchTerm;
            MyGeocodeQuery.GeoCoordinate = MyCoordinate == null ? new GeoCoordinate(0, 0) : MyCoordinate;
            MyGeocodeQuery.QueryCompleted += GeocodeQuery_QueryCompleted;
            MyGeocodeQuery.QueryAsync();
        }

        private void GeocodeQuery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {

            if (e.Error == null)
            {
                if (e.Result.Count > 0)
                {
                    if (_isRouteSearch) // Query is made to locate the destination of a route
                    {

                    }
                    else // Query is made to search the map for a keyword
                    {
                        // Add all results to MyCoordinates for drawing the map markers
                        for (int i = 0; i < e.Result.Count; i++)
                        {
                            MyCoordinates.Add(e.Result[i].GeoCoordinate);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No match found. Narrow your search e.g. Seattle WA.");
                }
            }
        }
    }
}