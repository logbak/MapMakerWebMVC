using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models._04_MapPreviewModels
{
    public class PreviewBlock
    {
        public int BlockID { get; set; }
        public char Icon { get; set; }
        public string Description { get; set; }
        public bool IsTrigger { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }

        public PreviewBlock() { }

        public PreviewBlock(int blockID, char icon, string description, bool isTrigger, int posX, int posY)
        {
            BlockID = blockID;
            Icon = icon;
            Description = description;
            IsTrigger = isTrigger;
            PosX = posX;
            PosY = posY;
        }
    }
}
