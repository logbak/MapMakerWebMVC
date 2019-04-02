using MapMaker.Data;
using MapMaker.Models;
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
                    Creator = ctx.Users.FirstOrDefault(u => u.Id == e.OwnerID.ToString()).Email,
                    Name = e.Name,
                    Description = e.Description,
                    SizeX = e.SizeX,
                    SizeY = e.SizeY
                }
                        );
                return query.ToArray();
            }
        }

        public IEnumerable<MapListItem> GetMapsByCurrentUser()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Maps.Where(e => e.OwnerID == _userID).Select( e => new MapListItem
                                {
                                    Creator = ctx.Users.FirstOrDefault(u => u.Id == e.OwnerID.ToString()).Email + "(You)",
                                    Name = e.Name,
                                    Description = e.Description,
                                    SizeX = e.SizeX,
                                    SizeY = e.SizeY
                                }
                        );
                return query.ToArray();
            }
        }

        public MapDetail GetMapByID(int mapID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Maps.Single(e => e.ID == mapID);
                var userEntity = ctx.Users.FirstOrDefault(e => e.Id == entity.OwnerID.ToString());

                return new MapDetail
                    {
                        MapID = entity.ID,
                        OwnerName = userEntity.Email,
                        Name = entity.Name,
                        Description = entity.Description,
                        SizeX = entity.SizeX,
                        SizeY = entity.SizeY,
                        BlockIDs = entity.BlockIDs
                    };
            }
        }
    }
}
