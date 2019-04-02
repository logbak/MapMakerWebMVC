using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Data
{
    public enum BlockType { Exit, Object, Wall, NPC}

    public class Block
    {
        [Key]
        public int BlockID { get; set; }

        [Key]
        public int MapID { get; set; }

        [Required]
        public Guid OwnerID { get; set; }

        [Required]
        public BlockType TypeOfBlock { get; set; }

        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }

        [Required]
        public int PosX { get; set; }

        [Required]
        public int PosY { get; set; }

    }
}
