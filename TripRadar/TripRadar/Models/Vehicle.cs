using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TripRadar.Models
{
    public class Vehicle
    {
        [Key]
        public int VehicleId { get; set; }

        [Display(Name = "Make")]
        public string VehicleMake { get; set; }
        [Display(Name = "Model")]
        public string VehicleModel { get; set; }
        [Display(Name = "Year")]
        public int VehicleYear { get; set; }
        [Display(Name = "VehicleKey")]
        public int VehicleKey { get; set; }



    }
}