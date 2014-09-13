using System;
using System.Collections.Generic;
using System.Linq;

namespace aa_roadwatch_live.Models
{
    public class Carpark
    {
        public Int32 Id { get; set; }
        public String City { get; set; }
        public String District { get; set; }
        public String Name { get; set; }
        public String Location { get; set; }
        public String Url { get; set; }
        public String Access { get; set; }
        public String Hours { get; set; }
        public String Tarrifs { get; set; }
        public Int32 CityId { get; set; }
    }
}