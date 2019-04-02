using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Data
{
    public enum Direction { North, South, East, West}

    public class ExitBlock : Block
    {
        public Direction ExitDirection { get; set; }

        public int ExitToID { get; set; }

    }
}
