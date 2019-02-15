using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripRadar.Models
{
    public class Email
    {

        public string ToLine { get; set; }
        public string FromLine { get; set; }
        public string Subject { get; set; }
        public string  Body { get; set; }
    }
}