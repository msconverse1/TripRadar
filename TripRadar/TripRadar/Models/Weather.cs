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
        public float mainTemp { get; set; }
        [Display(Name = "Wind Speed")]
        public float speedvalue { get; set; }
        [Display(Name = "Wind Name")]
        public string windName { get; set; }
        [Display(Name = "Could Cover")]
        public float cloudValue { get; set; }
        [Display(Name = "Cloud Name")]
        public string cloudName { get; set; }
        [Display(Name = "Precipitation Value")]
        public float precipitationValue { get; set; }
        [Display(Name = "Precipitation Name")]
        public string precipitationName { get; set; }
    }
}