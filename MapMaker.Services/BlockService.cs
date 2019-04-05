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

        public bool UpdateBlockListOnMap(CreateBlockViewModel model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Maps.Single(e => e.ID == model.MapModel.MapID && e.OwnerID == _userID);

                string BlockIdString = ctx.Maps.Single(m => m.ID == model.CreateBlockModel.MapID).BlockIDs;
                //new List<string> = BlockIdString.Split(',');
                //string newBlockID = String.Join(",", ???);

                //entity.BlockIDs = //new block ids string

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
                return new BlockDetail
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
            }
        }

        public IEnumerable<BlockListItem> GetBlocksByMapID(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Blocks.Where(e => e.MapID == id).Select(e => new BlockListItem
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
                return query.ToArray();
            }
        }

        public bool UpdateBlock(BlockEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                // exception here!
                var entity = ctx.Blocks.Single(e => e.MapID == model.MapID && e.OwnerID == _userID);

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
    }
}
