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
            var model = svc.GetBlockByID(id);
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
                return RedirectToAction("Details", "Map", new { id = model.MapModel.MapID});
            }

            return View(model);
        }

        //public bool AddBlockToMap(CreateBlockViewModel model)
        //{
        //    var service = CreateMapService();
        //    var detail = service.GetMapByID(model.MapModel.MapID);
        //    var mapEdit = new MapEdit
        //    {
        //        BlockIDs = detail.MapModel.BlockIDs
        //    };
        //}

        // GET: Block/Edit/5
        public ActionResult Edit(int id)
        {
            var service = CreateBlockService();
            var detail = service.GetBlockByID(id);
            var model = new BlockEdit
            {
                ID = detail.ID,
                MapID = detail.MapID,
                Creator = detail.Creator,
                TypeOfBlock = detail.TypeOfBlock,
                Name = detail.Name,
                Description = detail.Description,
                PosX = detail.PosX,
                PosY = detail.PosY
            };
            return View(model);
        }

        // POST: Block/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, BlockEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.ID != id)
            {
                ModelState.AddModelError("", "ID Mismatch");
                return View(model);
            }

            var service = CreateBlockService();

            if (service.UpdateBlock(model))
            {
                TempData["SaveResult"] = "The block was updated succesfully.";
                return RedirectToAction("Details", "Map", new { id = model.MapID });
            }

            ModelState.AddModelError("", "Your block could not be updated.");
            return View(model);
        }

        // GET: Block/Delete/5
        public ActionResult Delete(int id)
        {
            var service = CreateBlockService();
            var model = service.GetBlockByID(id);
            return View(model);
        }

        // POST: Block/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var service = CreateBlockService();
            var model = service.GetBlockByID(id);
            service.DeleteBlock(id);
            TempData["SaveResult"] = "Block was succesfully deleted.";
            return RedirectToAction("Details", "Map", new { id = model.MapID } );
        }
    }
}
