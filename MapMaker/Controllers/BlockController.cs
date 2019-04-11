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
        private readonly Lazy<BlockService> _bSvc;
        private readonly Lazy<MapService> _mSvc;
        private readonly Lazy<BlockValidationService> _bvSvc;

        public BlockController()
        {
            _bSvc = new Lazy<BlockService>(CreateBlockService);
            _mSvc = new Lazy<MapService>(CreateMapService);
            _bvSvc = new Lazy<BlockValidationService>();
        }

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
            if (!_bvSvc.Value.CheckIfBlockPlacementIsValidCreate(model.CreateBlockModel))
            {
                ModelState.AddModelError("", "There is already a block in that position.");
                return View(model);
            }
            if (_bSvc.Value.CreateBlock(model))
            {
                TempData["SaveResult"] = "Block succesfully added!";
                return RedirectToAction("Details", "Map", new { id = model.MapModel.MapID});
            }

            return View(model);
        }

        public ActionResult CreateExit(int id)
        {
            var model = _mSvc.Value.GetMapByID(id);
            model.CreateBlockModel.MapID = id;
            model.CreateBlockModel.Type = "Exit";
            return View(model);
        }

        // POST: Block/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateExit(CreateBlockViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Something went wrong | Error: ModelState is not valid.");
                return View(model);
            }
            if (!ExitValidation(model.CreateBlockModel)) return View(model);

            if (_bSvc.Value.CreateExitBlock(model))
            {
                TempData["SaveResult"] = "Block succesfully added!";
                return RedirectToAction("Details", "Map", new { id = model.MapModel.MapID });
            }

            return View(model);
        }

        private bool ExitValidation(BlockCreate model)
        {
            bool idValid = true;
            bool locationValid = true;
            if (!_bvSvc.Value.CheckIfExitLocationIsValid(model))
            {
                ModelState.AddModelError("", "Exit blocks must be positioned at the edge of the map and face a wall.");
                idValid = false;
            }
            if (!_bvSvc.Value.CheckIfExitIdIsValid(model.ExitToID, model.MapID))
            {
                var mapSvc = CreateMapService();
                ModelState.AddModelError("", "Please enter an ExitToID that matches an existing map other than the current one.");
                ModelState.AddModelError("", $"Available Map IDs to exit to: | {mapSvc.GetExitMapIDAvailabilityExcludingCurrentID(model.MapID)}");
                locationValid = false;
            }
            return (idValid && locationValid);
        }

        // GET: Block/Edit/5
        public ActionResult Edit(int id)
        {
            var detail = _bSvc.Value.GetBlockByID(id).BlockDetail;
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
            if (!EditValidation(id, model)) return View(model);

            if (model.TypeOfBlock == "Exit")
            {
                if (!EditIntoExitValidation(model)) return View(model);
                TempData["model"] = model;
                return RedirectToAction("ExitEdit", "Block", new { id = model.ID });
            }

            if (!_bvSvc.Value.CheckIfBlockPlacementIsValidEdit(model))
            {
                ModelState.AddModelError("", "There is already a block in that position.");
                return View(model);
            }

            if (_bSvc.Value.UpdateBlock(model))
            {
                TempData["SaveResult"] = "The block was updated succesfully.";
                return RedirectToAction("Details", "Map", new { id = model.MapID });
            }

            ModelState.AddModelError("", "Your block could not be updated.");
            return View(model);
        }

        private bool EditValidation(int id, BlockEdit model)
        {
            if (!ModelState.IsValid) return false;

            if (model.ID != id)
            {
                ModelState.AddModelError("", "ID Mismatch");
                return false;
            }

            if (model.TypeOfBlock == "Wall")
                if (_bvSvc.Value.CheckIfWallHasEvent(model))
                {
                    ModelState.AddModelError("", "Game event attached to this block must be removed before turning it into a wall block type.");
                    return false;
                }

            return true;
        }

        private bool EditIntoExitValidation(BlockEdit model)
        {
            if (!_bvSvc.Value.CheckIfExitLocationIsValid(model))
            {
                ModelState.AddModelError("", "Exit blocks must be positioned at the edge of the map.");
                return false;
            }
            return true;
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

            if (!ExitEditValidation(model)) return View(model);

            if (_bSvc.Value.UpdateBlock(model))
            {
                TempData["SaveResult"] = "The block was updated succesfully.";
                return RedirectToAction("Details", "Map", new { id = model.MapID });
            }

            TempData["SaveResult"] = "Your block could not be updated.";
            return View(model);
        }

        private bool ExitEditValidation(BlockEdit model)
        {
            bool idValid = true;
            bool locationValid = true;
            if (!_bvSvc.Value.CheckIfExitLocationIsValid(model))
            {
                TempData["SaveResult"] = "Exit blocks must face a wall.";
                idValid = false;
            }
            if (!_bvSvc.Value.CheckIfExitIdIsValid(model.ExitToID, model.MapID))
            {
                TempData["SaveResult"] = "Please enter an ExitToID that matches an existing map other than the current one.\n"
                    + $"Available Map IDs to exit to: | {_mSvc.Value.GetExitMapIDAvailabilityExcludingCurrentID(model.MapID)}";
                locationValid = false;
            }
            return (idValid && locationValid);
        }

        // GET: Block/Delete/5
        public ActionResult Delete(int id)
        {
            var model = _bSvc.Value.GetBlockByID(id).BlockDetail;
            return View(model);
        }

        // POST: Block/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var model = _bSvc.Value.GetBlockByID(id);
            _bSvc.Value.DeleteBlock(id);
            TempData["SaveResult"] = "Block was succesfully deleted.";
            return RedirectToAction("Details", "Map", new { id = model.BlockDetail.MapID } );
        }
    }
}
