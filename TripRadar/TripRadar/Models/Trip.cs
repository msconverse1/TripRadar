using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TripRadar.Models
{
    public class Trip
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Start Location")]
        public string StartLocation { get; set; }

        [Display(Name = "End Location")]
        public string EndLocation { get; set; }

        public string Name { get; set; }
    }
}