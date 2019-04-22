using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models._05_ExploreModels
{
    public class Exploration
    {
        public List<int> AvailableMaps { get; set; }

        public int MapID { get; set; }

        public string MapPreview { get; set; }

        public List<string> IconOptions { get; set; }

        public string PlayerIcon { get; set; }

        public int PosX { get; set; }

        public int PosY { get; set; }

        public int SizeX { get; set; }

        public int SizeY { get; set; }

        public List<string> OccupiedAreas { get; set; }

        public List<string> Descriptions { get; set; }

        public List<string> Events { get; set; }

        public List<string> ExitsInfo { get; set; }

        public Exploration()
        {
            IconOptions = new List<string>();
            IconOptions.AddRange(new string[] { "\u263b", "\u2605", "\u2b24", "\u2689", "\u2689" }); 
            PlayerIcon = "\u263b";
        }

    }
}
