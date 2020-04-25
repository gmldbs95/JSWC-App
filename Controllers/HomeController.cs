using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using massage.Models;
using Newtonsoft.Json;

namespace massage.Controllers
{
    public class HomeController : Controller
    {
        // database setup
        public ProjectContext dbContext;
        private User UserSession {
            get {return dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));}
            set {HttpContext.Session.SetInt32("UserId", value.UserId);}
        }
        // Redirects non-practitioner users to their respective dashboards
        // see PractitionerController.AccessCheck for details
        private string[] AccessCheck() {
            User ActiveUser = UserSession;
            if(ActiveUser == null) return new string[]{"Login", "Login"};
            else if (ActiveUser.Role == 0) return new string[]{"Dashboard", "Home"};
            else if (ActiveUser.Role == 1) return new string[]{"Dashboard", "Practitioner"};
            return null;
        }
        public HomeController(ProjectContext context)
        {
            dbContext = context;
        }

        // all reservations
        [HttpGet("allreservations")]
        public IActionResult GetAllReservations()
        {
            List<Reservation> allRs = dbContext.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Practitioner)
                .Include(r => r.Creator)
                .Include(r => r.Service)
                .Include(r => r.Room)
                .Include(r => r.Timeslot)
                .ToList();
            ViewModel vm = new ViewModel();
            vm.AllReservations = allRs;
            return View("AllReservations", vm);
            
        }
        [HttpGet("generatefakecontent")] // debug path to make fake content for testing purposes
        public IActionResult GenerateFakeContent()
        {
            Generate.GenerateFakeContent(dbContext);
            return RedirectToAction("Dashboard");
        }

        // Create New Entries
        public IActionResult NewService(Service newsvc)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(newsvc);
                dbContext.SaveChanges();
                List<Room> allRooms = dbContext.Rooms.ToList();
                foreach (Room r in allRooms)
                {
                    RoomService rs = new RoomService();
                    rs.RoomId = r.RoomId;
                    rs.ServiceId = newsvc.ServiceId;
                    dbContext.Add(rs);
                }
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public IActionResult NewRoom(Room r)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(r);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult NewCustomer(Customer c)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(c);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult NewPSchedule(PSchedule ps)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(ps);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult NewInsurance(Insurance i)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(i);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public IActionResult NewReservation(Reservation r)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(r);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            return PartialView();
        }

        [HttpGet("calendarJson")]
        public string calendarJson()
        {
            List<Timeslot> allTimeslots = Query.AllTimeslots(dbContext);
            string jsonEvents = QConvert.TimeslotsToEvents(allTimeslots);
            return jsonEvents;
        }
        [HttpGet("calendarReservationsJson")]
        public string calendarReservationsJson()
        {
            List<Reservation> allReservations = Query.AllReservations(dbContext);
            string jsonEvents = QConvert.ReservationsToEvents(allReservations);
            return jsonEvents;
        }
        [HttpPost("calendarFilterJson")]
        [RequestSizeLimit(2147483648)]
        public string calendarFilterJson(string body)
        {
            return QConvert.FilteredEvents(body, dbContext);
        }

        [HttpGet("/userProfile")]
        public IActionResult userProfile()
        {
            ViewModel vm = new ViewModel();
            // vm.CurrentUser = dbContext.Users.Include(u => u.PSchedules).Include(u => u.AvailTimes).Include(u => u.Appointments).FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("Id"));
            return PartialView("UserProfile", vm);
        }

           public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult clearusers() {
            foreach (object u in dbContext.Users) {
                dbContext.Remove(u);
            }
            dbContext.SaveChanges();
            return Redirect("login");
        }
    }
}
