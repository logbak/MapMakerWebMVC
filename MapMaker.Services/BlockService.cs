using MapMaker.Data;
using MapMaker.Models;
using MapMaker.Services;
using MapMaker.Models._02_BlockModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapMaker.Models._03_GameEventModels;

namespace MapMaker.Services
{
    public class BlockService
    {
        private readonly Guid _userID;
        public BlockService(Guid userID)
        {
            _userID = userID;
        }

        private BlockType GetBlockTypeFromString(string type)
        {
            Enum.TryParse(type, out BlockType blockType);
            return blockType;
        }

        private Direction GetExitDirectionFromString(string type)
        {
            Enum.TryParse(type, out Direction direction);
            return direction;
        }

        public bool CreateBlock(CreateBlockViewModel model)
        {
            var entity = new Block
            {
                MapID = model.MapModel.MapID,
                OwnerID = _userID,
                Name = model.CreateBlockModel.Name,
                Description = model.CreateBlockModel.Description,
                TypeOfBlock = GetBlockTypeFromString(model.CreateBlockModel.Type),
                PosX = model.CreateBlockModel.PosX,
                PosY = model.CreateBlockModel.PosY,
            };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Blocks.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public bool CreateExitBlock(CreateBlockViewModel model)
        {
            var entity = new ExitBlock
            {
                MapID = model.MapModel.MapID,
                OwnerID = _userID,
                Name = model.CreateBlockModel.Name,
                Description = model.CreateBlockModel.Description,
                TypeOfBlock = GetBlockTypeFromString("Exit"),
                PosX = model.CreateBlockModel.PosX,
                PosY = model.CreateBlockModel.PosY,
                ExitDirection = GetExitDirectionFromString(model.CreateBlockModel.ExitDirection),
                ExitToID = model.CreateBlockModel.ExitToID
            };

            using (var ctx = new ApplicationDbContext())
            {
                ctx.Blocks.Add(entity);
                return ctx.SaveChanges() == 1;
            }
        }

        public BlockDetailsWithEvent GetBlockByID(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Blocks.Single(e => e.ID == id);
                var userEntity = ctx.Users.FirstOrDefault(e => e.Id == entity.OwnerID.ToString());
                var blockDetail = new BlockDetail
                {
                    ID = entity.ID,
                    MapID = entity.MapID,
                    Creator = userEntity.Email,
                    TypeOfBlock = entity.TypeOfBlock.ToString(),
                    Name = entity.Name,
                    Description = entity.Description,
                    PosX = entity.PosX,
                    PosY = entity.PosY,
                    HasEvent = ctx.GameEvents.Any(ge => ge.BlockID == entity.ID)
                };

                if (entity.TypeOfBlock.ToString() == "Exit")
                {
                    var exitEntity = ctx.ExitBlocks.Single(e => e.ID == id);
                    blockDetail.ExitDirection = exitEntity.ExitDirection.ToString();
                    blockDetail.ExitToID = exitEntity.ExitToID;
                }

                var eventDetail = new GameEventDetail();
                if (blockDetail.HasEvent)
                {
                    eventDetail = GetGameEvent(entity.ID);
                }

                return new BlockDetailsWithEvent { BlockDetail = blockDetail, GameEventDetail = eventDetail };
            }
        }

        private GameEventDetail GetGameEvent(int id)
        {
            var svc = new GameEventService(_userID);
            var detail = svc.GetGameEventByID(id);
            return detail;
        }

        public IEnumerable<BlockListItem> GetBlocksByMapID(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var blockQuery = ctx.Blocks.Where(e => e.MapID == id && e.TypeOfBlock != BlockType.Exit).Select(e => new BlockListItem
                {
                    ID = e.ID,
                    MapID = e.MapID,
                    Type = e.TypeOfBlock.ToString(),
                    Name = e.Name,
                    Description = e.Description,
                    PosX = e.PosX,
                    PosY = e.PosY,
                    HasEvent = ctx.GameEvents.Where(g => g.BlockID == e.ID).Any()
                }
                );
                var exitQuery = ctx.ExitBlocks.Where(e => e.MapID == id && e.TypeOfBlock == BlockType.Exit).Select(e => new BlockListItem
                {
                    ID = e.ID,
                    MapID = e.MapID,
                    Type = e.TypeOfBlock.ToString(),
                    Name = e.Name,
                    Description = e.Description,
                    PosX = e.PosX,
                    PosY = e.PosY,
                    HasEvent = ctx.GameEvents.Where(g => g.BlockID == e.ID).Any(),
                    ExitDirection = e.ExitDirection.ToString(),
                    ExitToID = e.ExitToID,
                }
                );
                var queriesArray = blockQuery.ToArray().Concat(exitQuery.ToArray());
                return queriesArray;
            }
        }

        public bool UpdateBlock(BlockEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Blocks.Single(e => e.MapID == model.MapID && e.OwnerID == _userID && e.ID == model.ID);

                entity.TypeOfBlock = GetBlockTypeFromString(model.TypeOfBlock);
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.PosX = model.PosX;
                entity.PosY = model.PosY;

                return ctx.SaveChanges() == 1;
            }
        }

        public bool DeleteBlock(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Blocks.Single(e => e.ID == id && e.OwnerID == _userID);
                ctx.Blocks.Remove(entity);

                return ctx.SaveChanges() == 1;
            }
        }

        public bool DetachOrDeleteBlocksByMap(int mapID, bool delete)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Blocks.Where(e => e.MapID == mapID && e.OwnerID == _userID).Select(e => new BlockListItem { ID = e.ID });
                var exitQuery = ctx.ExitBlocks.Where(e => e.MapID == mapID && e.OwnerID == _userID).Select(e => new BlockListItem { ID = e.ID });

                if (!delete)
                {
                    //logic for zeroing out block MapIDs
                    //logic for exits
                }

                else
                {
                    //logic for deleting blocks and exits
                }

                //return may not need to change?
                return ctx.SaveChanges() == 1;
            }
        }
    }
}
