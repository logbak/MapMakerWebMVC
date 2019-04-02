using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Data
{
    public class Map
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public Guid OwnerID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int SizeX { get; set; }

        [Required]
        public int SizeY { get; set; }

        public string BlockIDs { get; set; }
    }
}
