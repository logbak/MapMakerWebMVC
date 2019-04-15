using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models._03_GameEventModels
{
    public class GameEventCreate
    {
        public int ID { get; set; }
        public int BlockID { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "Please enter at least 2 characters.")]
        [MaxLength(20, ErrorMessage = "Event name cannot exceed 20 characters.")]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000, ErrorMessage = "Descriptions cannot exceed 1000 characters.")]
        public string Description { get; set; }
    }
}
