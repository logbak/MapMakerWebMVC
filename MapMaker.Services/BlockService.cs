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
                    PosY = e.PosY
                }
                        );
                return query.ToArray();
            }
        }
    }
}
