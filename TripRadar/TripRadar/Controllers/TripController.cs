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
            var AllTrips = db.Trips.ToList();

            return View(AllTrips);
        }

        // GET: Trip/ViewTrip/5
        public ActionResult ViewTrip(int id)
        {
            var SeeMyTrip = db.Trips.Where(t => t.TripID == id).SingleOrDefault();

            return View(SeeMyTrip);
        }


        [HttpPost]
        public ActionResult ViewTrip()
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
        public ActionResult Create(TripViewModel model)
        {
            try
            {
                Trip newTrip = new Trip();
                var locationFromDb = db.Locations.Where(c => c.StreetName == model.StartLocation.StreetName && c.City == model.StartLocation.City && c.ZipCode == model.StartLocation.ZipCode).SingleOrDefault();
                if(locationFromDb != null)
                {
                    newTrip.StartLocation = locationFromDb.AddressString;
                }
                else
                {
                    db.Locations.Add(model.StartLocation);
                    newTrip.StartLocation = model.StartLocation.AddressString;
                }
                var endLocationFromDb = db.Locations.Where(c => c.StreetName == model.EndLocation.StreetName && c.City == model.EndLocation.City && c.ZipCode == model.EndLocation.ZipCode).SingleOrDefault();
                if (endLocationFromDb != null)
                {
                    newTrip.EndLocation = endLocationFromDb.AddressString;
                }
                else
                {
                    db.Locations.Add(model.EndLocation);
                    newTrip.EndLocation = model.EndLocation.AddressString;
                }
                newTrip.TripTime = .15f;
                newTrip.Name = model.Trip.Name;
                newTrip.WeatherID = 1;

                db.Trips.Add(newTrip);
                db.SaveChanges();
                return View("ViewTrip", newTrip);
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
            var DeleteThisTrip = db.Trips.Where(t => t.TripID == id).Single();
            
            return View(DeleteThisTrip);
        }

        // POST: Trip/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Trip thisItem)
        {
            var DeleteThisTrip = db.Trips.Where(t => t.TripID == id).Single();

            try
            {
                if (DeleteThisTrip != null)
                {
                    db.Trips.Remove(DeleteThisTrip);
                    db.SaveChanges();
                }
                
                
                

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
