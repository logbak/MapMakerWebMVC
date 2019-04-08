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

        public ActionResult CreateExit(int id)
        {
            var svc = CreateMapService();
            var model = svc.GetMapByID(id);
            return View(model);
        }

        // POST: Block/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateExit(CreateBlockViewModel model)
        {
            model.CreateBlockModel.Type = "Exit";
            model.CreateBlockModel.MapID = model.MapModel.MapID;
            if (!ModelState.IsValid)
                return View(model);
            var service = CreateBlockService();
            if (!ExitValidation(model.CreateBlockModel)) return View(model);

            //if (!service.CheckIfExitLocationIsValid(model.CreateBlockModel))
            //{
            //    ModelState.AddModelError("", "Exit blocks must be positioned at the edge of the map.");
            //    return View(model);
            //}

            //if (!service.CheckIfExitIdIsValid(model.CreateBlockModel.ExitToID, model.MapModel.MapID))
            //{
            //    ModelState.AddModelError("", "Please enter an ExitToID that matches an existing map other than the current one.");
            //    return View(model);
            //}

            if (service.CreateExitBlock(model))
            {
                TempData["SaveResult"] = "Block succesfully added!";
                return RedirectToAction("Details", "Map", new { id = model.MapModel.MapID });
            }

            return View(model);
        }

        public bool ExitValidation(BlockCreate model)
        {
            var service = CreateBlockService();
            bool idValid = true;
            bool locationValid = true;
            if (!service.CheckIfExitLocationIsValid(model))
            {
                ModelState.AddModelError("", "Exit blocks must be positioned at the edge of the map.");
                idValid = false;
            }
            if (!service.CheckIfExitIdIsValid(model.ExitToID, model.MapID))
            {
                ModelState.AddModelError("", "Please enter an ExitToID that matches an existing map other than the current one.");
                locationValid = false;
            }
            return (idValid && locationValid);
        }

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

            if (model.TypeOfBlock == "Exit")
            {
                if (!service.CheckIfExitLocationIsValid(model))
                {
                    ModelState.AddModelError("", "Exit blocks must be positioned at the edge of the map.");
                    return View(model);
                }
                TempData["model"] = model;
                return RedirectToAction("ExitEdit", "Block", new { id = model.ID });
            }

            if (service.UpdateBlock(model))
            {
                TempData["SaveResult"] = "The block was updated succesfully.";
                return RedirectToAction("Details", "Map", new { id = model.MapID });
            }

            ModelState.AddModelError("", "Your block could not be updated.");
            return View(model);
        }

        [HttpGet]
        public ActionResult ExitEdit (int id, BlockEdit currentModel)
        {
            currentModel = (BlockEdit)TempData["model"];
            var model = new BlockEdit
            {
                ID = currentModel.ID,
                MapID = currentModel.MapID,
                Creator = currentModel.Creator,
                TypeOfBlock = currentModel.TypeOfBlock,
                Name = currentModel.Name,
                Description = currentModel.Description,
                PosX = currentModel.PosX,
                PosY = currentModel.PosY
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExitEdit(BlockEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            var service = CreateBlockService();

            if (!service.CheckIfExitIdIsValid(model.ExitToID, model.MapID))
            {
                TempData["SaveResult"] = "Please enter an ExitToID that matches an existing map other than the current one.";
                return View(model);
            }

            if (service.UpdateBlock(model))
            {
                TempData["SaveResult"] = "The block was updated succesfully.";
                return RedirectToAction("Details", "Map", new { id = model.MapID });
            }

            TempData["SaveResult"] = "Your block could not be updated.";
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
