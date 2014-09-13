using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace aa_roadwatch_live.Models
{
    public class Place
    {
        public String Area { get; set; }
        public Int32 ID { get; set; }
        public Int32 IncidentTypeID { get; set; }
        public Double Latitude { get; set; }
        public String Location { get; set; }
        public Double Longitude { get; set; }
        public String Report { get; set; }
        public String Title { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Int32 ZoomLevel { get; set; }
        public GeoCoordinate Coordinate { get; set; }
        public BitmapImage Icon { get; set; }
        public String LastUpdate { get; set; }
        public String TitleSelection { get; set; }
    }
}
