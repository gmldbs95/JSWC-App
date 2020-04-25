using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace massage.Models
{
    public static class Query
    {
        // Customer Queries
        public static List<Customer> AllCustomers(ProjectContext db)
        {
            return db.Customers
                .Include(c => c.Insurance)
                .Include(c => c.Reservations).ThenInclude(r => r.Practitioner)
                .Include(c => c.Reservations).ThenInclude(r => r.Creator)
                .Include(c => c.Reservations).ThenInclude(r => r.Service)
                .Include(c => c.Reservations).ThenInclude(r => r.Room)
                .Include(c => c.Reservations).ThenInclude(r => r.Timeslot)
                .ToList();
        }
        public static Customer OneCustomer(int custID, ProjectContext db)
        {
            return db.Customers
                .Include(c => c.Insurance)
                .Include(c => c.Reservations).ThenInclude(r => r.Practitioner)
                .Include(c => c.Reservations).ThenInclude(r => r.Creator)
                .Include(c => c.Reservations).ThenInclude(r => r.Service)
                .Include(c => c.Reservations).ThenInclude(r => r.Room)
                .Include(c => c.Reservations).ThenInclude(r => r.Timeslot)
                .FirstOrDefault(c => c.CustomerId == custID);
        }
        public static Customer CreateCustomer(Customer newC, ProjectContext db)
        {
            db.Add(newC);
            db.SaveChanges();
            return newC;
        }
        public static Customer EditCustomer(Customer editedC, ProjectContext db)
        {
            Customer cToEdit = db.Customers.FirstOrDefault(c => c.CustomerId == editedC.CustomerId);
            cToEdit.Address1 = editedC.Address1;
            cToEdit.Address2 = editedC.Address2;
            cToEdit.City = editedC.City;
            cToEdit.Email = editedC.Email;
            cToEdit.FirstName = editedC.FirstName;
            cToEdit.Insurance = editedC.Insurance;
            cToEdit.LastName = editedC.LastName;
            cToEdit.Notes = editedC.Notes;
            cToEdit.Phone = editedC.Phone;
            cToEdit.State = editedC.State;
            cToEdit.UpdatedAt = DateTime.Now;
            cToEdit.Zip = editedC.Zip;
            db.SaveChanges();
            return db.Customers
                .Include(c => c.Insurance)
                .Include(c => c.Reservations).ThenInclude(r => r.Practitioner)
                .Include(c => c.Reservations).ThenInclude(r => r.Creator)
                .Include(c => c.Reservations).ThenInclude(r => r.Service)
                .Include(c => c.Reservations).ThenInclude(r => r.Room)
                .Include(c => c.Reservations).ThenInclude(r => r.Timeslot)
                .FirstOrDefault(c => c.CustomerId == cToEdit.CustomerId);
        }
        public static void DeleteCustomer(int custID, ProjectContext db)
        {
            Customer cToRemove = db.Customers.FirstOrDefault(c => c.CustomerId == custID);
            db.Remove(cToRemove);
            db.SaveChanges();
            return;
        }
        // Insurance Queries
        public static List<Insurance> AllInsurances(ProjectContext db)
        {
            return db.Insurances
                .Include(i => i.Customers)
                .Include(i => i.Practitioners)
                .ToList();
        }
        public static Insurance OneInsurance(int insID, ProjectContext db)
        {
            return db.Insurances
                .Include(i => i.Customers)
                .Include(i => i.Practitioners)
                .FirstOrDefault(i => i.InsuranceId == insID);
        }
        public static Insurance EditInsurance(Insurance editedIns, ProjectContext db)
        {
            Insurance insToEdit = db.Insurances.FirstOrDefault(i => i.InsuranceId == editedIns.InsuranceId);
            insToEdit.Name = editedIns.Name;
            insToEdit.UpdatedAt = DateTime.Now;
            db.SaveChanges();
            return db.Insurances
                .Include(i => i.Customers)
                .Include(i => i.Practitioners)
                .FirstOrDefault(i => i.InsuranceId == insToEdit.InsuranceId);
        }
        public static Insurance CreateInsurance(Insurance newIns, ProjectContext db)
        {
            db.Add(newIns);
            db.SaveChanges();
            return newIns;
        }
        public static void DeleteInsurance(int insID, ProjectContext db)
        {
            Insurance insToDelete = db.Insurances.FirstOrDefault(i => i.InsuranceId == insID);
            db.Remove(insToDelete);
            db.SaveChanges();
            return;
        }
        // User Queries
        public static List<User> AllUsers(ProjectContext db)
        {
            return db.Users
                .Include(u => u.PSchedules)
                .Include(u => u.AvailTimes)
                .ThenInclude(pat => pat.TimeSlot)
                .Include(u => u.CreatedReservations)
                .Include(u => u.Appointments)
                .Include(u => u.Services)
                .Include(u => u.InsurancesAccepted)
                .ToList();
        }
        public static List<User> AllPractitioners(ProjectContext db)
        {
            return db.Users
                .Include(u => u.PSchedules)
                .Include(u => u.AvailTimes)
                .ThenInclude(pat => pat.TimeSlot)
                .Include(u => u.Appointments)
                .Include(u => u.Services)
                .Include(u => u.InsurancesAccepted)
                .Where(u => u.Role == 1)
                .ToList();
        }
        public static List<User> AllReceptionists(ProjectContext db)
        {
            return db.Users
                .Include(u => u.CreatedReservations)
                .Where(u => u.Role == 2)
                .ToList();
        }
        public static User OnePractitioner(int pID, ProjectContext db)
        {
            return db.Users
                .Include(u => u.PSchedules)
                .Include(u => u.AvailTimes)
                .ThenInclude(pat => pat.TimeSlot)
                .Include(u => u.Appointments)
                .Include(u => u.Services)
                .Include(u => u.InsurancesAccepted)
                .Where(u => u.Role == 1)
                .FirstOrDefault(u => u.UserId == pID);
        }
        public static User OneReceptionist(int rID, ProjectContext db)
        {
            return db.Users
                .Include(u => u.CreatedReservations)
                .Where(u => u.Role == 2)
                .FirstOrDefault(u => u.UserId == rID);
        }
        public static User EditUser(User editedUser, ProjectContext db)
        {
            User userToEdit = db.Users.FirstOrDefault(u => u.UserId == editedUser.UserId);
            userToEdit.FirstName = editedUser.FirstName;
            userToEdit.LastName = editedUser.LastName;
            if (editedUser.Password.Length > 7) {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                userToEdit.Password = Hasher.HashPassword(editedUser, editedUser.Password);
            }
            userToEdit.Role = editedUser.Role;
            userToEdit.UpdatedAt = DateTime.Now;
            userToEdit.UserName = editedUser.UserName;
            db.SaveChanges();
            return db.Users
                .Include(u => u.PSchedules)
                .Include(u => u.AvailTimes)
                .ThenInclude(pat => pat.TimeSlot)
                .Include(u => u.CreatedReservations)
                .Include(u => u.Appointments)
                .Include(u => u.Services)
                .Include(u => u.InsurancesAccepted)
                .FirstOrDefault(u => u.UserId == userToEdit.UserId);
        }
        public static void DeleteUser(int Id, ProjectContext db)
        {
            User userToDel = db.Users.FirstOrDefault(u => u.UserId == Id);
            db.Remove(userToDel);
            db.SaveChanges();
            return;
        }
        // Practitioner Availability
        public static List<PAvailTime> AllPractitionerAvailabilities(ProjectContext db)
        {
            return db.PAvailTimes
                .Include(pat => pat.Practitioner)
                .Include(pat => pat.TimeSlot)
                .ToList();
        }
        public static List<PAvailTime> OnePractitionersAvailabilities(int pID, ProjectContext db)
        {
            return db.PAvailTimes
                .Include(pat => pat.Practitioner)
                .Include(pat => pat.TimeSlot)
                .Where(pat => pat.PractitionerId == pID)
                .ToList();
        }
        public static PAvailTime CreatePractitionerAvailability(int pID, int tsID, ProjectContext db)
        {
            PAvailTime newPAT = new PAvailTime();
            newPAT.PractitionerId = pID;
            newPAT.TimeslotId = tsID;
            db.Add(newPAT);
            db.SaveChanges();
            return db.PAvailTimes
                .Include(pat => pat.Practitioner)
                .Include(pat => pat.TimeSlot)
                .Where(pat => pat.PractitionerId == pID)
                .FirstOrDefault(pat => pat.TimeslotId == tsID);
        }
        public static void DeletePractitionerAvailability(int patID, ProjectContext db)
        {
            PAvailTime patToDel = db.PAvailTimes.FirstOrDefault(pat => pat.PAvailTimeId == patID);
            db.Remove(patToDel);
            db.SaveChanges();
            return;
        }
        public static List<PAvailTime> UpdateAllOfOnePsAvails(int pID, List<PAvailTime> updatedPATs, ProjectContext db)
        {
            List<PAvailTime> oldPats = db.PAvailTimes.Where(pat => pat.PractitionerId == pID).ToList();
            foreach (PAvailTime oldPat in oldPats)
            {
                PAvailTime patToDel = db.PAvailTimes.FirstOrDefault(pat => pat.PAvailTimeId == oldPat.PAvailTimeId);
                db.Remove(patToDel);
            }
            db.SaveChanges();
            foreach (PAvailTime updatedPat in updatedPATs)
            {
                PAvailTime newPAT = new PAvailTime();
                newPAT.PractitionerId = updatedPat.PractitionerId;
                newPAT.TimeslotId = updatedPat.TimeslotId;
                db.Add(newPAT);
            }
            db.SaveChanges();
            return db.PAvailTimes
                .Include(pat => pat.Practitioner)
                .Include(pat => pat.TimeSlot)
                .Where(pat => pat.PractitionerId == pID)
                .ToList();
        }
        // Practitioner's Services Queries
        public static List<PService> OnePsServices(int pID, ProjectContext db)
        {
            return db.PServices
                .Include(ps => ps.Practitioner)
                .Include(ps => ps.Service)
                .ToList();
        }
        public static PService OnePService(int psID, ProjectContext db)
        {
            return db.PServices
                .Include(ps => ps.Practitioner)
                .Include(ps => ps.Service)
                .FirstOrDefault(ps => ps.PServiceId == psID);
        }
        public static PService CreatePService(PService newPS, ProjectContext db)
        {
            db.Add(newPS);
            db.SaveChanges();
            return newPS;
        }
        public static void DeletePService(int psID, ProjectContext db)
        {
            PService psToDel = db.PServices.FirstOrDefault(ps => ps.PServiceId == psID);
            db.Remove(psToDel);
            db.SaveChanges();
            return;
        }
        public static List<PService> UpdateAllOfOnePsServices(int pID, List<PService> updatedPServices, ProjectContext db)
        {
            List<PService> oldPServices = db.PServices.Where(ps => ps.PractitionerId == pID).ToList();
            foreach (PService oldPS in oldPServices)
            {
                PService psToDel = db.PServices.FirstOrDefault(ps => ps.PServiceId == oldPS.PServiceId);
                db.Remove(psToDel);
            }
            db.SaveChanges();
            foreach (PService updatedPS in updatedPServices)
            {
                PService newPS = new PService();
                newPS.PractitionerId = updatedPS.PractitionerId;
                newPS.ServiceId = updatedPS.ServiceId;
                db.Add(newPS);
            }
            db.SaveChanges();
            return db.PServices
                .Include(ps => ps.Practitioner)
                .Include(ps => ps.Service)
                .ToList();
        }
        // Practitioner Insurance Queries
        public static List<PInsurance> OnePsInsurances(int pID, ProjectContext db)
        {
            return db.PInsurances
                .Include(pi => pi.Insurance)
                .Include(pi => pi.Practitioner)
                .Where(pi => pi.PractitionerId == pID)
                .ToList();
        }
        public static PInsurance OnePInsurance(int piID, ProjectContext db)
        {
            return db.PInsurances
                .Include(pi => pi.Insurance)
                .Include(pi => pi.Practitioner)
                .FirstOrDefault(pi => pi.PInsuranceId == piID);
        }
        public static PInsurance CreatePInsurance(PInsurance newPI, ProjectContext db)
        {
            db.Add(newPI);
            db.SaveChanges();
            return newPI;
        }
        public static void DeletePInsurance(int piID, ProjectContext db)
        {
            PInsurance piToDel = db.PInsurances.FirstOrDefault(pi => pi.PInsuranceId == piID);
            db.Remove(piToDel);
            db.SaveChanges();
            return;
        }
        public static List<PInsurance> UpdateAllOfOnePsInsurances(int pID, List<PInsurance> updatedPIs, ProjectContext db)
        {
            List<PInsurance> oldPInsurances = db.PInsurances.Where(pi => pi.PractitionerId == pID).ToList();
            foreach (PInsurance oldPI in oldPInsurances)
            {
                PInsurance piToDel = db.PInsurances.FirstOrDefault(pi => pi.PInsuranceId == oldPI.PInsuranceId);
                db.Remove(piToDel);
            }
            db.SaveChanges();
            foreach (PInsurance updatedPI in updatedPIs)
            {
                PInsurance newPI = new PInsurance();
                newPI.PractitionerId = updatedPI.PractitionerId;
                newPI.InsuranceId = updatedPI.InsuranceId;
                db.Add(newPI);
            }
            db.SaveChanges();
            return db.PInsurances
                .Include(pi => pi.Insurance)
                .Include(pi => pi.Practitioner)
                .Where(pi => pi.PractitionerId == pID)
                .ToList();
        }
        // Practitioner Schedule (template) Queries
        public static List<PSchedule> OnePsSchedules(int pID, ProjectContext db)
        {
            List<PSchedule> existingPSs = db.PSchedules
                .Include(ps => ps.Practitioner)
                .Where(ps => ps.PractitionerId == pID)
                .OrderByDescending(ps => ps.UpdatedAt)
                .ToList();
            if (existingPSs.Count != 7) // if there are 0, or if an error has happened and more or less than 7 exist, let's reset
            {
                foreach (PSchedule oldPS in existingPSs)
                {
                    db.Remove(oldPS);
                }
                db.SaveChanges();
                string[] days = new string[]{"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"};
                foreach (string day in days)
                {
                    PSchedule newPS = new PSchedule();
                    newPS.DayOfWeek = day;
                    newPS.PractitionerId = pID;
                    newPS.Approved = false;
                    db.Add(newPS);
                }
                db.SaveChanges();
                existingPSs = db.PSchedules
                    .Include(ps => ps.Practitioner)
                    .Where(ps => ps.PractitionerId == pID)
                    .OrderByDescending(ps => ps.UpdatedAt)
                    .ToList();
            }
            return existingPSs;
        }
        public static List<PSchedule> UpdateAllOfOnePsSchedules(int pID, List<PSchedule> updatedPSchedules, ProjectContext db)
        {
            if (updatedPSchedules.Count != 7)
            {
                System.Console.WriteLine("Error: The list of PSchedules provided to update didn't contain 7 PSchedules!  You need one for each day!");
                return updatedPSchedules;
            }
            List<PSchedule> oldPSs = db.PSchedules.Where(ps => ps.PractitionerId == pID).ToList();
            foreach (PSchedule oldPS in oldPSs)
            {
                db.Remove(oldPS);
            }
            db.SaveChanges();
            List<PAvailTime> thisPsPats = Query.OnePractitionersAvailabilities(pID, db);
            foreach (PAvailTime patToDel in thisPsPats)
            {
                db.Remove(patToDel);
            }
            db.SaveChanges();
            foreach (PSchedule newPS in updatedPSchedules)
            {
                db.Add(newPS);
                for (int h=6; h<=18; h++) {
                    if ((bool)newPS.GetType().GetProperty("t" + h).GetValue(newPS))
                    {
                        List<Timeslot> matchingTSs = db.Timeslots.Where(ts => ts.Date.DayOfWeek.ToString() == newPS.DayOfWeek)
                            .Where(ts => ts.Hour == h).ToList();
                        foreach (Timeslot tsToAdd in matchingTSs)
                        {
                            PAvailTime newPat = new PAvailTime();
                            newPat.TimeslotId = tsToAdd.TimeslotId;
                            newPat.PractitionerId = pID;
                            db.Add(newPat);
                        }

                    }
                    
                }
            }
            db.SaveChanges();
            return updatedPSchedules;
        }
        public static List<PSchedule> ApproveAllOfOnePsSchedules(int pID, ProjectContext db)
        {
            List<PSchedule> PSchedules = db.PSchedules.Where(ps => ps.PractitionerId == pID).ToList();
            if (PSchedules.Count != 7)
            {
                System.Console.WriteLine("Error: That practitioner does not have exactly 7 PSchedules and thus cannot be approved.  There should be one for each day!");
                return PSchedules;
            }
            foreach (PSchedule ps in PSchedules)
            {
                ps.Approved = true;
            }
            db.SaveChanges();
            return PSchedules;
        }
        // Query for which PSchedules haven't been approved yet to show to admin
        public static List<PSchedule> PSchedulesNeedingApproval(ProjectContext db)
        {
            return db.PSchedules
                .Include(ps => ps.Practitioner)
                .Where(ps => ps.Approved == false)
                .OrderByDescending(ps => ps.UpdatedAt)
                .ToList();
        }
        // Reservation Queries
        public static List<Reservation> AllReservations(ProjectContext db)
        {
            return db.Reservations
                .Include(r => r.Creator)
                .Include(r => r.Customer)
                .Include(r => r.Practitioner)
                .Include(r => r.Room)
                .Include(r => r.Service)
                .Include(r => r.Timeslot)
                .OrderBy(r => r.Timeslot.Date)
                .ThenBy(r => r.Timeslot.Hour)
                .ToList();
        }
        public static List<Reservation> AllFutureReservations(ProjectContext db)
        {
            return db.Reservations
                .Include(r => r.Creator)
                .Include(r => r.Customer)
                .Include(r => r.Practitioner)
                .Include(r => r.Room)
                .Include(r => r.Service)
                .Include(r => r.Timeslot)
                .Where(r => r.Timeslot.Date >= DateTime.Today)
                .OrderBy(r => r.Timeslot.Date)
                .ThenBy(r => r.Timeslot.Hour)
                .ToList();
        }
        public static List<Reservation> AllPastReservations(ProjectContext db)
        {
            return db.Reservations
                .Include(r => r.Creator)
                .Include(r => r.Customer)
                .Include(r => r.Practitioner)
                .Include(r => r.Room)
                .Include(r => r.Service)
                .Include(r => r.Timeslot)
                .Where(r => r.Timeslot.Date < DateTime.Today)
                .OrderBy(r => r.Timeslot.Date)
                .ThenBy(r => r.Timeslot.Hour)
                .ToList();
        }
        public static List<Reservation> AllThisMonthsReservations(ProjectContext db)
        {
            return db.Reservations
                .Include(r => r.Creator)
                .Include(r => r.Customer)
                .Include(r => r.Practitioner)
                .Include(r => r.Room)
                .Include(r => r.Service)
                .Include(r => r.Timeslot)
                .Where(r => r.Timeslot.Date.Year == DateTime.Today.Year && r.Timeslot.Date.Month == DateTime.Today.Month)
                .OrderBy(r => r.Timeslot.Date)
                .ThenBy(r => r.Timeslot.Hour)
                .ToList();
        }
        public static List<Reservation> OnePThisMonthsReservations(int pID, ProjectContext db)
        {
            return db.Reservations
                .Include(r => r.Creator)
                .Include(r => r.Customer)
                .Include(r => r.Practitioner)
                .Include(r => r.Room)
                .Include(r => r.Service)
                .Include(r => r.Timeslot)
                .Where(r => r.Timeslot.Date.Year == DateTime.Today.Year && r.Timeslot.Date.Month == DateTime.Today.Month)
                .Where(r => r.PractitionerId == pID)
                .OrderBy(r => r.Timeslot.Date)
                .ThenBy(r => r.Timeslot.Hour)
                .ToList();
        }
        public static List<Reservation> AllThisWeeksReservations(ProjectContext db)
        {
            DateTime start = DateTime.Today;
            while (start.DayOfWeek.ToString() != "Sunday")
            {
                start = start.AddDays(-1);
            }
            // now we have 'start' stored as our start date for the week
            DateTime end = DateTime.Today;
            while (end.DayOfWeek.ToString() != "Saturday")
            {
                end = end.AddDays(1);
            }
            // now we have 'end' stored as our end date for the week
            return db.Reservations
                .Include(r => r.Creator)
                .Include(r => r.Customer)
                .Include(r => r.Practitioner)
                .Include(r => r.Room)
                .Include(r => r.Service)
                .Include(r => r.Timeslot)
                .Where(r => r.Timeslot.Date >= start && r.Timeslot.Date <= end)
                .OrderBy(r => r.Timeslot.Date)
                .ThenBy(r => r.Timeslot.Hour)
                .ToList();
        }
        public static List<Reservation> OnePThisWeeksReservations(int pID, ProjectContext db)
        {
            DateTime start = DateTime.Today;
            while (start.DayOfWeek.ToString() != "Sunday")
            {
                start = start.AddDays(-1);
            }
            // now we have 'start' stored as our start date for the week
            DateTime end = DateTime.Today;
            while (end.DayOfWeek.ToString() != "Saturday")
            {
                end = end.AddDays(1);
            }
            // now we have 'end' stored as our end date for the week
            return db.Reservations
                .Include(r => r.Creator)
                .Include(r => r.Customer)
                .Include(r => r.Practitioner)
                .Include(r => r.Room)
                .Include(r => r.Service)
                .Include(r => r.Timeslot)
                .Where(r => r.Timeslot.Date >= start && r.Timeslot.Date <= end)
                .Where(r => r.PractitionerId == pID)
                .OrderBy(r => r.Timeslot.Date)
                .ThenBy(r => r.Timeslot.Hour)
                .ToList();
        }
        public static List<Reservation> AllTodaysReservations(ProjectContext db)
        {
            return db.Reservations
                .Include(r => r.Creator)
                .Include(r => r.Customer)
                .Include(r => r.Practitioner)
                .Include(r => r.Room)
                .Include(r => r.Service)
                .Include(r => r.Timeslot)
                .Where(r => r.Timeslot.Date == DateTime.Today)
                .OrderBy(r => r.Timeslot.Hour)
                .ToList();
        }
        public static List<Reservation> OnePTodaysReservations(int pID, ProjectContext db)
        {
            return db.Reservations
                .Include(r => r.Creator)
                .Include(r => r.Customer)
                .Include(r => r.Practitioner)
                .Include(r => r.Room)
                .Include(r => r.Service)
                .Include(r => r.Timeslot)
                .Where(r => r.Timeslot.Date == DateTime.Today)
                .Where(r => r.PractitionerId == pID)
                .OrderBy(r => r.Timeslot.Hour)
                .ToList();
        }
        public static Reservation OneReservation(int rID, ProjectContext db)
        {
            return db.Reservations
                .Include(r => r.Creator)
                .Include(r => r.Customer)
                .Include(r => r.Practitioner)
                .Include(r => r.Room)
                .Include(r => r.Service)
                .Include(r => r.Timeslot)
                .FirstOrDefault(r => r.ReservationId == rID);
        }
        public static Reservation CreateReservation(Reservation newR, ProjectContext db)
        {
            db.Add(newR);
            db.SaveChanges();
            return newR;
        }
        public static void DeleteReservation(int rID, ProjectContext db)
        {
            Reservation rToDelete = db.Reservations.FirstOrDefault(r => r.ReservationId == rID);
            db.Remove(rToDelete);
            db.SaveChanges();
            return;
        }
        public static Reservation EditOneReservation(Reservation updatedR, ProjectContext db)
        {
            DeleteReservation(updatedR.ReservationId, db);
            Reservation newR = CreateReservation(updatedR, db);
            return db.Reservations
                .Include(r => r.Creator)
                .Include(r => r.Customer)
                .Include(r => r.Practitioner)
                .Include(r => r.Room)
                .Include(r => r.Service)
                .Include(r => r.Timeslot)
                .FirstOrDefault(r => r.ReservationId == newR.ReservationId);
        }
        // Service Queries
        public static List<Service> AllServices(ProjectContext db)
        {
            return db.Services
                .Include(s => s.Practitioners)
                .Include(s => s.Rooms)
                .ToList();
        }
        public static Service OneService(int sID, ProjectContext db)
        {
            return db.Services
                .Include(s => s.Practitioners)
                .Include(s => s.Rooms)
                .FirstOrDefault(s => s.ServiceId == sID);
        }
        public static Service CreateService(Service newS, ProjectContext db)
        {
            db.Add(newS);
            db.SaveChanges();
            List<Room> allRooms = db.Rooms.ToList(); // these next few lines create the default associations for all rooms with this new service
            foreach (Room room in allRooms)
            {
                RoomService newRS = new RoomService();
                newRS.RoomId = room.RoomId;
                newRS.ServiceId = newS.ServiceId;
            }
            db.SaveChanges();
            return newS;
        }
        public static void DeleteService(int sID, ProjectContext db)
        {
            Service sToDel = db.Services.FirstOrDefault(s => s.ServiceId == sID);
            db.Remove(sToDel);
            db.SaveChanges();
            return;
        }
        public static Service EditService(Service updatedS, ProjectContext db)
        {
            Service sToUpdate = db.Services.FirstOrDefault(s => s.ServiceId == updatedS.ServiceId);
            sToUpdate.Name = updatedS.Name;
            sToUpdate.UpdatedAt = DateTime.Now;
            db.SaveChanges();
            return sToUpdate;
        }
        public static List<Timeslot> AllTimeslots(ProjectContext db)
        {
            return db.Timeslots
                .Include(t => t.PsAvail)
                .ThenInclude(pat => pat.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Customer)
                .Include(t => t.Reservations).ThenInclude(r => r.Creator)
                .Include(t => t.Reservations).ThenInclude(r => r.Room)
                .Include(t => t.Reservations).ThenInclude(r => r.Service)
                .OrderBy(t => t.Date)
                .ThenBy(t => t.Hour)                
                .ToList();
        }
        public static List<Timeslot> AllFutureTimeslots(ProjectContext db)
        {
            return db.Timeslots
                .Include(t => t.PsAvail)
                .ThenInclude(pat => pat.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Customer)
                .Include(t => t.Reservations).ThenInclude(r => r.Creator)
                .Include(t => t.Reservations).ThenInclude(r => r.Room)
                .Include(t => t.Reservations).ThenInclude(r => r.Service)
                .Where(t => t.Date >= DateTime.Today)
                .OrderBy(t => t.Date)
                .ThenBy(t => t.Hour)                
                .ToList();
        }
        public static List<Timeslot> AllPastTimeslots(ProjectContext db)
        {
            return db.Timeslots
                .Include(t => t.PsAvail)
                .ThenInclude(pat => pat.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Customer)
                .Include(t => t.Reservations).ThenInclude(r => r.Creator)
                .Include(t => t.Reservations).ThenInclude(r => r.Room)
                .Include(t => t.Reservations).ThenInclude(r => r.Service)
                .Where(t => t.Date < DateTime.Today)
                .OrderBy(t => t.Date)
                .ThenBy(t => t.Hour)                
                .ToList();
        }
        public static List<Timeslot> ThisMonthsTimeslots(ProjectContext db)
        {
            return db.Timeslots
                .Include(t => t.PsAvail)
                .ThenInclude(pat => pat.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Customer)
                .Include(t => t.Reservations).ThenInclude(r => r.Creator)
                .Include(t => t.Reservations).ThenInclude(r => r.Room)
                .Include(t => t.Reservations).ThenInclude(r => r.Service)
                .Where(t => t.Date.Month == DateTime.Now.Month && t.Date.Year == DateTime.Now.Year)
                .OrderBy(t => t.Date)
                .ThenBy(t => t.Hour)                
                .ToList();
        }
        public static List<Timeslot> OneMonthsTimeslots(DateTime dateInMonth, ProjectContext db)
        {
            return db.Timeslots
                .Include(t => t.PsAvail)
                .ThenInclude(pat => pat.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Customer)
                .Include(t => t.Reservations).ThenInclude(r => r.Creator)
                .Include(t => t.Reservations).ThenInclude(r => r.Room)
                .Include(t => t.Reservations).ThenInclude(r => r.Service)
                .Where(t => t.Date.Month == dateInMonth.Month && t.Date.Year == DateTime.Now.Year)
                .OrderBy(t => t.Date)
                .ThenBy(t => t.Hour)                
                .ToList();
        }
        public static List<Timeslot> ThisWeeksTimeslots(ProjectContext db)
        {
            DateTime start = DateTime.Today;
            System.Console.WriteLine("Entering this weeks timeslots function");
            while (start.DayOfWeek.ToString() != "Sunday")
            {
                start = start.AddDays(-1);
            }
            // now we have 'start' stored as our start date for the week
            DateTime end = DateTime.Today;
            while (end.DayOfWeek.ToString() != "Saturday")
            {
                end = end.AddDays(1);
            }
            System.Console.WriteLine($"Start is {start.Day}, End is {end.Day}");
            // now we have 'end' stored as our end date for the week
            return db.Timeslots
                .Include(t => t.PsAvail)
                .ThenInclude(pat => pat.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Customer)
                .Include(t => t.Reservations).ThenInclude(r => r.Creator)
                .Include(t => t.Reservations).ThenInclude(r => r.Room)
                .Include(t => t.Reservations).ThenInclude(r => r.Service)
                .Where(t => t.Date >= start && t.Date <= end)
                .OrderBy(t => t.Date)
                .ThenBy(t => t.Hour)                
                .ToList();
        }
        public static List<Timeslot> OneWeeksTimeslots(DateTime startDay, ProjectContext db)
        {
            DateTime start = startDay;
            while (start.DayOfWeek.ToString() != "Sunday")
            {
                start = start.AddDays(-1);
            }
            // now we have 'start' stored as our start date for the week
            DateTime end = DateTime.Today;
            while (end.DayOfWeek.ToString() != "Saturday")
            {
                end = end.AddDays(1);
            }
            // now we have 'end' stored as our end date for the week
            return db.Timeslots
                .Include(t => t.PsAvail)
                .ThenInclude(pat => pat.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Customer)
                .Include(t => t.Reservations).ThenInclude(r => r.Creator)
                .Include(t => t.Reservations).ThenInclude(r => r.Room)
                .Include(t => t.Reservations).ThenInclude(r => r.Service)
                .Where(t => t.Date >= start && t.Date <= end)
                .OrderBy(t => t.Date)
                .ThenBy(t => t.Hour)                
                .ToList();
        }
        public static List<Timeslot> TodaysTimeslots(ProjectContext db)
        {
            return db.Timeslots
                .Include(t => t.PsAvail)
                .ThenInclude(pat => pat.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Customer)
                .Include(t => t.Reservations).ThenInclude(r => r.Creator)
                .Include(t => t.Reservations).ThenInclude(r => r.Room)
                .Include(t => t.Reservations).ThenInclude(r => r.Service)
                .Where(t => t.Date == DateTime.Today)
                .OrderBy(t => t.Hour)             
                .ToList();
        }
        public static List<Timeslot> OneDaysTimeslots(DateTime day, ProjectContext db)
        {
            return db.Timeslots
                .Include(t => t.PsAvail)
                .ThenInclude(pat => pat.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Customer)
                .Include(t => t.Reservations).ThenInclude(r => r.Creator)
                .Include(t => t.Reservations).ThenInclude(r => r.Room)
                .Include(t => t.Reservations).ThenInclude(r => r.Service)
                .Where(t => t.Date == day)
                .OrderBy(t => t.Hour)             
                .ToList();
        }
        public static Timeslot OneTimeslot(int tsID, ProjectContext db)
        {
            return db.Timeslots
                .Include(t => t.PsAvail)
                .ThenInclude(pat => pat.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Practitioner)
                .Include(t => t.Reservations).ThenInclude(r => r.Customer)
                .Include(t => t.Reservations).ThenInclude(r => r.Creator)
                .Include(t => t.Reservations).ThenInclude(r => r.Room)
                .Include(t => t.Reservations).ThenInclude(r => r.Service)
                .FirstOrDefault(t => t.TimeslotId == tsID);
        }
        public static Timeslot CreateTimeslot(Timeslot newTS, ProjectContext db)
        {
            db.Add(newTS);
            db.SaveChanges();
            return newTS;
        }
        public static void DeleteTimeslot(int tsID, ProjectContext db)
        {
            Timeslot tsToDel = db.Timeslots.FirstOrDefault(t => t.TimeslotId == tsID);
            db.Remove(tsToDel);
            db.SaveChanges();
            return;
        }
        public static Timeslot EditTimeslot(Timeslot editedTS, ProjectContext db) // Honestly we should just not use this because it could create really weird issues timeslots on the same time, etc
        {
            Timeslot tsToEdit = db.Timeslots.FirstOrDefault(t => t.TimeslotId == editedTS.TimeslotId);
            tsToEdit.Date = editedTS.Date;
            tsToEdit.Hour = editedTS.Hour;
            tsToEdit.UpdatedAt = DateTime.Now;
            db.SaveChanges();
            return tsToEdit;
        }
    }
}

