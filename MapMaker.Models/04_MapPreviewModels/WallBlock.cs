using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models._04_MapPreviewModels
{
    public enum WallType { Top, Bottom, Side, Door, SideDoor }

    public class WallBlock : PreviewBlock
    {
        public WallType TypeOfWall { get; set; }
        public int WidthX { get; set; }
        public int HeightY { get; set; }

        public WallBlock() { }

        public WallBlock(WallType typeOfWall, bool isTrigger, int posX, int posY, int widthX, int heightY)
        {
            TypeOfWall = typeOfWall;
            IsTrigger = isTrigger;
            PosX = posX;
            PosY = posY;
            WidthX = widthX;
            HeightY = heightY;
            switch (typeOfWall)
            {
                case WallType.Top:
                    Icon = '\u2584';
                    break;
                case WallType.Side:
                    Icon = '\u2588';
                    break;
                case WallType.Bottom:
                    Icon = '\u2580';
                    break;
                case WallType.Door:
                    Icon = '\u2550';
                    break;
                case WallType.SideDoor:
                    Icon = '\u2551';
                    break;
            }
        }
    }
}
