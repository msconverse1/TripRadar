using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TripRadar.Models
{
    public class Weather
    {

<<<<<<< HEAD
=======
        [Key]
        public int WeatherId { get; set; }
        [Display(Name = "Tempature")]
        public float MainTemp { get; set; }
        [Display(Name = "Wind Speed")]
        public float Speedvalue { get; set; }
        [Display(Name = "Wind Name")]
        public string WindName { get; set; }
        [Display(Name = "Could Cover")]
        public float CloudValue { get; set; }
        [Display(Name = "Cloud Name")]
        public string CloudName { get; set; }
        [Display(Name = "Precipitation Value")]
        public float PrecipitationValue { get; set; }
        [Display(Name = "Precipitation Name")]
        public string PrecipitationName { get; set; }
>>>>>>> b63abc371e0523229dda26f424c4ab272e16b95f
    }
}