using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace massage.Models
{
    public class PService
    {
        [Key]
        public int PServiceId { get; set; }

        public int PractitionerId { get; set; }
        public User Practitioner { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }
}