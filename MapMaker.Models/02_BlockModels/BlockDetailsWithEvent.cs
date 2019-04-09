using MapMaker.Models._03_GameEventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models._02_BlockModels
{
    public class BlockDetailsWithEvent
    {
        public BlockDetail BlockDetail { get; set; }
        public GameEventDetail GameEventDetail { get; set; }

        public BlockDetailsWithEvent()
        {
            GameEventDetail = new GameEventDetail();
        }
    }
}
