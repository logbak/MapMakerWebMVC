using MapMaker.Models._05_ExploreModels;
using MapMaker.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MapMaker.Controllers
{
    public class ExploreController : Controller
    {
        private readonly Lazy<ExploreService> _eSvc;

        public ExploreController()
        {
            _eSvc = new Lazy<ExploreService>(CreateExploreService);
        }
        private ExploreService CreateExploreService()
        {
            var service = new ExploreService();
            return service;
        }
        // GET: Explore
        public ActionResult Index()
        {
            var model = new Exploration();
            model.AvailableMaps = _eSvc.Value.GetMapIdList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Exploration model)
        {
            if (model.MapID == 0) return RedirectToAction("Index");

            var newModel = new Exploration();
            var mapSize = _eSvc.Value.GetMapSize(model.MapID);
            newModel.AvailableMaps = _eSvc.Value.GetMapIdList();
            newModel.PlayerIcon = model.PlayerIcon;
            newModel.MapPreview = _eSvc.Value.GetMapPreview(model.MapID);
            var position = _eSvc.Value.GetPlayerInitialPosition(model.MapID);
            newModel.PosX = position[0];
            newModel.PosY = position[1];
            newModel.SizeX = mapSize[0];
            newModel.SizeY = mapSize[1];
            newModel.OccupiedAreas = _eSvc.Value.GetOccupiedBlocks(model.MapID);
            newModel.ExitsInfo = _eSvc.Value.GetExitsInfo(model.MapID);
            return View(newModel);
        }
    }
}