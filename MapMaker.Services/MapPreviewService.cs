﻿using MapMaker.Data;
using MapMaker.Models._04_MapPreviewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Services
{
    public class MapPreviewService
    {
        public Room GenerateInputRoomFromMap(Map map)
        {
            List<Exit> exits = new List<Exit>();
            List<PreviewBlock> blocks = new List<PreviewBlock>();
            List<WallBlock> wallBlocks = new List<WallBlock>();

            using (var ctx = new ApplicationDbContext())
                {

                    foreach (ExitBlock exit in ctx.ExitBlocks.Where(e => e.MapID == map.ID))
                    {
                        exits.Add(GenerateExitFromBlock(exit));
                    }

                    foreach (Block blck in ctx.Blocks.Where(b => b.MapID == map.ID && b.TypeOfBlock != BlockType.Exit))
                    {
                        blocks.Add(GeneratePreviewBlockFromBlock(blck));
                    }

                }

            Room output = new Room
                (
                    map.ID, 
                    exits, 
                    blocks, 
                    wallBlocks, 
                    map.Name, 
                    map.Description, 
                    map.SizeX, 
                    map.SizeY
                 );
            
            return output;
        }

        private Exit GenerateExitFromBlock(ExitBlock eb)
        {
            Exit output = new Exit
                (
                    eb.PosX, 
                    eb.PosY, 
                    eb.ExitDirection == Direction.North, 
                    eb.ExitDirection == Direction.South, 
                    eb.ExitDirection == Direction.East, 
                    eb.ExitDirection == Direction.West, 
                    eb.ExitToID, 
                    0, 
                    0
                 );
            return output;
        }

        private PreviewBlock GeneratePreviewBlockFromBlock(Block blck)
        {
            bool hasEvent;
            using (var ctx = new ApplicationDbContext())
            {
                hasEvent = ctx.GameEvents.Any(e => e.BlockID == blck.ID);
            }
            PreviewBlock output = new PreviewBlock
                (
                    blck.ID, 
                    blck.Name[0], 
                    blck.Description, 
                    hasEvent, 
                    blck.PosX, 
                    blck.PosY
                );
            if (blck.TypeOfBlock == BlockType.Wall) output.Icon = '\u2588';
            return output;
        }
        public string PrintCurrentRoomForCanvas(Room room)
        {
            StringBuilder output = new StringBuilder();

            string topWall = "000 " + PrintTopWalls(room);
            string topNumbers = PrintTopNumbersFirstLine(room);
            string topNumbersTwo = PrintTopNumbersSecondLine(room);
            string topNumbersThree = PrintTopNumbersThirdLine(room);
            output.Append(topNumbers + "\n");
            output.Append(topNumbersTwo + "\n");
            output.Append(topNumbersThree + "\n");
            output.Append(topWall + "\n");

            string sideWalls = PrintSideWalls(room);

            Dictionary<int, string> blockLayers = PrintBlockPositions(room);

            for (int i = 1; i <= room.SizeY; i++)
            {
                if (room.ExitList.Exists(b => b.PosY == i) || room.BlockList.Exists(b => b.PosY == i) || room.WallList.Exists(b => b.PosY == i))
                {
                    blockLayers.TryGetValue(i, out string currentLayer);
                    if (i >= 10 && i <= 99) output.AppendLine($"0{i} " + currentLayer);
                    else if (i >= 100 && i <= 999) output.AppendLine($"{i} " + currentLayer);
                    else output.Append($"00{i} " + currentLayer + "\n");
                }
                else
                {
                    if (i >= 10 && i <= 99) output.AppendLine($"0{i} " + sideWalls);
                    else if (i >= 100 && i <= 999) output.AppendLine($"{i} " + sideWalls);
                    else output.Append($"00{i} " + sideWalls + "\n");
                }
            }

            string bottomWall = PrintBottomWalls(room);
            output.Append("    " + bottomWall + "\n");

            return output.ToString();
        }

        private string PrintTopNumbersFirstLine(Room room)
        {
            string[] numbers = new string[(room.SizeX + 2)];
            for (int i = 0; i <= room.SizeX; i++)
            {
                if (i <= 99)
                {
                    numbers[i + 1] = "0";
                    continue;
                }
                numbers[i + 1] = GetDigits(i).First().ToString();
            }
            numbers[0] = "   X";
            string topWallFull = String.Join("", numbers);
            return topWallFull;
        }

        private string PrintTopNumbersSecondLine(Room room)
        {
            string[] numbers = new string[(room.SizeX + 2)];
            for (int i = 0; i <= room.SizeX; i++)
            {
                if (i <= 9)
                {
                    numbers[i + 1] = "0";
                    continue;
                }
                else if (i >= 10 && i <= 99)
                {
                    numbers[i + 1] = GetDigits(i).First().ToString();
                    continue;
                }
                numbers[i + 1] = GetDigits(i)[1].ToString();
            }
            numbers[0] = "    ";
            string topWallFull = String.Join("", numbers);
            return topWallFull;
        }
        private string PrintTopNumbersThirdLine(Room room)
        {
            string[] numbers = new string[(room.SizeX + 2)];
            for (int i = 0; i <= room.SizeX; i++)
            {
                if (i >= 10 && i <= 99)
                {
                    numbers[i + 1] = GetDigits(i)[1].ToString();
                    continue;
                }
                else if (i >= 100)
                {
                    numbers[i + 1] = GetDigits(i)[2].ToString();
                    continue;
                }
                numbers[i + 1] = $"{i}";
            }
            numbers[0] = "Y   ";
            string topWallFull = String.Join("", numbers);
            return topWallFull;
        }
        private int[] GetDigits(int number)
        {
            string temp = number.ToString();
            int[] rtn = new int[temp.Length];
            for (int i = 0; i < rtn.Length; i++)
            {
                rtn[i] = int.Parse(temp[i].ToString());
            }
            return rtn;
        }
        private string PrintTopWalls(Room room)
        {
            string[] topWall = new string[(room.SizeX + 2)];
            for (int i = 0; i <= (room.SizeX + 1); i++)
            {
                if (room.ExitList.Exists(e => e.PosX == i && e.PosY == 1 && e.North == true))
                {
                    topWall[i] = "\u2550";
                    continue;
                }
                topWall[i] = "\u2584";
            }
            string topWallFull = String.Join("", topWall);
            return topWallFull;
        }

        private string PrintBottomWalls(Room room)
        {
            string[] bottomWall = new string[(room.SizeX + 2)];
            for (int i = 0; i <= (room.SizeX + 1); i++)
            {
                if (room.ExitList.Exists(e => e.PosX == i && e.PosY == room.SizeY && e.South == true))
                {
                    bottomWall[i] = "\u2550";
                    continue;
                }
                bottomWall[i] = "\u2580";
            }
            string bottomWallFull = String.Join("", bottomWall);
            return bottomWallFull;
        }

        private string PrintSideWalls(Room room)
        {
            string[] sideWalls = new string[(room.SizeX + 2)];
            sideWalls[0] = "\u2588";
            for (int i = 1; i <= (room.SizeX + 1); i++)
            {
                sideWalls[i] = " ";
            }
            sideWalls[(room.SizeX + 1)] = "\u2588";
            string sideWallsFull = String.Join("", sideWalls);
            return sideWallsFull;
        }

        private Dictionary<int, string> PrintBlockPositions(Room room)
        {
            Dictionary<int, string> blockLayer = new Dictionary<int, string>();
            string currentBlockLayer = "";
            foreach (Exit exit in room.ExitList)
            {
                if (blockLayer.Keys.Contains(exit.PosY))
                {
                    continue;
                }
                currentBlockLayer = PrintSingleBlockRow(room, exit.PosY);
                blockLayer.Add(exit.PosY, currentBlockLayer);
            }
            foreach (PreviewBlock block in room.BlockList)
            {
                if (blockLayer.Keys.Contains(block.PosY))
                {
                    continue;
                }
                currentBlockLayer = PrintSingleBlockRow(room, block.PosY);
                blockLayer.Add(block.PosY, currentBlockLayer);
            }
            return blockLayer;
        }

        private string PrintSingleBlockRow(Room room, int currentY)
        {
            string[] block = new string[(room.SizeX + 2)];
            if (room.ExitList.Exists(e => e.PosY == currentY && e.PosX == 1 && e.West == true))
            {
                block[0] = "\u2551";
            }
            else
            {
                block[0] = "\u2588";
            }
            if (room.ExitList.Exists(e => e.PosY == currentY && e.PosX == room.SizeX && e.East == true))
            {
                block[(room.SizeX + 1)] = "\u2551";
            }
            else
            {
                block[(room.SizeX + 1)] = "\u2588";
            }
            for (int i = 1; i <= room.SizeX; i++)
            {
                if (room.BlockList.Exists(b => b.PosX == i && b.PosY == currentY))
                {
                    block[i] = room.BlockList.Find(b => b.PosX == i && b.PosY == currentY).Icon.ToString();
                    continue;
                }
                //// leftover attempt as integrating wallblocks in original game code-- may add to MCV once working in-game
                //else if (room.WallList.Exists(w => w.PosX >= i && w.WidthX <= i && w.PosY == currentY))
                //{
                //    block[i] = room.WallList.Find(w => w.PosX == i && w.PosY == currentY).Icon.ToString();
                //    continue;
                //}
                block[i] = " ";
            }
            string blockfull = String.Join("", block);
            return blockfull;
        }
    }
}
