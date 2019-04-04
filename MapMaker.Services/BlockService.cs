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

        private BlockType GetBlockTypeFromString(string type)
        {
            Enum.TryParse(type, out BlockType blockType);
            return blockType;
        }

        public IEnumerable<BlockListItem> GetBlocksByMapID(int id)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Blocks.Where(e => e.MapID == id).Select(e => new BlockListItem
                {
                    ID = e.ID,
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
    }
}
