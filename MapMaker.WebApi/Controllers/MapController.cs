using MapMaker.Models;
using MapMaker.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MapMaker.WebApi.Controllers
{
    [Authorize]
    public class MapController : ApiController
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

        public IHttpActionResult Get()
        {
            var model = _mSvc.Value.GetMaps();
            return Ok(model);
        }

        // -- Need to figure out how to utilize multiple no-parameter Get methods
        //[AllowAnonymous]
        //public IHttpActionResult GetNoAuth()
        //{
        //    var model = _nAMSvc.Value.GetMaps();
        //    return Ok(model);
        //}
        //public IHttpActionResult GetByUser()
        //{
        //    var model = _mSvc.Value.GetMapsByCurrentUser();
        //    return Ok(model);
        //}

        [AllowAnonymous]
        public IHttpActionResult GetById(int id)
        {
            MapBlockViewModel model = new MapBlockViewModel();
            model.MapDetail = _nAMSvc.Value.GetMapByID(id).MapModel;
            model.BlockLists = _nABSvc.Value.GetBlocksByMapID(id);
            return Ok(model);
        }

        public IHttpActionResult Post(MapCreate model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!_mSvc.Value.CreateMap(model))
                return InternalServerError();

            return Ok();
        }

        public IHttpActionResult Put(MapEdit model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!_mSvc.Value.UpdateMap(model))
                return InternalServerError();

            return Ok();
        }

        public IHttpActionResult Delete(int id)
        {
            // -- original MVC had option to cascade delete or retain attached blocks (retaining them would set their MapId to 0)
            // -- the line below requires changing method to: Delete(int id, bool deleteBlocks)
            //_bSvc.Value.DetachOrDeleteBlocksByMap(id, deleteBlocks);

            if (!_mSvc.Value.DeleteMap(id))
                return InternalServerError();

            return Ok();
        }

    }
}
