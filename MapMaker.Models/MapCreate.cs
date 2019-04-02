using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Models
{
    public class MapCreate
    {
        [Required]
        [MinLength(2, ErrorMessage = "Please enter at least 2 characters.")]
        [MaxLength(20, ErrorMessage = "Map name cannot exceed 20 characters.")]
        public string Name { get; set; }

        [Required]
        [MaxLength(250, ErrorMessage = "Descriptons cannot exceed 250 characters.")]
        public string Description { get; set; }

        [Required]
        public int SizeX { get; set; }

        [Required]
        public int SizeY { get; set; }

        //public override string ToString()
        //{
        //    return base.ToString();
        //}
    }
}
