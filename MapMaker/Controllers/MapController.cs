using MapMaker.Models;
using MapMaker.Models._02_BlockModels;
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
        private readonly Lazy<BlockService> _bSvc;
        private readonly Lazy<BlockService> _nABSvc;
        private readonly Lazy<MapService> _mSvc;
        private readonly Lazy<MapService> _nAMSvc;

        public MapController()
        {
            _bSvc = new Lazy<BlockService>(CreateBlockService);
            _nABSvc = new Lazy<BlockService>(CreateBlockServiceNoAuth);
            _mSvc = new Lazy<MapService>(CreateMapService);
            _nAMSvc = new Lazy<MapService>(CreateMapServiceNoAuth);
        }

        private MapService CreateMapService()
        {
            var userID = Guid.Parse(User.Identity.GetUserId());
            var service = new MapService(userID);
            return service;
        }

        private MapService CreateMapServiceNoAuth()
        {
            var service = new MapService();
            return service;
        }

        private BlockService CreateBlockService()
        {
            var userID = Guid.Parse(User.Identity.GetUserId());
            var service = new BlockService(userID);
            return service;
        }
        private BlockService CreateBlockServiceNoAuth()
        {
            var service = new BlockService();
            return service;
        }

        [AllowAnonymous]
        public ActionResult Maps()
        {
            var model = _nAMSvc.Value.GetMaps();
            return View(model);
        }

        // GET: Map
        public ActionResult Index()
        {
            var model = _mSvc.Value.GetMaps();
            return View(model);
        }

        public ActionResult MyMaps()
        {
            var model = _mSvc.Value.GetMapsByCurrentUser();
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            MapBlockViewModel model = new MapBlockViewModel();
            model.MapDetail = _nAMSvc.Value.GetMapByID(id).MapModel;
            model.BlockLists = _nABSvc.Value.GetBlocksByMapID(id);
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

            if (_mSvc.Value.CreateMap(model))
            {
                TempData["SaveResult"] = "Map succesfully created!";
                return RedirectToAction("MyMaps");
            }

            return View(model);
        }

        // GET: Map/Edit/5
        public ActionResult Edit(int id)
        {
            var detail = _mSvc.Value.GetMapByID(id);
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

            if (_mSvc.Value.UpdateMap(model))
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
            var model = _mSvc.Value.GetMapByID(id);
            return View(model.MapModel);
        }

        // POST: Map/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, MapDetail model)
        {
            _bSvc.Value.DetachOrDeleteBlocksByMap(id, model.DeleteBlocks);
            _mSvc.Value.DeleteMap(id);
            TempData["SaveResult"] = "Map was succesfully deleted.";
            return RedirectToAction("MyMaps");
        }
    }
}
