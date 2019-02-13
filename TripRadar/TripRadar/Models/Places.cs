using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TripRadar.Models
{
    public class Places
    {
        [Key]
        public string RecommendedPlace { get; set; }
        public string LocationOfPlace { get; set; }
        public string Discription { get; set; }
        public double DistanceFromUser { get; set; }

        [ForeignKey("Trip")]
        [Display(Name = "trip")]
        public string trip { get; set; }
        public Trip Trip { get; set; }

    }
}