using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace massage.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        public string Notes { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public int PractitionerId { get; set; }
        public User Practitioner { get; set; }

        public int CreatorId { get; set; }
        public User Creator { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        public int TimeslotId { get; set; }
        public Timeslot Timeslot { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}