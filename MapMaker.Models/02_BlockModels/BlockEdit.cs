using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models._02_BlockModels
{
    public class BlockEdit
    {
        public int ID { get; set; }

        public int MapID { get; set; }

        public string Creator { get; set; }

        public string TypeOfBlock { get; set; }
        
        public string Name { get; set; }

        public string Description { get; set; }
        
        public int PosX { get; set; }
        
        public int PosY { get; set; }
    }
}
