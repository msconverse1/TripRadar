
﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Net.Http;
using System.Threading.Tasks;

using System.Web;
using System.Web.Mvc;

using TripRadar.Models;
using System.IO;
using System.Xml;

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
        public async Task<ActionResult> ViewTrip(int id)
        {
            var SeeMyTrip = db.Trips.Where(t => t.TripID == id).SingleOrDefault();
            
            var toUpdate = db.Weathers.Where(w => w.WeatherId == SeeMyTrip.WeatherID).FirstOrDefault();
            var location = db.Locations.Where(l => l.StreetName + " " + l.City + " " + l.State + " " + l.ZipCode == SeeMyTrip.StartLocation).FirstOrDefault();

           
            SeeMyTrip.WeatherID = await WeatherInfo(location);
            // db.SaveChanges();
            SeeMyTrip.Weather = db.Weathers.Where(w => w.WeatherId == SeeMyTrip.WeatherID).FirstOrDefault();
            TripWeatherView tripWeatherView = new TripWeatherView()
            {
                Trip = SeeMyTrip,
                Weather = SeeMyTrip.Weather
            };
            return View(tripWeatherView);
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
        public async Task<ActionResult> Create(TripViewModel model)
        {
            try
            {
                //Location location = new Location();
                //location = model.StartLocation;
                //db.Locations.Add(location);
                Trip newTrip = new Trip();

                Vehicle newVehicle = AddVehicle(model.User.Vehicle);
                var user = GetUser();
                newVehicle.VehicleKey = await GetVehicleKey(newVehicle);
                newVehicle.VehicleAvgMpg = await GetVehicleMpg(newVehicle);
                newTrip.WeatherID = await WeatherInfo(model.StartLocation);
                var time = await GetDrivingDistance(model.StartLocation, model.EndLocation);
                var locationFromDb = db.Locations.Where(c => c.StreetName == model.StartLocation.StreetName && c.City == model.StartLocation.City && c.ZipCode == model.StartLocation.ZipCode).SingleOrDefault();
                if (locationFromDb != null)
                {
                    newTrip.StartLocation = locationFromDb.AddressString;
                }
                else
                {
                    db.Locations.Add(model.StartLocation);
                    db.SaveChanges();
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
                    db.SaveChanges();
                    newTrip.EndLocation = model.EndLocation.AddressString;
                }
              
              
                newTrip.TripTime = time[1];
                newTrip.TripDistance = time[0];
                newTrip.Name = model.Trip.Name;
                newTrip.Weather = db.Weathers.Where(w => w.WeatherId == newTrip.WeatherID).FirstOrDefault();
                CalMPG(newTrip,newVehicle);

                db.Trips.Add(newTrip);
                db.SaveChanges();
                user.TripID = newTrip.TripID;
                user.Vehicle = newVehicle;
                TripWeatherView tripWeatherView = new TripWeatherView()
                {
                    Trip = newTrip,
                  Weather = newTrip.Weather
                };
                return View("ViewTrip", tripWeatherView);
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


        public ActionResult SendEmail(int id)
        {
            var ShareThisTrip = db.Trips.Find(id);
            //string url = Url.Action("ShareThisTrip", "Trip", new System.Web.Routing.RouteValueDictionary(new { id = id }), "https", Request.Url.Host);

            return View(ShareThisTrip);
        }

        [HttpPost]
        public ActionResult SendEmail(string receiver, string subject, string message, string URL, int id)
        {
            string url = Url.Action("ShareThisTrip", "Trip", new System.Web.Routing.RouteValueDictionary(new { id = id }), "https", Request.Url.Host);

            try
            { 
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("Nevin.Seibel.Test@gmail.com", "Trip Radar");
                    var receiverEmail = new MailAddress(receiver, "Receiver");
                    var password = "donthackme1";
                    ////var sub = subject;
                    ////var body = message;
                    //var URL = db.Trips
                    var smtp = new SmtpClient()
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        Credentials = new NetworkCredential(senderEmail.Address, password),
                        Timeout = 20000
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail))
                    {
                        mess.Subject = subject;
                        mess.Body = "Check out my trip " + url + "";
                        mess.IsBodyHtml = true;
                        smtp.Send(mess);
                    }
                }
                
            }

            catch (Exception)
            {
                ViewBag.Error = "Some Error";
            }

            return RedirectToAction("Index");
            
        }
        //Get Weather based on Location call
        public async Task<int> WeatherInfo(Location location)
        {
            string weatherAPI = "4a219d24ec4bd8504123161859504e32";
            // User driver = db.User.Where(u => u.UserId == id).FirstOrDefault();
            //  var zipcode= driver.Trip.Location.ZipCode;
            ////   string urlforzipcode = $"http://api.openweathermap.org/data/2.5/forecast?zip={53202}" +
            // $"&mode=json&units=metric&APPID={weatherAPI}";
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("http://api.openweathermap.org");
                var response = await client.GetAsync($"/data/2.5/weather?zip={location.ZipCode}&appid={weatherAPI}&units=metric");
                response.EnsureSuccessStatusCode();


                var stringResult = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(stringResult);
                var j_weatherDesc = json["weather"][0]["description"];
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
                TimeZoneInfo timeZone = TimeZoneInfo.Local;
                dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
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
                db.Weathers.Add(weather);
                db.SaveChanges();
                return weather.WeatherId;


            }

        }
        public async Task<string[]> GetDrivingDistance(Location origin, Location destination)
        {
            var Origin = origin.StreetName+" " + origin.City+" " + origin.State+" " + origin.ZipCode;
            var Destination = destination.StreetName + " " + destination.City + " " + destination.State + " " + destination.ZipCode;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://maps.googleapis.com");
                var response = await client.GetAsync($" /maps/api/distancematrix/json?units=imperial&origins={Origin}  &destinations={Destination} &key=AIzaSyClqIXEuixfPAVE6ZoxSCO7zOFtX2rCpwA");
                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(stringResult);
                 var j_tripDistance = json["rows"][0]["elements"][0]["distance"]["text"];
                 var j_tripTime = json["rows"][0]["elements"][0]["duration"]["text"];
                var tripDistance = j_tripDistance.ToObject<string>();
                var tripTime = j_tripTime.ToObject<string>();
                string[] concatDistanceTime = new string[2];
                concatDistanceTime[0] = tripDistance;
                concatDistanceTime[1] = tripTime;
                    return concatDistanceTime;
            }
            
        }

        public async void CalMPG(Trip trip,Vehicle vehicle)
        {
           
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://maps.googleapis.com");
                var response = await client.GetAsync($"/maps/api/directions/json?origin={trip.StartLocation}&destination={trip.EndLocation}&key=AIzaSyClqIXEuixfPAVE6ZoxSCO7zOFtX2rCpwA");
                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(stringResult);
                string[] currentMiles;

                
                var stepscount = json["routes"][0]["legs"][0]["steps"].Count();
                for (int i = 0; i < stepscount; i++)
                {
                    var toconvert = json["routes"][0]["legs"][0]["steps"][i]["distance"]["text"].ToObject<string>();
                    string[] stringSeparators = new string[] { " " };
                    var result = toconvert.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    var miles = Convert.ToDouble(result[0]);
                    if (result[1] == "feet")
                    {

                    }

                }



                var TotalMilesICanDrive = vehicle.VehicleAvgMpg * 12;
            var NotifyForGas = .1 * TotalMilesICanDrive;
            }
        }

        public async Task<int> GetVehicleKey(Vehicle vehicle)
        {
            //Seeding for Test purposes	

            WebRequest request = WebRequest.Create($"https://www.fueleconomy.gov/ws/rest/vehicle/menu/options?year={vehicle.VehicleYear}&make={vehicle.VehicleMake}&model={vehicle.VehicleModel}");
            // WebResponse response = await request.GetResponseAsync();	
            WebResponse response = await request.GetResponseAsync();

            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string responseFromServer = reader.ReadToEnd();

            //For JSON object	
            //JObject parsedString = JObject.Parse(responseFromServer);	

            //FOR XML	
            //XElement xElement = XElement.Parse(responseFromServer);	
            //JsonConvert.SerializeXmlNode(xElement);	

            //Converting XML object to JSON	
            XmlDocument document = new XmlDocument();
            document.LoadXml(responseFromServer);
            string jsonString = JsonConvert.SerializeXmlNode(document);

            //Getting JObject as vehicle from string json	
            var parsedObject = JsonConvert.DeserializeObject<JObject>(jsonString);
            var objectKey = parsedObject["menuItems"]["menuItem"][0]["value"];
            vehicle.VehicleKey = Convert.ToInt32(objectKey);

            return vehicle.VehicleKey;
        }

        //helper method to get avgMPG	
        public async Task<float> GetVehicleMpg(Vehicle vehicle)
        {

            WebRequest request = WebRequest.Create($"https://www.fueleconomy.gov/ws/rest/ympg/shared/ympgVehicle/{vehicle.VehicleKey}");
            WebResponse response = await request.GetResponseAsync();

            Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            string responseFromServer = reader.ReadToEnd();

            XmlDocument document = new XmlDocument();
            document.LoadXml(responseFromServer);
            string jsonString = JsonConvert.SerializeXmlNode(document);

            //Getting JObject as vehicle from string json	
            var parsedObject = JsonConvert.DeserializeObject<JObject>(jsonString);
            var objectAvgMpg = parsedObject["yourMpgVehicle"]["avgMpg"].ToString();
            vehicle.VehicleAvgMpg = float.Parse(objectAvgMpg);
            return vehicle.VehicleAvgMpg;
        }
        public Vehicle AddVehicle(Vehicle vehicle)
        {

            if (!DoesVehicleExist(vehicle))
            {
                Vehicle userVehicle = new Vehicle();
                userVehicle.VehicleMake = vehicle.VehicleMake;
                userVehicle.VehicleModel = vehicle.VehicleModel;
                userVehicle.VehicleYear = vehicle.VehicleYear;
                db.Vehicles.Add(userVehicle);
                db.SaveChanges();
                return userVehicle;
            }
            else
            {
                var userVehicle = db.Vehicles.SingleOrDefault(v => v.VehicleKey == vehicle.VehicleKey);
                userVehicle.VehicleId = vehicle.VehicleId;
                db.SaveChanges();
                return userVehicle;
            }
        }

        public bool DoesVehicleExist(Vehicle vehicle)
        {
            var vehicleFromDb = db.Vehicles.Where(d => d.VehicleId == vehicle.VehicleId).SingleOrDefault();
            if (vehicleFromDb == null)
                return false;
            else
                return true;
        }

        public User GetUser()
        {
            var userLoggedIn = User.Identity.GetUserId();
            var user = db.User.SingleOrDefault(u => u.ApplicationUserId == userLoggedIn);
            return user;
        }
    }
}
       