﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models
{
    public class MapListItem
    {
        public int ID { get; set; }
        public string Creator { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
    }
}
