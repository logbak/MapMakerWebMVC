using MapMaker.Data;
using MapMaker.Models;
using MapMaker.Services;
using MapMaker.Models._02_BlockModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Services
{
    public class BlockService
    {
        private readonly Guid _userID;

        public BlockService(Guid userID)
        {
            _userID = userID;
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

        private BlockType GetBlockTypeFromString(string type)
        {
            Enum.TryParse(type, out BlockType blockType);
            return blockType;
        }

        public BlockDetail GetBlockByID(int id)
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
                    PosY = entity.PosY
                };

                if (entity.TypeOfBlock.ToString() == "Exit")
                {
                    var exitEntity = ctx.ExitBlocks.Single(e => e.ID == id);
                    blockDetail.ExitDirection = exitEntity.ExitDirection.ToString();
                    blockDetail.ExitToID = exitEntity.ExitToID;
                }

                return blockDetail;
            }
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
                if (model.TypeOfBlock == "Exit")
                {
                    return UpdateBlockExit(model);
                }

                var entity = ctx.Blocks.Single(e => e.MapID == model.MapID && e.OwnerID == _userID && e.ID == model.ID);

                entity.TypeOfBlock = GetBlockTypeFromString(model.TypeOfBlock);
                entity.Name = model.Name;
                entity.Description = model.Description;
                entity.PosX = model.PosX;
                entity.PosY = model.PosY;

                return ctx.SaveChanges() == 1;
            }
        }

        public bool UpdateBlockExit(BlockEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if (ctx.ExitBlocks.Any(e => e.MapID == model.MapID && e.OwnerID == _userID && e.ID == model.ID))
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

                ctx.Blocks.Remove(ctx.Blocks.Single(b => b.ID == model.ID));
                var newEntity = new ExitBlock
                {
                    ID = model.ID,
                    MapID = model.MapID,
                    OwnerID = _userID,
                    Name = model.Name,
                    Description = model.Description,
                    TypeOfBlock = GetBlockTypeFromString(model.TypeOfBlock),
                    PosX = model.PosX,
                    PosY = model.PosY,
                    ExitDirection = GetExitDirectionFromString(model.ExitDirection),
                    ExitToID = model.ExitToID
                };
                ctx.ExitBlocks.Add(newEntity);

                return ctx.SaveChanges() == 2;
            }
        }

        public bool CheckIfExitIdIsValid(int id, int mapID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if (ctx.Maps.Any(m => m.ID == id) && id != mapID)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckIfExitLocationIsValid(BlockEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Maps.Single(m => m.ID == model.MapID);
                if (entity.SizeX == model.PosX || model.PosX == 1 || entity.SizeY == model.PosY || model.PosY == 1)
                {
                    return true;
                }
            }
            return false;
        }

        //repeats above with different overload, see if there is a way to combine
        public bool CheckIfExitLocationIsValid(BlockCreate model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Maps.Single(m => m.ID == model.MapID);
                if (entity.SizeX == model.PosX || model.PosX == 1 || entity.SizeY == model.PosY || model.PosY == 1)
                {
                    return true;
                }
            }
            return false;
        }

        private Direction GetExitDirectionFromString(string type)
        {
            Enum.TryParse(type, out Direction direction);
            return direction;
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
    }
}
