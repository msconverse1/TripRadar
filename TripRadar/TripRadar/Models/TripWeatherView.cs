﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripRadar.Models
{
    public class TripWeatherView
    {
        public Trip Trip { get; set; }
        public Weather Weather {get;set;}

    }
}