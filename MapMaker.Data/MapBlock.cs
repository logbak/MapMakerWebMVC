using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Data
{
    public class MapBlock
    {
        public int ID { get; set; }
        public int MapID { get; set; }
        public int BlockID { get; set; }
        public int ExitBlockID { get; set; }
    }
}
