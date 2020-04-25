using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace massage.Models
{
    public class RoomService
    {
        [Key]
        public int RoomServiceId { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}