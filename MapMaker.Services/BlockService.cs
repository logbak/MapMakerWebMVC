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
                    Creator = userEntity.UserName,
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
            var detail = svc.GetGameEventByID(id, true);
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

        public List<BlockListItem> GetFreeBlocks()
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Blocks.Where(e => e.MapID == 0 && e.TypeOfBlock != BlockType.Exit).Select(e => new BlockListItem
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
                return query.ToList();
            }
        }
        public List<int> GetMapIdList(int id)
        {
            List<int> mapIdList = new List<int>();
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Maps.Where(m => m.ID != id).ToArray();
                foreach (Map map in query)
                {
                    mapIdList.Add(map.ID);
                }
                return mapIdList;
            }
        }

        public bool AddBlockToMap(CreateBlockViewModel model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Blocks.Single(e => e.ID == model.CreateBlockModel.ID);

                entity.MapID = model.MapModel.MapID;
                entity.OwnerID = _userID;
                entity.TypeOfBlock = GetBlockTypeFromString(model.CreateBlockModel.Type);
                entity.Name = model.CreateBlockModel.Name;
                entity.Description = model.CreateBlockModel.Description;
                entity.PosX = model.CreateBlockModel.PosX;
                entity.PosY = model.CreateBlockModel.PosY;

                return ctx.SaveChanges() == 1;
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
        public bool UpdateExitBlock(BlockEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.ExitBlocks.Single(e => e.MapID == model.MapID && e.OwnerID == _userID && e.ID == model.ID);

                entity.TypeOfBlock = GetBlockTypeFromString(model.TypeOfBlock);
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.PosX = model.PosX;
                entity.PosY = model.PosY;
                entity.ExitDirection = GetExitDirectionFromString(model.ExitDirection);
                entity.ExitToID = model.ExitToID;

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

        public void DetachOrDeleteBlocksByMap(int mapID, bool delete)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Blocks.Where(e => e.MapID == mapID && e.OwnerID == _userID).Select(e => new BlockListItem { ID = e.ID }).ToList();
                var exitQuery = ctx.ExitBlocks.Where(e => e.MapID == mapID && e.OwnerID == _userID).Select(e => new BlockListItem { ID = e.ID }).ToList();

                if (!delete)
                {
                    foreach (BlockListItem block in query)
                    {
                        ctx.Blocks.Single(b => b.ID == block.ID).MapID = 0;
                    }
                    foreach (BlockListItem exit in exitQuery)
                    {
                        ctx.ExitBlocks.Remove(ctx.ExitBlocks.Single(e => e.ID == exit.ID));
                    }
                }

                else
                {
                    foreach (BlockListItem block in query)
                    {
                        ctx.Blocks.Remove(ctx.Blocks.Single(e => e.ID == block.ID));
                    }
                    foreach (BlockListItem exit in exitQuery)
                    {
                        ctx.ExitBlocks.Remove(ctx.ExitBlocks.Single(e => e.ID == exit.ID));
                    }
                }

                ctx.SaveChanges();
            }
        }
    }
}
