using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models
{
    public class MapDetail
    {
        public int MapID { get; set; }

        public string OwnerName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int SizeX { get; set; }

        public int SizeY { get; set; }

        public string BlockIDs { get; set; }

        public string MapPreview { get; set; }
    }
}
