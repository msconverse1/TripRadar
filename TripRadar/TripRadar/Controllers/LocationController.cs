using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TripRadar.Models;

namespace TripRadar.Controllers
{
    public class LocationController : Controller
    {
        ApplicationDbContext db;

        public LocationController()
        {
            db = new ApplicationDbContext();
        }
        // GET: Location
        public ActionResult Index()
        {
            return View();
        }

        // GET: Location/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Location/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Location/Create
        [HttpPost]
        public ActionResult Create(Location location)
        {
            try
            {
                var locationFromDb = db.Locations.Where(l => l.StreetName == location.StreetName).Where(m => m.City == location.City)
                    .Where(n => n.ZipCode == location.ZipCode).SingleOrDefault();
                if (locationFromDb != null)
                {
                    //if location is already there in the database
                    location.ID = locationFromDb.ID;
                }
                else
                {
                    //create new location
                    db.Locations.Add(location);
                    db.SaveChanges();

                }
                
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Location/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Location/Edit/5
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

        // GET: Location/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Location/Delete/5
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
