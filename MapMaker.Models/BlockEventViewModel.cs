using MapMaker.Models._02_BlockModels;
using MapMaker.Models._03_GameEventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models
{
    public class BlockEventViewModel
    {
        public GameEventCreate CreateEvent { get; set; }
        public BlockDetail DetailOfBlock { get; set; }
        public IEnumerable<GameEventListItem> GameEvents { get; set; }

        public BlockEventViewModel()
        {
            CreateEvent = new GameEventCreate();
        }
    }
}
