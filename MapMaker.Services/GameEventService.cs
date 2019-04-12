using MapMaker.Data;
using MapMaker.Models;
using MapMaker.Models._03_GameEventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Services
{
    public class GameEventService
    {
        private readonly Guid _userID;

        public GameEventService(Guid userID)
        {
            _userID = userID;
        }

        private EventType GetEventTypeFromString(string type)
        {
            Enum.TryParse(type, out EventType eventType);
            return eventType;
        }

        public bool CreateGameEvent(BlockEventViewModel model)
        {
            var entity = new GameEvent()
            {
                OwnerID = _userID,
                BlockID = model.DetailOfBlock.ID,
                TypeOfEvent = GetEventTypeFromString(model.CreateEvent.Type),
                Name = model.CreateEvent.Name,
                Description = model.CreateEvent.Description
            };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.GameEvents.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public IEnumerable<GameEventListItem> GetGameEvents()
        {
            using (var ctx = new ApplicationDbContext())
            {

                var query = ctx.GameEvents.Select(e => new GameEventListItem
                {
                    ID = e.ID,
                    BlockID = e.BlockID,
                    Creator = ctx.Users.FirstOrDefault(u => u.Id == e.OwnerID.ToString()).Email,
                    TypeOfEvent = e.TypeOfEvent.ToString(),
                    Name = e.Name,
                    Description = e.Description
            }
                        );
                return query.ToArray();
            }
        }

        public IEnumerable<GameEventListItem> GetGameEventsByCurrentUser()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.GameEvents.Where(e => e.OwnerID == _userID).Select(e => new GameEventListItem
                {
                    ID = e.ID,
                    BlockID = e.BlockID,
                    Creator = ctx.Users.FirstOrDefault(u => u.Id == e.OwnerID.ToString()).Email,
                    TypeOfEvent = e.TypeOfEvent.ToString(),
                    Name = e.Name,
                    Description = e.Description
                }
                );
                return query.ToArray();
            }
        }

        public GameEventDetail GetGameEventByID(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.GameEvents.Single(e => e.ID == id);
                var userEntity = ctx.Users.FirstOrDefault(e => e.Id == entity.OwnerID.ToString());
                int mapID = 0;
                if (ctx.Blocks.Any(b => b.ID == entity.BlockID)) mapID = ctx.Blocks.Single(b => b.ID == entity.BlockID).MapID;
                var gameEventModel = new GameEventDetail
                {
                    ID = entity.ID,
                    BlockID = entity.BlockID,
                    MapID = mapID,
                    Creator = userEntity.Email,
                    TypeOfEvent = entity.TypeOfEvent.ToString(),
                    Name = entity.Name,
                    Description = entity.Description
                };

                return gameEventModel;
            }
        }

        public bool UpdateGameEvent(GameEventEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.GameEvents.Single(e => e.ID == model.ID && e.OwnerID == _userID);

                entity.TypeOfEvent = GetEventTypeFromString(model.TypeOfEvent);
                entity.Name = model.Name;
                entity.Description = model.Description;

                return ctx.SaveChanges() == 1;
            }
        }

        public bool DeleteGameEvent(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.GameEvents.Single(e => e.ID == id && e.OwnerID == _userID);
                ctx.GameEvents.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }

        public bool DetachOrDeleteGameEvent(int blockID, bool delete)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.GameEvents.Single(e => e.BlockID == blockID && e.OwnerID == _userID);

                if (!delete) entity.BlockID = 0;

                else ctx.GameEvents.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }
    }
}

