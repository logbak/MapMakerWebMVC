using MapMaker.Data;
using MapMaker.Models;
using MapMaker.Models._02_BlockModels;
using MapMaker.Models._05_ExploreModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Services
{
    public class ExploreService
    {
        public Exploration GetExplorationModel(Exploration inputModel)
        {
            var model = new Exploration();
            var mapSize = GetMapSize(inputModel.MapID);
            model.AvailableMaps = GetMapIdList();
            model.MapPreview = GetMapPreview(inputModel.MapID);
            var position = GetPlayerInitialPosition(inputModel.MapID, inputModel.HasExitPos, inputModel.ExitPos);
            model.PosX = position[0];
            model.PosY = position[1];
            model.SizeX = mapSize[0];
            model.SizeY = mapSize[1];
            model.OccupiedAreas = GetOccupiedBlocks(inputModel.MapID);
            model.Descriptions = GetDescriptions(inputModel.MapID);
            model.Events = GetEvents(inputModel.MapID);
            model.ExitsInfo = GetExitsInfo(inputModel.MapID);
            return model;
        }

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

        private string GetMapPreview(int id)
        {
            var svc = new MapPreviewService();
            using (var ctx = new ApplicationDbContext())
            {
                var map = ctx.Maps.Single(e => e.ID == id);
                return svc.PrintCurrentRoomForCanvas(svc.GenerateInputRoomFromMap(map));
            }
                
        }

        private int[] GetMapSize(int id)
        {
            var svc = new MapPreviewService();
            using (var ctx = new ApplicationDbContext())
            {
                var map = ctx.Maps.Single(e => e.ID == id);
                int[] output = { map.SizeX, map.SizeY };
                return output;
            }

        }

        private List<int> GetPlayerInitialPosition(int mapID, bool hasExitPos, string exitPos)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var x = 1;
                var y = 1;
                if (hasExitPos && exitPos != "0/0")
                {
                    string[] output = exitPos.Split('/');
                    x = int.Parse(output[0]);
                    y = int.Parse(output[1]);
                }
                else { 
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
                }
                List<int> position = new List<int>();
                position.Add(x);
                position.Add(y);
                return position;
            }
        }

        private List<string> GetOccupiedBlocks(int mapID)
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

        private List<string> GetDescriptions(int mapID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var map = ctx.Maps.Single(m => m.ID == mapID);

                var query = ctx.Blocks.Where(e => e.MapID == mapID && e.TypeOfBlock != BlockType.Exit && e.TypeOfBlock != BlockType.Wall).Select(e => new BlockListItem
                {
                    PosX = e.PosX,
                    PosY = e.PosY,
                    Name = e.Name,
                    Description = e.Description
                });

                var output = new List<string>();

                output.Add(map.Name + "," + map.Description);

                foreach (BlockListItem b in query)
                {
                    output.Add(b.PosX + "," + b.PosY + "," + b.Name + "," + b.Description);
                }
                return output;
            }
        }

        private List<string> GetEvents(int mapID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var bQuery = ctx.Blocks.Where(e => e.MapID == mapID && e.TypeOfBlock != BlockType.Exit).Select(e => new BlockListItem
                {
                    ID = e.ID,
                    Name = e.Name,
                    PosX = e.PosX,
                    PosY = e.PosY
                });
                //turning query into a list so it can be closed (and allow the game events query to be done further down)
                var query = bQuery.ToList();

                var output = new List<string>();

                foreach (BlockListItem b in query)
                {
                    if (ctx.GameEvents.Any(e => e.BlockID == b.ID))
                    {
                        //gets game event attached to the block if one is found in the above conditional
                        var ge = ctx.GameEvents.Single(e => e.BlockID == b.ID);
                        //sets a string in the list of "X,Y,BlockName,EventName,EventDescription"
                        output.Add(b.PosX + "," + b.PosY + "," + b.Name + "," + ge.Name + "," + ge.Description);
                        continue;
                    }
                    //if no events exist for the block loop with continue without adding to the list
                    continue;
                }
                
                return output;
            }
        }

        private List<string> GetExitsInfo(int mapID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var query = ctx.ExitBlocks.Where(e => e.MapID == mapID && e.TypeOfBlock == BlockType.Exit).Select(e => new BlockListItem
                {
                    PosX = e.PosX,
                    PosY = e.PosY,
                    ExitDirection = e.ExitDirection.ToString(),
                    ExitToID = e.ExitToID,
                    ID = e.ID
                });

                var output = new List<string>();
                char direction = ' ';
                foreach (BlockListItem b in query)
                {
                    direction = b.ExitDirection.First();
                    output.Add(b.PosX + "," + b.PosY + "," + direction + "," + b.ExitToID + "," + GetExitPosition(b.ID));
                }
                return output;
            }
        }

        private string GetExitPosition(int exitBlockID)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var exit = ctx.ExitBlocks.Single(e => e.ID == exitBlockID);
                string output = "0/0";

                if (ctx.ExitBlocks.Any(e => e.MapID == exit.ExitToID && e.ExitToID == exit.MapID)){

                    var entrance = ctx.ExitBlocks.Single(e => e.MapID == exit.ExitToID && e.ExitToID == exit.MapID);
                    output = (entrance.PosX + "/" + entrance.PosY);
                    return output;
                }
                return output;
            }
        }
    }
}
