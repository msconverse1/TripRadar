using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TripRadar.Models
{
    public class Location
    {
        [Key]
        public int ID { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        [Display(Name = "ZipCode")]
        public int ZipCode { get; set; }

        public string AddressString { get { return StreetName + " " + City + " " + State + " " + ZipCode; } }

    }
}