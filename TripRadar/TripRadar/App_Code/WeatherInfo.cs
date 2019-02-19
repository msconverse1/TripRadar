using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using TripRadar.Models;

namespace TripRadar.App_Code
{
    public class WeatherInfo
    {
        [WebMethod]
        public static  async Task JWeatherInfoAsync(Location location)
        {
            string weatherAPI = "4a219d24ec4bd8504123161859504e32";
            WebRequest request = WebRequest.Create($"http://api.openweathermap.org/data/2.5/weather?zip={location.ZipCode}&appid={weatherAPI}&units=metric");
            WebResponse response = await request.GetResponseAsync();


        }
    }
}