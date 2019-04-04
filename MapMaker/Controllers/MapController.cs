using MapMaker.Models;
using MapMaker.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Dynamic;
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

        private BlockService CreateBlockService()
        {
            var userID = Guid.Parse(User.Identity.GetUserId());
            var service = new BlockService(userID);
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
            var bsvc = CreateBlockService();
            MapBlockViewModel model = new MapBlockViewModel();
            model.MapDetail = svc.GetMapByID(id).MapModel;
            model.BlockLists = bsvc.GetBlocksByMapID(id);
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
            var service = CreateMapService();
            var detail = service.GetMapByID(id);
            var model = new MapEdit
            {
                MapID = detail.MapModel.MapID,
                Name = detail.MapModel.Name,
                Description = detail.MapModel.Description,
                BlockIDs = detail.MapModel.BlockIDs
            };
            return View(model);
        }

        // POST: Map/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, MapEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.MapID != id)
            {
                ModelState.AddModelError("", "ID Mismatch");
                return View(model);
            }

            var service = CreateMapService();

            if (service.UpdateMap(model))
            {
                TempData["SaveResult"] = "The map was updated succesfully.";
                return RedirectToAction("MyMaps");
            }

            ModelState.AddModelError("", "Your map could not be updated.");
            return View(model);
        }

        // GET: Map/Delete/5
        public ActionResult Delete(int id)
        {
            var service = CreateMapService();
            var model = service.GetMapByID(id);
            return View(model);
        }

        // POST: Map/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var service = CreateMapService();
            service.DeleteMap(id);
            TempData["SaveResult"] = "Map was succesfully deleted.";
            return RedirectToAction("MyMaps");
        }
    }
}
