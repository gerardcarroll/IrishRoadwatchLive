using System;
using System.Collections.Generic;
using System.Linq;

namespace aa_roadwatch_live.Models
{
    public class ParkingCity
    {
        public Int32 Id { get; set; }
        public String Name { get; set; }

        public ParkingCity()
        {
            
        }

        public ParkingCity(int id, string nme)
        {
            Id = id;
            Name = nme;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}