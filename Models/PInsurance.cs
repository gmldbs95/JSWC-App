using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
namespace massage.Models
{
    public class PInsurance
    {
        [Key]
        public int PInsuranceId { get; set; }

        public int InsuranceId { get; set; }
        public Insurance Insurance { get; set; }

        public int PractitionerId { get; set; }
        public User Practitioner { get; set; }
    }
}