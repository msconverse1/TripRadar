using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TripRadar.Models
{
    public class Trip
    {

        [Key]
        public int TripID { get; set; }

        [Display(Name = "Start Location")]
        public string StartLocation { get; set; }

        [Display(Name = "End Location")]
        public string EndLocation { get; set; }

        public string Name { get; set; }

        public string TripTime { get; set; }
        public string TripDistance { get; set; }

        [ForeignKey("Weather")]
        [Display(Name = "Weather")]
        public int WeatherID { get; set; }

        public Weather Weather { get; set; }
    }
}