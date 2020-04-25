using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
namespace massage.Models
{
    public class ViewModel
    {
        public User CurrentUser { get; set; }

        public List<User> AllUsers { get; set; }

        public User OneUser { get; set; }

        public List<Timeslot> AllTimeslots { get; set; }

        public Timeslot OneTimeslot { get; set; }

        public List<Insurance> AllInsurances { get; set; }

        public Insurance OneInsurance { get; set; }

        public List<PSchedule> AllPSchedules { get; set; }

        public PSchedule OnePSchedule { get; set; }

        public Dictionary<string, Dictionary<string, bool>> PSDict { get; set; }

        public List<Customer> AllCustomers { get; set; }

        public Customer OneCustomer { get; set; }

        public List<Reservation> AllReservations { get; set; }

        public Reservation OneReservation { get; set; }

        public List<Service> AllServices { get; set; }
        public List<User> AllPractitioners { get; set; }

        public Service OneService { get; set; }

        public List<List<Timeslot>> OldQueries { get; set; }
        // Needed for _ASidebarPartial
        public bool NewEmps { get; set; }
    }
}