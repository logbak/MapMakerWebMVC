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
    public class ExploreService
    {
        public List<int> GetMapIdList()
        {
            List<int> mapIdList = new List<int>();
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Maps.ToArray();
                foreach (Map map in query)
                    {
                        mapIdList.Add(map.ID);
                    }
                    return mapIdList;
                }
        }

        public string GetMapPreview(int id)
        {
            var svc = new MapPreviewService();
            using (var ctx = new ApplicationDbContext())
            {
                var map = ctx.Maps.Single(e => e.ID == id);
                return svc.PrintCurrentRoomForCanvas(svc.GenerateInputRoomFromMap(map));
            }
                
        }

        public int[] GetMapSize(int id)
        {
            var svc = new MapPreviewService();
            using (var ctx = new ApplicationDbContext())
            {
                var map = ctx.Maps.Single(e => e.ID == id);
                int[] output = { map.SizeX, map.SizeY };
                return output;
            }

        }

        public List<int> GetPlayerInitialPosition(int mapID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var x = 1;
                var y = 1;
                var mapModel = ctx.Maps.Single(m => m.ID == mapID);
                while (ctx.Blocks.Any(b => b.PosX == x && b.PosY == y && b.MapID == mapID && b.TypeOfBlock != BlockType.Exit))
                {
                    if (x < mapModel.SizeX)
                    {
                        x++;
                        continue;
                    }
                    x = 1;
                    y++;
                }
                List<int> position = new List<int>();
                position.Add(x);
                position.Add(y);
                return position;
            }
        }

        public List<string> GetOccupiedBlocks(int mapID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.Blocks.Where(e => e.MapID == mapID && e.TypeOfBlock != BlockType.Exit).Select(e => new BlockListItem
                {
                    PosX = e.PosX,
                    PosY = e.PosY
                });

                var output = new List<string>();
                foreach (BlockListItem b in query)
                {
                    output.Add(b.PosX + "," + b.PosY);
                }
                return output;
            }
        }

        public List<string> GetExitsInfo(int mapID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.ExitBlocks.Where(e => e.MapID == mapID && e.TypeOfBlock == BlockType.Exit).Select(e => new BlockListItem
                {
                    PosX = e.PosX,
                    PosY = e.PosY,
                    ExitDirection = e.ExitDirection.ToString(),
                    ExitToID = e.ExitToID
                });

                var output = new List<string>();
                char direction = ' ';
                foreach (BlockListItem b in query)
                {
                    direction = b.ExitDirection.First();
                    output.Add(b.PosX + "," + b.PosY + "," + direction + "," + b.ExitToID);
                }
                return output;
            }
        }

    }
}
