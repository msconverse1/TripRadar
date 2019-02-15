using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripRadar.Models
{
    public class TripViewModel
    {
        public Trip Trip { get; set; }

        public Location StartLocation { get; set; }

        public Location EndLocation { get; set; }

        public User User { get; set; }
    }
}