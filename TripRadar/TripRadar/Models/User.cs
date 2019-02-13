using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TripRadar.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string CurrentLocation { get; set; }


        [ForeignKey("Vehicle")]
        [Display(Name = "car")]
        public string CarID { get; set; }
        public Vehicle Vehicle { get; set; }

        [ForeignKey("Trip")]
        [Display(Name = "trip")]
        public string TripID { get; set; }
        public Trip Trip { get; set; }


    }
}