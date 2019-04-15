using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models._02_BlockModels
{
    public class CreateBlockViewModel
    {
        public MapDetail MapModel { get; set; }
        public BlockCreate CreateBlockModel { get; set; }
        public IEnumerable<BlockListItem> BlockLists { get; set; }

        public CreateBlockViewModel()
        {
            CreateBlockModel = new BlockCreate();
        }
    }
}
