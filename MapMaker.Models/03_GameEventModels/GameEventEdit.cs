using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models._03_GameEventModels
{
    public class GameEventEdit
    {
        public int ID { get; set; }

        public int BlockID { get; set; }

        public int MapID { get; set; }

        public string Creator { get; set; }

        public string TypeOfEvent { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
