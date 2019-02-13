using System;
using System.Collections.Generic;
using System.Linq;
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
        public ActionResult WeatherInfo(int? id)
        {
            User driver = db.User.Where(u => u.UserId == id).FirstOrDefault();
           // driver.Trip.

            return RedirectToAction("Index");
        }
    }
}
