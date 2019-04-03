using MapMaker.Models._02_BlockModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models
{
    public class MapBlockViewModel
    {
        public MapDetail MapDetail { get; set; }
        public IEnumerable<BlockListItem> BlockLists { get; set; }
    }
}
    