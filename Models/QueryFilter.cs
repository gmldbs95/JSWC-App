using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace massage.Models
{
    public static class QueryFilter
    {
        public static List<Timeslot> ByService(int SId, List<Timeslot> currentList, ProjectContext db)
        {
            List<Timeslot> pFilteredTimeslots = new List<Timeslot>();
            List<Timeslot> FinalFilter = new List<Timeslot>();
            List<Room> PossibleRooms = db.Rooms.Include(r => r.Services).Where(r => r.Services.Any(s => s.ServiceId == SId)).ToList(); // filter rooms by services possible in rooms
            List<User> filtPs = db.Users.Include(u => u.Services).Include(u => u.PSchedules).Where(u => u.Services.Any(s => s.ServiceId == SId)).ToList(); // filter practitioners by services they can perform
            // filter timeslots by practitioners schedules available
            bool isPAvail;
            foreach (Timeslot ts in currentList)
            {
                isPAvail = false;
                foreach (PAvailTime pat in ts.PsAvail)
                {
                    if (filtPs.IndexOf(pat.Practitioner) != -1) // if the practitioner in this timeslot's availability list is in our filtered acceptable practitioners list
                    {
                        isPAvail = true;
                    }
                }
                if (isPAvail == true) // if a practitioner that we can use for this service is available at this timeslot
                {
                    pFilteredTimeslots.Add(ts);
                }
            }
         
            // filter based on rooms available
            int count;
            foreach (Timeslot pts in pFilteredTimeslots)
            {
                count = 0;
                foreach (Reservation resv in pts.Reservations)
                {
                    if (PossibleRooms.IndexOf(resv.Room) != -1) // the room for this reservation is in our list of possible rooms for this service
                    {
                        count++;
                    }
                }
                if (count < PossibleRooms.Count)
                {
                    FinalFilter.Add(pts);
                }
            }
            Console.WriteLine("TOTAL FILTPS:", filtPs.Count);
            return FinalFilter;
        }

        public static List<Timeslot> ByPractitioner(int PId, List<Timeslot> currentList, ProjectContext db)
        {
            List<Timeslot> FilteredList = new List<Timeslot>();
            foreach (Timeslot ts in currentList)
            {
                foreach (PAvailTime pat in ts.PsAvail)
                {
                    if (pat.PractitionerId == PId)
                    {
                        FilteredList.Add(ts);
                    }
                }
            }
            return FilteredList;
        }
        public static List<Timeslot> ByTime(int Hour, List<Timeslot> currentList, ProjectContext db)
        {
            List<Timeslot> FilteredList = new List<Timeslot>();
            foreach (Timeslot ts in currentList)
            {
                if (ts.Hour == Hour)
                {
                    FilteredList.Add(ts);
                }
            }
            return FilteredList;
        }
        public static List<Timeslot> ByDate(DateTime date, List<Timeslot> currentList, ProjectContext db)
        {
            List<Timeslot> FilteredList = new List<Timeslot>();
            foreach (Timeslot ts in currentList)
            {
                if (ts.Date == date)
                {
                    FilteredList.Add(ts);
                }
            }
            return FilteredList;
        }

        public static List<Timeslot> ByCustomer(int CId, List<Timeslot> currentList, ProjectContext db) // filter based on the customer's insurance
        {
            List<Timeslot> FilteredList = new List<Timeslot>();
            Customer thisCustomer = db.Customers.Include(c => c.Insurance).FirstOrDefault(c => c.CustomerId == CId);
            List<User> filtPs = db.Users.Include(u => u.InsurancesAccepted).ThenInclude(i => i.Insurance)
                .Where(p => p.InsurancesAccepted.Any(ia => ia.InsuranceId == thisCustomer.Insurance.InsuranceId)).ToList(); // get all practitioners who accept the customer's insurance
            bool isPAvail;
            foreach (Timeslot ts in currentList)
            {
                isPAvail = false;
                foreach (PAvailTime pat in ts.PsAvail)
                {
                    if (filtPs.IndexOf(pat.Practitioner) != -1)
                    {
                        isPAvail = true;
                    }
                }
                if (isPAvail == true)
                {
                    FilteredList.Add(ts);
                }
            }
            return FilteredList;
        }
    }
}