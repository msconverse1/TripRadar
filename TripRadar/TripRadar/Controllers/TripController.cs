using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using TripRadar.Models;

namespace TripRadar.Controllers
{
    public class TripController : Controller
    {
        ApplicationDbContext db;

        public TripController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Trip
        public ActionResult Index()
        {
            return View();
        }

        // GET: Trip/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Trip/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Trip/Create
        [HttpPost]
        public ActionResult Create(Trip trip)
        {
            try
            {
                Trip newTrip = new Trip();
                newTrip.StartLocation = trip.StartLocation;
                newTrip.EndLocation = trip.EndLocation;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Trip/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Trip/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Trip/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Trip/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public async Task<ActionResult> WeatherInfo(int? id)
        {
            string weatherAPI = "4a219d24ec4bd8504123161859504e32";
            // User driver = db.User.Where(u => u.UserId == id).FirstOrDefault();
            //  var zipcode= driver.Trip.Location.ZipCode;
            ////   string urlforzipcode = $"http://api.openweathermap.org/data/2.5/forecast?zip={53202}" +
            // $"&mode=json&units=metric&APPID={weatherAPI}";
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("http://api.openweathermap.org");
                var response = await client.GetAsync($"/data/2.5/weather?lat={43.03}&lon={-87.92}&appid={weatherAPI}&units=metric");
                response.EnsureSuccessStatusCode();


                var stringResult = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(stringResult);
                var j_weatherDesc = json["weather"]["description"];
                var j_humidity = json["main"]["humidity"];
                var j_cloudcover = json["clouds"]["all"];
                var j_datetimeUinx = json["dt"];
                var j_tempature = json["main"]["temp"];
                var j_windspeed = json["wind"]["speed"];
                var j_windDirection = json["wind"]["deg"];
                var WeatherDesc = j_weatherDesc.ToObject<string>();
                var Humidity = j_humidity.ToObject<float>();
                var CloudCover = j_cloudcover.ToObject<float>();
                var datetimeUnix = j_datetimeUinx.ToObject<double>();
                var Tempature = j_tempature.ToObject<float>();
                var WindSpeed = j_windspeed.ToObject<float>();
                var WindDegs = j_windDirection.ToObject<float>();
                Tempature = (Tempature * 1.8f) + 32;
                System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                dateTime = dateTime.AddSeconds(datetimeUnix);
                Weather weather = new Weather()
                {
                    MainTemp = Tempature,
                    Speedvalue = WindSpeed,
                    CloudValue = CloudCover,
                    WindDeg = WindDegs,
                    Humidity = Humidity,
                    TypeOfSkys = WeatherDesc,
                    DateTime = dateTime
                };

            }
            return RedirectToAction("Index");
        }
    }
}
