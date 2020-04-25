using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace massage.Models
{
    public class PSchedule
    {
        [Key]
        public int PScheduleId { get; set; }

        public int PractitionerId { get; set; }

        public User Practitioner { get; set; }

        public string DayOfWeek { get; set; } // Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday
        
        // times available

        public bool t6 { get; set; } = false; // 6am
        public bool t7 { get; set; } = false; // 7am
        public bool t8 { get; set; } = false; // 8am
        public bool t9 { get; set; } = true; // 9am
        public bool t10 { get; set; } = true; // 10am
        public bool t11 { get; set; } = true; // 11am
        public bool t12 { get; set; } = true; // noon
        public bool t13 { get; set; } = true; // 1pm
        public bool t14 { get; set; } = true; // 2pm
        public bool t15 { get; set; } = true; // 3pm
        public bool t16 { get; set; } = true; // 4pm
        public bool t17 { get; set; } = true; // 5pm
        public bool t18 { get; set; } = false; // 6pm

        public bool Approved { get; set; } = false; // Whether schedule has been approved by an Admin

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}