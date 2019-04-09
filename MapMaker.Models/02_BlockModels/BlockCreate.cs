using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models._02_BlockModels
{
    public class BlockCreate
    {
        public int MapID { get; set; }

        [Required]
        public string Type { get; set; }
        
        [Required]
        [MinLength(2, ErrorMessage = "Please enter at least 2 characters.")]
        [MaxLength(20, ErrorMessage = "Map name cannot exceed 20 characters.")]
        public string Name { get; set; }
        
        [MaxLength(250, ErrorMessage = "Descriptions cannot exceed 250 characters.")]
        public string Description { get; set; }

        [Required]
        public int PosX { get; set; }

        [Required]
        public int PosY { get; set; }

        public string ExitDirection { get; set; }

        public int ExitToID { get; set; }

    }

}
