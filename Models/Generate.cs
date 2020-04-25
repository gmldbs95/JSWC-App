using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace massage.Models
{
    public static class Generate
    {
        
        // Check if Generation is needed
        public static void CheckTimeslots(ProjectContext db)
        {
            int daysAhead = 60; // this is the number of days in advance the system should keep timeslots built for
            Timeslot lastTS = db.Timeslots.OrderByDescending(t => t.Date).FirstOrDefault();
            if (lastTS == null)
            {
                GenerateTodaysTimeslots(db);
                GenerateTimeslots(daysAhead, lastTS, db);
                return;
            }
            else
            {
                if (lastTS.Date < DateTime.Today)
                {
                    GenerateTodaysTimeslots(db);
                }
                int daysToBuild = daysAhead - (int)(lastTS.Date - DateTime.Today).TotalDays; // difference between days we want to stay ahead and days the last existing timeslot is ahead of Now
                if (daysToBuild == 0)
                {
                    return;
                }
                else
                {
                    GenerateTimeslots(daysToBuild, lastTS, db);
                    return;
                }
            }
        }
        public static void GenerateTodaysTimeslots(ProjectContext db)
        {
            List<User> allPs = db.Users.Include(u => u.PSchedules).Where(u => u.Role == 1).ToList(); // all practitioners (user role 1) including their schedules
            int minHour = 6;
            int maxHour = 18;
            for (int h=minHour; h<=maxHour; h++)
            {
                Timeslot newTS = new Timeslot();
                newTS.Date = DateTime.Today;
                newTS.Hour = h;
                db.Add(newTS);
                foreach (User p in allPs)
                {
                    foreach (PSchedule ps in p.PSchedules)
                    {
                        if (ps.DayOfWeek == newTS.Date.DayOfWeek.ToString())
                        {
                            // objName.GetType().GetProperty("propName").GetValue(objName); // this is code format for getting a property using a string for the property name
                            bool isPAvailNow = (bool)ps.GetType().GetProperty("t" + h).GetValue(ps); // adds the letter t to the integer of the timeslot's hour and gets that property value from the practitioner schedule to see if they are available
                            if (isPAvailNow)
                            {
                                PAvailTime pat = new PAvailTime();
                                pat.PractitionerId = ps.PractitionerId;
                                pat.TimeslotId = newTS.TimeslotId;
                                db.Add(pat);
                            }
                        }
                    }
                }
            }
            db.SaveChanges();
            return;
        }

        // Generate New Entries
        public static void GenerateTimeslots(int daysToBuild, Timeslot lastTS, ProjectContext db)
        {
            System.Console.WriteLine($"Beginning Timeslot Generation at {DateTime.Now}");
            DateTime startTime = DateTime.Now;
            List<User> allPs = db.Users.Include(u => u.PSchedules).Where(u => u.Role == 1).ToList(); // all practitioners (user role 1) including their schedules
            int minHour = 6;
            int maxHour = 18;
            for (int d=1; d<daysToBuild; d++)
            {
                for (int h=minHour; h<=maxHour; h++)
                {
                    // generate new timeslot for each hour of each day we are adding
                    Timeslot newTS = new Timeslot();
                    if (lastTS == null)
                    {
                        newTS.Date = DateTime.Today.AddDays(d);
                    }
                    else {
                        newTS.Date = lastTS.Date.AddDays(d);
                    }
                    newTS.Hour = h;
                    db.Add(newTS);
                    // generate new PAvailTimes to connect practitioners to each timeslot if their PSchedule lists them as available at this time/day
                    foreach (User p in allPs)
                    {
                        foreach (PSchedule ps in p.PSchedules)
                        {
                            if (ps.DayOfWeek == newTS.Date.DayOfWeek.ToString())
                            {
                                // objName.GetType().GetProperty("propName").GetValue(objName); // this is code format for getting a property using a string for the property name
                                bool isPAvailNow = (bool)ps.GetType().GetProperty("t" + h).GetValue(ps); // adds the letter t to the integer of the timeslot's hour and gets that property value from the practitioner schedule to see if they are available
                                if (isPAvailNow)
                                {
                                    PAvailTime pat = new PAvailTime();
                                    pat.PractitionerId = ps.PractitionerId;
                                    pat.TimeslotId = newTS.TimeslotId;
                                    db.Add(pat);
                                }
                            }
                        }
                    }
                }
            }
            db.SaveChanges();        
            System.Console.WriteLine($"Timeslot Generation completed at {DateTime.Now}");
            System.Console.WriteLine($"Time taken: {(DateTime.Now - startTime).TotalSeconds} seconds");
        }

        public static void GenerateFakeContent(ProjectContext dbContext)
        {
            Insurance newIns = new Insurance();
            newIns.Name = "Cash";
            dbContext.Add(newIns);
            dbContext.SaveChanges();
            Insurance newIns2 = new Insurance();
            newIns2.Name = "Blue Cross/Blue Shield";
            dbContext.Add(newIns2);
            dbContext.SaveChanges();
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
            newCust.Insurance = newIns;
            dbContext.Add(newCust);
            dbContext.SaveChanges();
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
            newCust2.Insurance = newIns2;
            dbContext.Add(newCust2);
            dbContext.SaveChanges();
            User pract1 = new User();
            pract1.Password = "AQAAAAEAACcQAAAAEOfdJHYZmxEQpJaWvaD4c7z4CiXIa5ZPfplYWFuCOPYYbXAUxjGKOM3zhm0plujL5g=="; // password hash for Password1!
            pract1.FirstName = "John";
            pract1.LastName = "Smith";
            pract1.Role = 1;
            pract1.UserName = "JohnSmith";
            dbContext.Add(pract1);
            dbContext.SaveChanges();
            User pract2 = new User();
            pract2.Password = "AQAAAAEAACcQAAAAEOfdJHYZmxEQpJaWvaD4c7z4CiXIa5ZPfplYWFuCOPYYbXAUxjGKOM3zhm0plujL5g=="; // password hash for Password1!
            pract2.FirstName = "Chris";
            pract2.LastName = "Rodger";
            pract2.Role = 1;
            pract2.UserName = "ChrisRodger";
            dbContext.Add(pract2);
            dbContext.SaveChanges();
            User recep1 = new User();
            recep1.Password = "AQAAAAEAACcQAAAAEOfdJHYZmxEQpJaWvaD4c7z4CiXIa5ZPfplYWFuCOPYYbXAUxjGKOM3zhm0plujL5g=="; // password hash for Password1!
            recep1.FirstName = "Jane";
            recep1.LastName = "Doe";
            recep1.Role = 2;
            recep1.UserName = "ChrisRodger";
            dbContext.Add(recep1);
            dbContext.SaveChanges();
            User recep2 = new User();
            recep2.Password = "AQAAAAEAACcQAAAAEOfdJHYZmxEQpJaWvaD4c7z4CiXIa5ZPfplYWFuCOPYYbXAUxjGKOM3zhm0plujL5g=="; // password hash for Password1!
            recep2.FirstName = "Jane";
            recep2.LastName = "Doe";
            recep2.Role = 2;
            recep2.UserName = "ChrisRodger";
            dbContext.Add(recep2);
            dbContext.SaveChanges();
            Service serv1 = new Service();
            serv1.Name = "Deep Tissue Massage";
            dbContext.Add(serv1);
            dbContext.SaveChanges();
            Service serv2 = new Service();
            serv2.Name = "Foot Massage";
            dbContext.Add(serv2);
            dbContext.SaveChanges();
            Service serv3 = new Service();
            serv3.Name = "Accupuncture";
            dbContext.Add(serv3);
            dbContext.SaveChanges();

            //Create Rooms
            Room room1 = new Room();
            dbContext.Add(room1);
            dbContext.SaveChanges();
            Room room2 = new Room();
            dbContext.Add(room2);
            dbContext.SaveChanges();
            Room room3 = new Room();
            dbContext.Add(room3);
            dbContext.SaveChanges();
            Room room4 = new Room();
            dbContext.Add(room4);
            dbContext.SaveChanges();
            Room room5 = new Room();
            dbContext.Add(room5);
            dbContext.SaveChanges();
            Room room6 = new Room();
            dbContext.Add(room6);
            dbContext.SaveChanges();

        //Associate Rooms with Service
            List<Room> allRooms = new List<Room>(){room1, room2, room3, room4, room5, room6};
            List<Service> allServices = new List<Service>(){serv1, serv2, serv3};
            foreach (Room room in allRooms)
            {
                foreach (Service serv in allServices)
                {
                    RoomService rs = new RoomService();
                    rs.RoomId = room.RoomId;
                    rs.ServiceId = serv.ServiceId;
                    dbContext.Add(rs);
                    dbContext.SaveChanges();
                }
            }
            dbContext.SaveChanges();

            //Associate Practitioners with Service
            PService ps = new PService();
            ps.ServiceId = serv1.ServiceId;
            ps.PractitionerId = pract1.UserId;
            dbContext.Add(ps);
            dbContext.SaveChanges();
            PService ps2 = new PService();
            ps2.ServiceId = serv2.ServiceId;
            ps2.PractitionerId = pract1.UserId;
            dbContext.Add(ps2);
            dbContext.SaveChanges();
            PService ps3 = new PService();
            ps3.ServiceId = serv3.ServiceId;
            ps3.PractitionerId = pract1.UserId;
            dbContext.Add(ps3);
            dbContext.SaveChanges();
            PService ps4 = new PService();
            ps4.ServiceId = serv2.ServiceId;
            ps4.PractitionerId = pract2.UserId;
            dbContext.Add(ps4);
            dbContext.SaveChanges();
            PService ps5 = new PService();
            ps5.ServiceId = serv3.ServiceId;
            ps5.PractitionerId = pract2.UserId;
            dbContext.Add(ps5);
            dbContext.SaveChanges();

            //Associate Practitioner with Insurance
            PInsurance pi1 = new PInsurance();
            pi1.PractitionerId = pract1.UserId;
            pi1.InsuranceId = newIns.InsuranceId;
            dbContext.Add(pi1);
            dbContext.SaveChanges();
            PInsurance pi2 = new PInsurance();
            pi2.PractitionerId = pract1.UserId;
            pi2.InsuranceId = newIns2.InsuranceId;
            dbContext.Add(pi2);
            dbContext.SaveChanges();
            PInsurance pi3 = new PInsurance();
            pi3.PractitionerId = pract2.UserId;
            pi3.InsuranceId = newIns.InsuranceId;
            dbContext.Add(pi3);
            dbContext.SaveChanges();

            //Query for One schedule for practitioner1, then same for practitioner2
            Query.OnePsSchedules(pract1.UserId, dbContext);
            Query.OnePsSchedules(pract2.UserId, dbContext);
            //Check last timeslot, generate timeslot based off of last timeslot
            Generate.CheckTimeslots(dbContext);
            Timeslot tsToday = dbContext.Timeslots.OrderByDescending(t => t.Hour).FirstOrDefault(t => t.Date == DateTime.Today);
            
            //Create new reservation
            //Associaton: CreaterId, CustomerId, PractitionerId, RoomId, ServiceId, TimeslotId
            Reservation newResToday = new Reservation();
            newResToday.CreatorId = recep1.UserId;
            newResToday.CustomerId = newCust.CustomerId;
            newResToday.Notes = "Important VIP Reservation!!!!!!";
            newResToday.PractitionerId = pract1.UserId;
            newResToday.RoomId = room1.RoomId;
            newResToday.ServiceId = serv1.ServiceId;
            newResToday.TimeslotId = tsToday.TimeslotId;
            dbContext.Add(newResToday);
            dbContext.SaveChanges();

            Reservation newResToday2 = new Reservation();
            newResToday2.CreatorId = recep2.UserId;
            newResToday2.CustomerId = newCust2.CustomerId;
            newResToday2.Notes = "Will they show up?";
            newResToday2.PractitionerId = pract2.UserId;
            newResToday2.RoomId = room2.RoomId;
            newResToday2.ServiceId = serv3.ServiceId;
            newResToday2.TimeslotId = tsToday.TimeslotId;
            dbContext.Add(newResToday2);
            dbContext.SaveChanges();

            //Create another Reservation based of off the tstomorrow timeslot
            Timeslot tsTomorrow = dbContext.Timeslots.OrderByDescending(t => t.Hour).FirstOrDefault(t => t.Date == DateTime.Today.AddDays(1));
            Reservation newResTomorrow = new Reservation();
            newResTomorrow.CreatorId = recep1.UserId;
            newResTomorrow.CustomerId = newCust.CustomerId;
            newResTomorrow.Notes = "Important VIP Reservation!!!!!!";
            newResTomorrow.PractitionerId = pract1.UserId;
            newResTomorrow.RoomId = room1.RoomId;
            newResTomorrow.ServiceId = serv1.ServiceId;
            newResTomorrow.TimeslotId = tsTomorrow.TimeslotId;
            dbContext.Add(newResTomorrow);
            dbContext.SaveChanges();

            Reservation newResTomorrow2 = new Reservation();
            newResTomorrow2.CreatorId = recep2.UserId;
            newResTomorrow2.CustomerId = newCust2.CustomerId;
            newResTomorrow2.Notes = "Will they show up?";
            newResTomorrow2.PractitionerId = pract2.UserId;
            newResTomorrow2.RoomId = room2.RoomId;
            newResTomorrow2.ServiceId = serv3.ServiceId;
            newResTomorrow2.TimeslotId = tsTomorrow.TimeslotId;
            dbContext.Add(newResTomorrow2);
            dbContext.SaveChanges();
            return;
        }
    }
}