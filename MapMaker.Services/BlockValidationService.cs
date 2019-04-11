using MapMaker.Data;
using MapMaker.Models._02_BlockModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Services
{
    public class BlockValidationService
    {
        public bool CheckIfBlockPlacementIsValidCreate(BlockCreate model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if (ctx.Blocks.Any(b => b.PosX == model.PosX && b.PosY == model.PosY))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckIfBlockPlacementIsValidEdit(BlockEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if (ctx.Blocks.Any(b => b.PosX == model.PosX && b.PosY == model.PosY && b.ID != model.ID))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckIfWallHasEvent(BlockEdit model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                if (ctx.GameEvents.Any(e => e.BlockID == model.ID) && model.TypeOfBlock == "Wall")
                {
                    return true;
                }
            }
            return false;
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

                bool locationValid = CheckIfExitIsAtEdgeOfMap(model.PosX, model.PosY, entity.SizeX, entity.SizeY);
                bool spotNotOccupied = CheckForExistingExits(model.PosX, model.PosY, model.ExitDirection, entity);

                return (locationValid && spotNotOccupied);
            }
        }

        public bool CheckIfExitLocationIsValid(BlockCreate model)
        {
            using (var ctx = new ApplicationDbContext())
            {
                var entity = ctx.Maps.Single(m => m.ID == model.MapID);

                bool locationValid = CheckIfExitIsAtEdgeOfMap(model.PosX, model.PosY, entity.SizeX, entity.SizeY);
                bool directionValid = CheckIfDirectionIsValid(model.PosX, model.PosY, model.ExitDirection, entity.SizeX, entity.SizeY);
                bool spotNotOccupied = CheckForExistingExits(model.PosX, model.PosY, model.ExitDirection, entity);

                return (locationValid && directionValid && spotNotOccupied);
            }
        }

        public bool CheckIfExitIsAtEdgeOfMap(int x, int y, int mapX, int mapY)
        {
            return (mapX == x || x == 1 || mapY == y || y == 1);
        }

        public bool CheckIfDirectionIsValid(int x, int y, string direction, int mapX, int mapY)
        {
            bool directionValid = false;

            //checks that the given direction is facing a wall from a given position
            switch (direction)
            {
                case "North":
                    if (y == 1) directionValid = true;
                    break;
                case "South":
                    if (y == mapY) directionValid = true;
                    break;
                case "East":
                    if (x == mapX) directionValid = true;
                    break;
                case "West":
                    if (x == 1) directionValid = true;
                    break;
            };

            return directionValid;
        }

        public bool CheckForExistingExits(int x, int y, string direction, Map map)
        {
            using (var ctx = new ApplicationDbContext())
            {
                //check if any exits already exist in a location on a given map
                if (ctx.ExitBlocks.Any(e => e.PosX == x && e.PosY == y && e.MapID == map.ID))
                {
                    //checks if given position is in a corner
                    if ((x == 1 && y == 1) || (x == map.SizeX && y == 1) || (x == map.SizeX && y == map.SizeY) || (x == 1 && y == map.SizeY))
                    {
                        //checks the shared corner to see if the remaining exit direction is already in use
                        return ctx.ExitBlocks.Any(e => e.PosX == x && e.PosY == y && e.MapID == map.ID && 

                        // statment below returns true no matter what... resolve later
                        e.ExitDirection.ToString() != direction);

                    }
                            
                    //if shared location is not in a corner returns false to prevent two exits 
                    //from being placed in the same spot where only one direction is available
                    return false;
                }
            }
            return true;
        }
    }
}
