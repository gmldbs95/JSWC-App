using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace massage.Models
{
    public class PAvailTime
    {
        [Key]
        public int PAvailTimeId { get; set; }

        public int PractitionerId { get; set; }

        public User Practitioner { get; set; }

        public int TimeslotId { get; set; }

        public Timeslot TimeSlot { get; set; }
    }
}