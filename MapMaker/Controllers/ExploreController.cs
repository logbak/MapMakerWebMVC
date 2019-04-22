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

            var newModel = _eSvc.Value.GetExplorationModel(model.MapID);
            newModel.PlayerIcon = model.PlayerIcon;
            return View(newModel);
        }
    }
}