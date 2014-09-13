using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;

namespace aa_roadwatch_live
{
    public class Camera
    {
        public Int32 Id { get; set; }
        public String Area { get; set; }
        public String Junction { get; set; }
        public String Url { get; set; }
        public BitmapImage CamImage { get; set; }
        public Boolean Fav { get; set; }
    }
}