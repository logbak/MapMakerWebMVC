using MapMaker.Models;
using MapMaker.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MapMaker.Controllers
{
    [Authorize]
    public class MapController : Controller
    {
        private MapService CreateMapService()
        {
            var userID = Guid.Parse(User.Identity.GetUserId());
            var service = new MapService(userID);
            return service;
        }
        // GET: Map
        public ActionResult Index()
        {
            var service = CreateMapService();
            var model = service.GetMaps();
            return View(model);
        }

        public ActionResult MyMaps()
        {
            var service = CreateMapService();
            var model = service.GetMapsByCurrentUser();
            return View(model);
        }

        // GET: Map/Details/5
        public ActionResult Details(int id)
        {
            var svc = CreateMapService();
            var model = svc.GetMapByID(id);
            return View(model);
        }

        // GET: Map/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Map/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MapCreate model)
        {
            if (!ModelState.IsValid) return View(model);
                
            var service = CreateMapService();
            if (service.CreateMap(model))
            {
                TempData["SaveResult"] = "Map succesfully created!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Map/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Map/Edit/5
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

        // GET: Map/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Map/Delete/5
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
