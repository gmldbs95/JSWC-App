using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using massage.Models;
using Microsoft.EntityFrameworkCore;



namespace massage.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(255, ErrorMessage="First Name may not be more than 255 characters long")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(255, ErrorMessage="Last Name may not be more than 255 characters long")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [MinLength(2, ErrorMessage="Username must be at least 2 characters long")]
        [MaxLength(29, ErrorMessage="Username must be less than 30 characters long")]
        public string UserName { get; set; }

        [Required]
        [MinLength(8, ErrorMessage="Password must be at least 8 characters in length.")]
        [DataType(DataType.Password)]

        public string Password { get; set; }

        [NotMapped]
        [Required]
        [Compare("Password")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]

        public string ConfirmPassword { get; set; }
        [Display(Name = "Role")]
        public int Role { get; set; } = 0; // Set Role by default to 0 for unassigned (1 = Practitioner, 2 = Receptionist, 5 = Admin)
        
        public List<PSchedule> PSchedules { get; set; }

        public List<PAvailTime> AvailTimes { get; set; }

        [InverseProperty("Creator")]
        public List<Reservation> CreatedReservations { get; set; }

        [InverseProperty("Practitioner")]
        public List<Reservation> Appointments { get; set; }

        public List<PService> Services { get; set; }

        public List<PInsurance> InsurancesAccepted { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

    }
}