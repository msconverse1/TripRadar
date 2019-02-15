using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using TripRadar.Models;

namespace TripRadar.Controllers
{
    public class VehicleController : Controller
    {
        ApplicationDbContext db;


        public VehicleController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Vehicle
        //Make this List of Vehicles user have
        public ActionResult Index()
        {

            return View();
        }

        // GET: Vehicle/Details/5
        //Details of Each vehicle
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Vehicle/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Vehicle/Create
        [HttpPost]
        public ActionResult Create(Vehicle vehicle)
        {
            try
            {
                var vehicleFromDb = db.Vehicles.SingleOrDefault(v => v.VehicleMake == vehicle.VehicleMake && v.VehicleModel == vehicle.VehicleModel
                                    && v.VehicleYear == vehicle.VehicleYear);
                if(vehicleFromDb != null)
                {
                    vehicle.VehicleId = vehicleFromDb.VehicleId;
                }
                else
                {
                    db.Vehicles.Add(vehicle);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Vehicle/Edit/5.
        //Can Edit each vehicle/Update each vehicle
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Vehicle/Edit/5
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

        // GET: Vehicle/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Vehicle/Delete/5
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
        
        //public async Task<int> GetVehicleKey(Vehicle vehicle)
        public async Task<Vehicle> GetVehicleKey(Vehicle vehicle)
        {
            //Seeding for Test purposes
            vehicle.VehicleYear = 2012;
            vehicle.VehicleMake = "Honda";
            vehicle.VehicleModel = "Accord";
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

            //Got the vehicle key now, need to make another API call to get avgMPG
            vehicle = GetVehicleMpg(vehicle);

            return vehicle;
        }

        //helper method to get avgMPG
        public Vehicle GetVehicleMpg(Vehicle vehicle)
        {
            
            WebRequest request = WebRequest.Create($"https://www.fueleconomy.gov/ws/rest/ympg/shared/ympgVehicle/{vehicle.VehicleKey}");
            WebResponse response = request.GetResponse();

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
            return vehicle;
        }
    }
}
