using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Identity;
namespace massage.Models
{
    public static class Testing
    {
        public static bool CreateUser(ProjectContext db)
        {
            //User Level 1: Practitioner
            for (int i = 0; i < 11; i++)
            {
                User newUser = new User();
                newUser.UserName = $"Practitioner{i}";
                newUser.Password = "password";
                newUser.FirstName = $"Practitioner{i}";
                newUser.LastName = $"LastName";
                newUser.Role = 1;

                if (db.Users.Any(u => u.UserName == newUser.UserName))
                { //userName already in use
                    return false;
                }
                else
                {//user not in db, can register
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    db.Add(newUser);
                    db.SaveChanges();
                }
            }

            //User Level 2: Receptionist
            for (int i = 0; i < 4; i++)
            {
                User newUser = new User();
                newUser.UserName = $"Receptionist{i}";
                newUser.Password = "password";
                newUser.FirstName = $"Receptionist{i}";
                newUser.LastName = $"LastName";
                newUser.Role = 2;

                if (db.Users.Any(u => u.UserName == newUser.UserName))
                { //userName already in use
                    return false;
                }
                else
                {//user not in db, can register
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    db.Add(newUser);
                    db.SaveChanges();
                }
            }
            return true;
        }

        public static bool CreateServices(ProjectContext db)
        {
            Service serv1 = new Service();
            serv1.Name = "Massage";

            Service serv2 = new Service();
            serv2.Name = "Accupuncture";


            if (db.Services.Any(s => s.Name == serv1.Name))
            { //Name already in use
                return false;
            }
            else
            {
                db.Add(serv1);
                db.Add(serv2);
                db.SaveChanges();
            }
            return true;
        }



        public static bool CreateCustomers(ProjectContext db)
        {
            CreateInsurances(db);
            Customer newCust = new Customer();
            newCust.Address1 = "123 Fake St";
            newCust.Address2 = "Apt 4";
            newCust.City = "Boston";
            newCust.Email = "fakeUser@fake.com";
            newCust.FirstName = "Bob";
            newCust.LastName = "Barker";
            newCust.Notes = "this guy always cancels";
            newCust.Phone = "1234567890";
            newCust.State = "MA";
            newCust.Zip = 02115;
            Insurance newCustInsurance = new Insurance();
            newCust.InsuranceId = 2;
            newCust.Insurance = newCustInsurance;
            newCustInsurance.Name = "Aetna";
            Customer newCust2 = new Customer();
            newCust2.Address1 = "4324 bleh ave";
            newCust2.Address2 = "Apt 1";
            newCust2.City = "San Francisco";
            newCust2.Email = "SomePerson@aol.com";
            newCust2.FirstName = "Mary";
            newCust2.LastName = "Sue";
            newCust2.Notes = "so friendly!";
            newCust2.Phone = "9087654321";
            newCust2.State = "CA";
            newCust2.Zip = 94611;
            Insurance newCust2Insurance = new Insurance();
            newCust2.InsuranceId = 2;
            newCust2.Insurance = newCust2Insurance;
            newCust2Insurance.Name = "Premera";

            if (db.Customers.Any(c => c.CustomerId == newCust.CustomerId))
            { //Name already in use
                return false;
            }
            else
            {
                db.Add(newCust);
                db.Add(newCust2);
                db.SaveChanges();
            }
            return true;
        }


        public static bool CreateInsurances(ProjectContext db)
        {
            Insurance Ins1 = new Insurance();
            Ins1.Name = "Cash";

            Insurance Ins2 = new Insurance();
            Ins2.Name = "Premera";

            Insurance Ins3 = new Insurance();
            Ins3.Name = "Aetna";

            if (db.Insurances.Any(I => I.InsuranceId == Ins1.InsuranceId))
            { //Name already in use
                return false;
            }
            else
            {
                db.Add(Ins1);
                db.Add(Ins2);
                db.Add(Ins3);
                db.SaveChanges();
            }
            return true;
        }

        public static bool CreateTimeslots(ProjectContext db)
        {
            return true;
        }

        public static bool CreatePSchedule(ProjectContext db, int PracId)
        {
            User practitioner = db.Users.FirstOrDefault(u => u.UserId == PracId);
            PSchedule monday = new PSchedule();
            monday.PractitionerId = practitioner.UserId;
            monday.DayOfWeek = "Monday";
            db.Add(monday);
            PSchedule tuesday = new PSchedule();
            tuesday.PractitionerId = practitioner.UserId;
            tuesday.DayOfWeek = "Tuesday";
            db.Add(tuesday);
            PSchedule wednesday = new PSchedule();
            wednesday.PractitionerId = practitioner.UserId;
            wednesday.DayOfWeek = "Wednesday";
            db.Add(wednesday);
            PSchedule thursday = new PSchedule();
            thursday.PractitionerId = practitioner.UserId;
            thursday.DayOfWeek = "Thursday";
            db.Add(thursday);
            PSchedule friday = new PSchedule();
            friday.PractitionerId = practitioner.UserId;
            friday.DayOfWeek = "Friday";
            return true;
        }















    }
}