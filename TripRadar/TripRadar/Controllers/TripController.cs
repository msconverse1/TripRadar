using Microsoft.AspNet.Identity;
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
        public ActionResult Index(bool? ViewArchived)
        {

            
            if(ViewArchived == true)
            {
                ViewBag.Archived = true;
                var ViewTheseTrips = db.Trips.Where(t => t.IsArchived == true).ToList();
                return View(ViewTheseTrips);
            }

            var AllTrips = db.Trips.Where(t => t.IsArchived == false).ToList();
            return View(AllTrips);
        }

        // GET: Trip/ViewTrip/5
        public async Task<ActionResult> ViewTrip(int id)
        {
            var SeeMyTrip = db.Trips.Where(t => t.TripID == id).SingleOrDefault();
            var SeeMyTripWeather = db.Weathers.Where(w => w.WeatherId == SeeMyTrip.WeatherID).FirstOrDefault();
            var location = db.Locations.Where(l => l.StreetName + " " + l.City + " " + l.State + " " + l.ZipCode == SeeMyTrip.StartLocation).FirstOrDefault();
           // SeeMyTrip.Weather = SeeMyTripWeather;

           
            //SeeMyTrip.WeatherID = await WeatherInfo(location);
            //// db.SaveChanges();
            //SeeMyTrip.Weather = db.Weathers.Where(w => w.WeatherId == SeeMyTrip.WeatherID).FirstOrDefault();

            TripWeatherView tripWeatherView = new TripWeatherView();


            // Will return a Weather ID based on a api call (newest weather)
            var CurrentWeatherReport = await WeatherInfo(location);

            // if the weather api returns a weather object that already exists then carry on displaying that informaiton through the viewmod
            if (CurrentWeatherReport == SeeMyTrip.WeatherID)
            {
                SeeMyTripWeather.DateTime = DateTime.Now;
                TripWeatherView tripWeatherView1 = new TripWeatherView() {
                    
                    Trip = SeeMyTrip,
                    Weather = SeeMyTripWeather
            };
                return View(tripWeatherView1);
            }


            
            // if the weather api creates a new weather object then we need to set out trip's weather id to that new entrys id
            // and view the trip through the viewMod
            else
            {
                var WeatherBeforeUpdating = db.Weathers.Where(w => w.WeatherId == CurrentWeatherReport).SingleOrDefault();

                // Compare temps to get a difference,
                var PreTempToken = WeatherBeforeUpdating.MainTemp - SeeMyTripWeather.MainTemp;
                var TempToken = Math.Abs(PreTempToken);
                // Compare Wind to get difference
                var PreWindToken = WeatherBeforeUpdating.Speedvalue - SeeMyTripWeather.Speedvalue;
                var WindToken = Math.Abs(PreWindToken);
                
                // Logic of comparisons: Threshold for wind and temp is >10 
                if (TempToken > 10 || WindToken > 10)
                {
                    SeeMyTrip.HasBigChangeWeather = true;
                    // get users email address to send weather update.
                    var _user = User.Identity.GetUserId();
                    var CurrentUser = db.Users.Where(u => u.Id == _user).SingleOrDefault();
                    var CurrentUserEmail = CurrentUser.Email;
                    // Send the email
                    SendEmail(CurrentUserEmail, "Weather Update", SeeMyTrip.TripID);
                    // reset email auto-generate-email trigger
                    SeeMyTrip.HasBigChangeWeather = false;
                }

                SeeMyTrip.WeatherID = CurrentWeatherReport; // this is the update **
                db.SaveChanges();

                TripWeatherView tripWeatherView2 = new TripWeatherView();
                tripWeatherView2.Trip = SeeMyTrip;
                tripWeatherView2.Weather = SeeMyTripWeather;
                return View(tripWeatherView2);
            }


         
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
                List<PinLocations> pinLocations;
                pinLocations = await  CalMPG(newTrip,newVehicle);

                db.Trips.Add(newTrip);
                db.SaveChanges();

                //user.TripID = newTrip.TripID;
                user.Vehicle = newVehicle;
                //user.TripID = newTrip.TripID;

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

            var tripInDb = db.Trips.SingleOrDefault(t => t.TripID == id);
            if (tripInDb == null)
            {
                RedirectToAction("Create");
            }
            return View(tripInDb);

       

        }

        // POST: Trip/Edit/5
        [HttpPost]

        public ActionResult Edit(Trip trip, int id)
        {
            var editThisTrip = db.Trips.Where(t => t.TripID == id).Single();
            try
            {
                if(editThisTrip != null)
                {
                    editThisTrip.StartLocation = trip.StartLocation;
                    editThisTrip.EndLocation = trip.EndLocation;
                    editThisTrip.Name = trip.Name;
                    db.SaveChanges();
                }

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
            //try
            //{
                if (DeleteThisTrip != null)
                {
                    db.Trips.Remove(DeleteThisTrip);
                    //db.Weathers.Remove(DeleteThisTrip.Weather);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            //}
            //catch
            //{
            //    return View();
            //}
        }


        public ActionResult SendEmail(int id)
        {

            return View();
        }


        [HttpPost]
        public ActionResult SendEmail(string receiver, string subject, int id)
        {
            string url = Url.Action("ShareThisTrip", "Trip", new System.Web.Routing.RouteValueDictionary(new { id = id }), "https", Request.Url.Host);
            var TripEmailIsAbout = db.Trips.Find(id);

            try
            { 

                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("Nevin.Seibel.Test@gmail.com", "Trip Radar");
                    var receiverEmail = new MailAddress(receiver, "Receiver");
                    var password = "donthackme1";
                    string body;


                    // logic for auto-generated-email body. Should alert user of changes in weather (current threshhold is now at 10 for wind and temp)
                    if(TripEmailIsAbout.HasBigChangeWeather == true)
                    {
                         body = "Warning! " +
                            " We have tracked some dirastic changes in wind speeds and tempature " +
                            " for certain areas included in your trip. For more information please " +
                            " refer to your trips details at the following link. " +
                            " https://localhost:44386/trip/ViewTrip/" + id + "";

                    }
                    else
                    {
                        body = "Check out my trip at: https://localhost:44386/trip/ViewTrip/" + id + "";
                    }
                    

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
                        mess.Body = body;
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
                var _Humidity = j_humidity.ToObject<float>();
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
                    Humidity = _Humidity,
                    TypeOfSkys = WeatherDesc,
                    DateTime = DateTime.Now
                };
                // Checking to see if weather values exist in table before adding a new entry to weather table.
                // omitted the datetime attribute, under assumption that datetime will be differnt - relative.
                // attempt to narrow that chances of a duplicate entry into the weather table. 
                // effort necessary for the sending of an email based on weather conditions chaning S
                var CheckForDuplicateWeather = db.Weathers.Where(w => Math.Floor(w.MainTemp) == Math.Floor(Tempature)  && Math.Floor(w.Speedvalue) == Math.Floor(WindSpeed) &&
                Math.Floor(w.CloudValue) == Math.Floor(CloudCover) && Math.Floor(w.WindDeg) == Math.Floor(WindDegs) && Math.Floor(w.Humidity) == Math.Floor(_Humidity) && w.TypeOfSkys == WeatherDesc && w.DateTime == dateTime).SingleOrDefault(); 
                // If nothing already exists then add it into the table. 
                if (CheckForDuplicateWeather == null)
                {
                    db.Weathers.Add(weather);
                    db.SaveChanges();
                }
                // Set the Just pulled weather ID to the id of the entry that already exists in the table
                else
                {
                    weather.WeatherId = CheckForDuplicateWeather.WeatherId;
                    db.SaveChanges();
                }
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

        public async Task<List<PinLocations>> CalMPG(Trip trip,Vehicle vehicle)
        {
            List<PinLocations> pinLocations = new List<PinLocations>();
            var TotalMilesICanDrive = vehicle.VehicleAvgMpg * 12;
            var NotifyForGas = (.1 * TotalMilesICanDrive);
            var MileToFillAt = TotalMilesICanDrive - NotifyForGas;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://maps.googleapis.com");
                var response = await client.GetAsync($"/maps/api/directions/json?origin={trip.StartLocation}&destination={trip.EndLocation}&key=AIzaSyClqIXEuixfPAVE6ZoxSCO7zOFtX2rCpwA");
                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(stringResult);
                double totalmiles =0;

                
                var stepscount = json["routes"][0]["legs"][0]["steps"].Count();
                for (int i = 0; i < stepscount; i++)
                {
                    var toconvert = json["routes"][0]["legs"][0]["steps"][i]["distance"]["text"].ToObject<string>();
                    string[] stringSeparators = new string[] { " " };
                    var result = toconvert.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                    var miles = Convert.ToDouble(result[0]);
                    if (result[1] == "ft")
                    {
                        miles = (Convert.ToDouble(miles) / 5280);
                        totalmiles += miles;
                    }
                    else if (result[1] == "mi")
                    {
                        miles = Convert.ToDouble(miles);
                        totalmiles += miles;
                    }
                    if (totalmiles >= MileToFillAt)
                    {

                        //Logic to drop a pin at this  location to display das station near by
                        PinLocations latlong = new PinLocations()
                        {
                            Lat = json["routes"][0]["legs"][0]["steps"][i]["start_location"]["lat"].ToObject<double>(),
                            Lng = json["routes"][0]["legs"][0]["steps"][i]["start_location"]["lng"].ToObject<double>()

                        };
                        pinLocations.Add(latlong);
                        //reset this call and total miles till next stop
                        if (json["routes"][0]["legs"][0]["steps"][i+1]["start_location"] != null)
                        {
                            
                            totalmiles = miles;
                        }
                       
                        //Maybe check weather for this location??

                        //
                        
                    }
                }
                return pinLocations;
            }
        }

        public async Task<int> GetVehicleKey(Vehicle vehicle)
        {
            
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
            try
            {
                var objectKey = parsedObject["menuItems"]["menuItem"][0]["value"];
                vehicle.VehicleKey = Convert.ToInt32(objectKey);
            }
            catch
            {
                //For vehicle not found in the Fuel Economy database/or if user puts in wrong vehicle info
                vehicle.VehicleKey = 0;
            }
            
            return vehicle.VehicleKey;
        }

        //helper method to get avgMPG	
        public async Task<float> GetVehicleMpg(Vehicle vehicle)
        {
            if(vehicle.VehicleKey != 0)
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
            

                //Getting JObject as vehicle from string json
                var parsedObject = JsonConvert.DeserializeObject<JObject>(jsonString);
                try
                {
                    var objectAvgMpg = parsedObject["yourMpgVehicle"]["avgMpg"].ToString();
                    vehicle.VehicleAvgMpg = float.Parse(objectAvgMpg);
                }
                catch
                {
                    //hard coding for now if user vehicle is not found in FE database
                    vehicle.VehicleAvgMpg = 16.55f;
                }
            }
            else
            {
                //hard coding for now if user vehicle is not found in FE database
                vehicle.VehicleAvgMpg = 16.55f;
            };
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
                var userVehicle = db.Vehicles.SingleOrDefault(d => d.VehicleYear == vehicle.VehicleYear && d.VehicleModel == vehicle.VehicleModel && d.VehicleMake == vehicle.VehicleMake);
                //userVehicle.VehicleId = vehicle.VehicleId;
                db.SaveChanges();
                return userVehicle;
            }
        }

        public bool DoesVehicleExist(Vehicle vehicle)
        {
            var vehicleFromDb = db.Vehicles.Where(d => d.VehicleYear == vehicle.VehicleYear && d.VehicleModel == vehicle.VehicleModel && d.VehicleMake == vehicle.VehicleMake).SingleOrDefault();
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

        //Get
        public ActionResult Archive(int id)
        {
            Trip ArchiveThisTrip = db.Trips.Find(id);
            return View(ArchiveThisTrip);
        }

        [HttpPost]
        public ActionResult Archive(int id, Trip trip)
        {
            var ArchiveThisTrip = db.Trips.Find(id);
            if (ArchiveThisTrip != null)
            {
                ArchiveThisTrip.IsArchived = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return RedirectToAction("Index");
        }

        public ActionResult UnArchive(int id)
        {
            Trip UnarchiveThisTrip = db.Trips.Find(id);
            if(UnarchiveThisTrip != null)
            {
                UnarchiveThisTrip.IsArchived = false;
                db.SaveChanges();
                
            }
            return RedirectToAction("Index");
        }

        public ActionResult Places(int id)
        {
            var trip = db.Trips.SingleOrDefault(v => v.TripID == id);
            return View(trip);
        }

        public ActionResult WatchWeather()
        {
            return View();
        }
    }
}
