using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models
{
    public class MapEdit
    {
        public int MapID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string BlockIDs { get; set; }
    }
}
