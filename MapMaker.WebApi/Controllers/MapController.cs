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

        //[AllowAnonymous]
        //public IHttpActionResult Get()
        //{
        //    var model = _nAMSvc.Value.GetMaps();
        //    return Ok(model);
        //}

        public IHttpActionResult Get()
        {
            var model = _mSvc.Value.GetMapsByCurrentUser();
            return Ok(model);
        }
    }
}
