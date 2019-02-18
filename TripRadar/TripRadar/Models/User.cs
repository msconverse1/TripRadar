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
        public int? VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }

        //[ForeignKey("Trip")]
        //[Display(Name = "TripID")]
        //public int? TripID { get; set; }
        //public Trip Trip { get; set; }


        [ForeignKey("ApplicationUser")]
        [Display(Name = "UserId")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }


    }
}