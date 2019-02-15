using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
    }
}
