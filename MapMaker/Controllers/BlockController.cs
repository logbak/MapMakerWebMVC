using MapMaker.Models;
using MapMaker.Models._02_BlockModels;
using MapMaker.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MapMaker.Controllers
{
    public class BlockController : Controller
    {
        private BlockService CreateBlockService()
        {
            var userID = Guid.Parse(User.Identity.GetUserId());
            var service = new BlockService(userID);
            return service;
        }

        private MapService CreateMapService()
        {
            var userID = Guid.Parse(User.Identity.GetUserId());
            var service = new MapService(userID);
            return service;
        }

        // GET: Block/Details/5
        public ActionResult Details(int id)
        {
            var svc = CreateBlockService();
            var model = svc.GetBlocksByMapID(id);
            //add multi-model model so event can be viewed alonside the attached event
            return View(model);
        }

        // GET: Block/Create
        public ActionResult Create(int id)
        {
            var svc = CreateMapService();
            var model = svc.GetMapByID(id);
            return View(model);
        }

        // POST: Block/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateBlockViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var service = CreateBlockService();
            if (service.CreateBlock(model))
            {
                TempData["SaveResult"] = "Block succesfully added!";
                return RedirectToAction("Details", "Map");
            }

            return View(model);
        }

        // GET: Block/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Block/Edit/5
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

        // GET: Block/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Block/Delete/5
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
