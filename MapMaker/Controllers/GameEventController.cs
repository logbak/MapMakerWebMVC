using MapMaker.Models;
using MapMaker.Models._03_GameEventModels;
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
    public class GameEventController : Controller
    {
        private GameEventService CreateGameEventService()
        {
            var userID = Guid.Parse(User.Identity.GetUserId());
            var service = new GameEventService(userID);
            return service;
        }

        private BlockService CreateBlockService()
        {
            var userID = Guid.Parse(User.Identity.GetUserId());
            var service = new BlockService(userID);
            return service;
        }

        // GET: GameEvent
        public ActionResult Index()
        {
            var service = CreateGameEventService();
            var model = service.GetGameEvents();
            return View(model);
        }

        // GET: GameEvent/Details/5
        public ActionResult Details(int id)
        {
            var svc = CreateGameEventService();
            var model = svc.GetGameEventByID(id);
            return View(model);
        }

        // GET: GameEvent/Create
        public ActionResult Create(int id)
        {
            var svc = CreateBlockService();
            var blockDetail = svc.GetBlockByID(id).BlockDetail;
            var model = new BlockEventViewModel { DetailOfBlock = blockDetail };
            return View(model);
        }

        // POST: GameEvent/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BlockEventViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var service = CreateGameEventService();
            if (service.CreateGameEvent(model))
            {
                TempData["SaveResult"] = "Event succesfully added!";
                return RedirectToAction("Details", "Block", new { id = model.DetailOfBlock.ID });
            }

            return View(model);
        }

        // GET: GameEvent/Edit/5
        public ActionResult Edit(int id)
        {
            var service = CreateGameEventService();
            var detail = service.GetGameEventByID(id);
            var model = new GameEventEdit
            {
                ID = detail.ID,
                BlockID = detail.BlockID,
                MapID = detail.MapID,
                Creator = detail.Creator,
                TypeOfEvent = detail.TypeOfEvent,
                Name = detail.Name,
                Description = detail.Description
            };
            return View(model);
        }

        // POST: GameEvent/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, GameEventEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.ID != id)
            {
                ModelState.AddModelError("", "ID Mismatch");
                return View(model);
            }

            var service = CreateGameEventService();

            if (service.UpdateGameEvent(model))
            {
                TempData["SaveResult"] = "The event was updated succesfully.";
                return RedirectToAction("Details", "Block", new { id = model.BlockID });
            }

            ModelState.AddModelError("", "Your event could not be updated.");
            return View(model);
        }

        // GET: GameEvent/Delete/5
        public ActionResult Delete(int id)
        {
            var service = CreateGameEventService();
            var model = service.GetGameEventByID(id);
            return View(model);
        }

        // POST: GameEvent/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var service = CreateGameEventService();
            var model = service.GetGameEventByID(id);
            service.DeleteGameEvent(id);
            TempData["SaveResult"] = "Event was succesfully deleted.";
            return RedirectToAction("Details", "Block", new { id = model.BlockID });
        }
    }
}
