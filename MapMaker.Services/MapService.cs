using MapMaker.Data;
using MapMaker.Models;
using MapMaker.Models._02_BlockModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Services
{
    public class MapService
    {
        private readonly Guid _userID;

        public MapService(){}

        public MapService(Guid userID)
        {
            _userID = userID;
        }

        public bool CreateMap(MapCreate model)
        {
            var entity = new Map()
                {
                    OwnerID = _userID,
                    Name = model.Name,
                    Description = model.Description,
                    SizeX = model.SizeX,
                    SizeY = model.SizeY
                };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Maps.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public IEnumerable<MapListItem> GetMaps()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Maps.Select(e => new MapListItem
                {
                    ID = e.ID,
                    Creator = ctx.Users.FirstOrDefault(u => u.Id == e.OwnerID.ToString()).UserName,
                    Name = e.Name,
                    Description = e.Description,
                    SizeX = e.SizeX,
                    SizeY = e.SizeY
                }
                        );
                return query.ToArray();
            }
        }

        public string GetExitMapIDAvailabilityExcludingCurrentID(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                StringBuilder sb = new StringBuilder(); 

                var query = ctx.Maps.Where(e => e.ID != id).Select(e => new MapListItem { ID = e.ID });
                
                foreach (MapListItem mapList in query)
                {
                    sb.Append(mapList.ID.ToString() + " | ");
                }

                return sb.ToString();
            }
        }

        public IEnumerable<MapListItem> GetMapsByCurrentUser()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Maps.Where(e => e.OwnerID == _userID).Select( e => new MapListItem
                    {
                        ID = e.ID,
                        Creator = ctx.Users.FirstOrDefault(u => u.Id == e.OwnerID.ToString()).UserName,
                        Name = e.Name,
                        Description = e.Description,
                        SizeX = e.SizeX,
                        SizeY = e.SizeY
                     }
                );
                return query.ToArray();
            }
        }

        public CreateBlockViewModel GetMapByID(int mapID)
        {
            MapPreviewService previewService = new MapPreviewService();

            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Maps.Single(e => e.ID == mapID);
                var userEntity = ctx.Users.FirstOrDefault(e => e.Id == entity.OwnerID.ToString());

                string preview =
                    previewService.PrintCurrentRoomForCanvas
                    (
                        previewService.GenerateInputRoomFromMap(entity)
                    );

                var mapModel = new MapDetail
                {
                    MapID = entity.ID,
                    OwnerName = userEntity.UserName,
                    Name = entity.Name,
                    Description = entity.Description,
                    SizeX = entity.SizeX,
                    SizeY = entity.SizeY,
                    BlockIDs = BlockIDsByMapID(mapID),
                    MapPreview = preview
                };

                return new CreateBlockViewModel
                {
                    MapModel = mapModel
                };
            }
        }

        private string BlockIDsByMapID(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Blocks.Where(e => e.MapID == id && e.TypeOfBlock != BlockType.Exit).Select(e => new BlockListItem
                { ID = e.ID } );

                List<string> output = new List<string>();

                foreach (BlockListItem block in query)
                {
                    output.Add(block.ID.ToString());
                }

                return String.Join(",", output);
            }
        }

        public bool UpdateMap(MapEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Maps.Single(e => e.ID == model.MapID && e.OwnerID == _userID);

                entity.Name = model.Name;
                entity.Description = model.Description;

                return ctx.SaveChanges() == 1;
            }
        }
        public bool DeleteMap(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Maps.Single(e => e.ID == id && e.OwnerID == _userID);
                ctx.Maps.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }
    }
}
