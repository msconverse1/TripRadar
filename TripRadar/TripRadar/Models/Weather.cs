using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TripRadar.Models
{
    public class Weather
    {


        [Key]
        public int WeatherId { get; set; }
        [Display(Name = "Tempature")]
        public float MainTemp { get; set; }
        [Display(Name = "Wind Speed")]
        public float Speedvalue { get; set; }
        [Display(Name = "Wind Direction")]
        public float WindDeg { get; set; }
        [Display(Name = "Cloudd Cover")]
        public float CloudValue { get; set; }
        [Display(Name = "Weather Description")]
        public string TypeOfSkys { get; set; }
        [Display(Name = "Humidity")]
        public float Humidity { get; set; }
        [Display(Name ="DateTime")]
        public DateTime DateTime { get; set; }
    }
}