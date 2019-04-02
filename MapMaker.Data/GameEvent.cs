using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapMaker.Data
{
    public enum EventType { Dialog, Minigame, Other}

    public class GameEvent
    {
        [Key]
        public int GameEventID { get; set; }

        public Guid OwnerID { get; set; }

        [Key]
        public int BlockID { get; set; }

        [Required]
        public EventType TypeOfEvent { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }


    }
}
