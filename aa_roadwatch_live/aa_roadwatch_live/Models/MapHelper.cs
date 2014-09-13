using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace aa_roadwatch_live.Models
{
    public static class MapHelper
    {
        public static bool IsVisiblePoint(this Map map, Point point)
        {
            return point.X > 0 && point.X < map.ActualWidth && point.Y > 0 && point.Y < map.ActualHeight;
        }
    }
}
