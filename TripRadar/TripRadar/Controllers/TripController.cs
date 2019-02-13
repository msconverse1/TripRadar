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
        public ActionResult ViewTrip(Trip trip)
        {
            var SeeMyTrip = db.Trips.Where(t => t.TripID == trip.TripID).SingleOrDefault();

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
        public ActionResult Create(Trip trip)
        {
            try
            {
                Trip newTrip = new Trip();
                newTrip.StartLocation = trip.StartLocation;
                newTrip.EndLocation = trip.EndLocation;
                newTrip.Name = trip.Name;
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
        public ActionResult Delete(int tripId)
        {
            var DeleteThisTrip = db.Trips.Where(t => t.TripID == tripId).SingleOrDefault();
            
            return View(DeleteThisTrip);
        }

        // POST: Trip/Delete/5
        [HttpPost]
        public ActionResult Delete(Trip deleteThisTrip)
        {
            try
            {
                db.Trips.Remove(deleteThisTrip);
                db.SaveChanges();
                // Notify the user that the trip was sucessfully or unsucessfully removed. 

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
