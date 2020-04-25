using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using massage.Models;
using Microsoft.AspNetCore.Http;

namespace massage.Controllers
{
    [Route("rec")]
    public class ReceptionistController : Controller
    {
        // Database setup
        public ProjectContext dbContext;
        public ReceptionistController(ProjectContext context)
        {
            dbContext = context;
        }

        // User session to keep track who is logged in!!
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

        // ROUTES

        // REC DASHBOARD
        // Access from Receptionst: "dashboard"
        // Access from anywhere else: "/rec/dashboard"
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            // Checks User's role and login
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            Generate.CheckTimeslots(dbContext); // check if timeslots need to be generated
            ViewModel vm = new ViewModel();
            vm.CurrentUser = UserSession;
            vm.AllUsers = Query.AllUsers(dbContext);
            vm.AllCustomers = Query.AllCustomers(dbContext);
            vm.AllInsurances = Query.AllInsurances(dbContext);
            vm.AllServices = Query.AllServices(dbContext);
            vm.AllTimeslots = Query.AllTimeslots(dbContext);
            vm.AllPractitioners = Query.AllPractitioners(dbContext);
            vm.AllReservations = Query.AllReservations(dbContext);
            return PartialView("RDashboard",vm);
        }
        [HttpGet("insurances")]
        public IActionResult Insurances() {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            ViewModel vm = new ViewModel();
            vm.CurrentUser = UserSession;
            vm.AllInsurances = Query.AllInsurances(dbContext);
            vm.AllUsers = Query.AllUsers(dbContext);
            return PartialView(vm);
        }

        [HttpGet("CurrentReservation/{id}")]
        public IActionResult CurrentReservation(int id)
        {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            ViewModel vm = new ViewModel();
            vm.CurrentUser = Query.OneReceptionist(UserSession.UserId, dbContext);
            vm.AllUsers = Query.AllUsers(dbContext);
            vm.AllCustomers = Query.AllCustomers(dbContext);
            vm.AllInsurances = Query.AllInsurances(dbContext);
            vm.AllServices = Query.AllServices(dbContext);
            vm.AllTimeslots = Query.AllTimeslots(dbContext);
            vm.AllPractitioners = Query.AllPractitioners(dbContext);
            vm.AllReservations = Query.AllReservations(dbContext);
            vm.OneReservation = Query.OneReservation(id, dbContext);
            return PartialView(vm);
        }

        // ALL RESERVATIONS
        [HttpGet("all_res")]
        public IActionResult AllReservations()
        {
            // Checks User's role and login
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            Query.AllReservations(dbContext);
            return PartialView();
        }

        // FORM PAGE??
        [HttpGet("NewReservation")]
        public IActionResult NewReservation()
        {
            // Checks User's role and login
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            Generate.CheckTimeslots(dbContext); // check if timeslots need to be generated
            ViewModel vm = new ViewModel();
            vm.CurrentUser = Query.OneReceptionist(UserSession.UserId, dbContext);
            vm.AllUsers = Query.AllUsers(dbContext);
            vm.AllCustomers = Query.AllCustomers(dbContext);
            vm.AllInsurances = Query.AllInsurances(dbContext);
            vm.AllServices = Query.AllServices(dbContext);
            vm.AllTimeslots = Query.AllTimeslots(dbContext);
            vm.AllPractitioners = Query.AllPractitioners(dbContext);
            vm.OneReservation = new Reservation();
            vm.OneReservation.CreatorId = vm.CurrentUser.UserId;
            return PartialView(vm);
        }
        [HttpGet("newinsurance")]
        public IActionResult NewInsurance()
        {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            return View();
        }
        [HttpPost("CreateInsurance")]
        public IActionResult CreateInsurance(Insurance newInsurance)
        {
            User currentUser = dbContext.Users.Where(u => u.UserId == HttpContext.Session.GetInt32("UserId")).SingleOrDefault();
            if (ModelState.IsValid)
            {
                dbContext.Add(newInsurance);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return PartialView("NewInsurance");
            }
        }
        [HttpGet("Customers")]
        public IActionResult Customers() {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            ViewModel vm = new ViewModel();
            vm.CurrentUser = UserSession;
            vm.AllCustomers = Query.AllCustomers(dbContext);
            vm.AllUsers = Query.AllUsers(dbContext);
            return PartialView(vm);
        }

        // Create Reservation from timeslot id
        [HttpPost("CreateReservation")]
        public IActionResult CreateReservation(ViewModel vm)
        {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            if (vm.OneReservation.CreatorId == 0 || vm.OneReservation.CustomerId == 0 || vm.OneReservation.PractitionerId == 0 || vm.OneReservation.ServiceId == 0 || vm.OneReservation.TimeslotId == 0)
            {
                ViewBag.errormsg = "You must select all options!";
                vm.CurrentUser = Query.OneReceptionist(UserSession.UserId, dbContext);
                vm.AllUsers = Query.AllUsers(dbContext);
                vm.AllCustomers = Query.AllCustomers(dbContext);
                vm.AllInsurances = Query.AllInsurances(dbContext);
                vm.AllServices = Query.AllServices(dbContext);
                vm.AllTimeslots = Query.AllTimeslots(dbContext);
                vm.AllPractitioners = Query.AllPractitioners(dbContext);
                vm.OneReservation.Practitioner = Query.OnePractitioner(vm.OneReservation.PractitionerId, dbContext);
                return PartialView("NewReservation", vm);
            }
            Timeslot thisTS = Query.OneTimeslot(vm.OneReservation.TimeslotId, dbContext);
            vm.OneTimeslot = thisTS;
            vm.AllPractitioners = Query.AllPractitioners(dbContext);
            vm.OneCustomer = Query.OneCustomer(vm.OneReservation.CustomerId, dbContext);
            vm.OneService = Query.OneService(vm.OneReservation.ServiceId, dbContext);
            vm.OneInsurance = Query.OneInsurance(vm.OneCustomer.InsuranceId, dbContext);
            vm.OneReservation.Practitioner = Query.OnePractitioner(vm.OneReservation.PractitionerId, dbContext);
            vm.CurrentUser = UserSession;
            return PartialView("ReservationForm", vm);
        }
        [HttpPost("SubmitReservation")]
        public IActionResult SubmitReservation(ViewModel vm)
        {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            vm.OneReservation.CreatorId = UserSession.UserId;
            int roomID = 1;
            Timeslot thisTS = Query.OneTimeslot(vm.OneReservation.TimeslotId, dbContext);
            List<int> roomIDList = new List<int>();
            foreach (Reservation resv in thisTS.Reservations)
            {
                roomIDList.Add(resv.RoomId);
            }
            while (roomIDList.IndexOf(roomID) != -1)
            {
                roomID ++;
            }
            vm.OneReservation.RoomId = roomID;
            if (ModelState.IsValid)
            {
                
                // manual validations
                if (vm.OneReservation.CreatorId == 0 || vm.OneReservation.CustomerId == 0 || vm.OneReservation.PractitionerId == 0 || vm.OneReservation.RoomId == 0 || vm.OneReservation.ServiceId == 0 || vm.OneReservation.TimeslotId == 0)
                {
                    ViewBag.errormsg = "An association value was 0, an error has occured";
                    vm.CurrentUser = UserSession;
                    return RedirectToAction("NewReservation");
                }
                Query.CreateReservation(vm.OneReservation, dbContext);
                return RedirectToAction("Dashboard");
            }
            else {
                vm.CurrentUser = UserSession;
                return RedirectToAction("NewReservation");
            }
        }

        // SUBMIT: cancel res
        [HttpPost("CancelReservation/{id}")]
        public IActionResult CancelReservation(int id)
        {
            Reservation newReservation = Query.OneReservation(id, dbContext);
            Query.DeleteReservation(newReservation.ReservationId, dbContext);
            return RedirectToAction("Dashboard", "Receptionist");
        }

        // VIEW ALL CUSTOMER
        [HttpGet]
        public IActionResult AllCustomers()
        {
            // Checks User's role and login
            // Checks User's role and login
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            return View();
        }

        // NEW CUSTOMER FORM?
        [HttpGet("NewCustomer")]
        public IActionResult NewCustomer()
        {
            // Checks User's role and login
            // Checks User's role and login
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            ViewModel vm = new ViewModel();
            vm.CurrentUser = UserSession;
            vm.AllUsers = Query.AllUsers(dbContext);
            vm.AllCustomers = Query.AllCustomers(dbContext);
            vm.AllInsurances = Query.AllInsurances(dbContext);
            vm.AllServices = Query.AllServices(dbContext);
            vm.AllTimeslots = Query.AllTimeslots(dbContext);
            return PartialView(vm);
        }

        // SUBMIT: create new customer
        [HttpPost("CreateCustomer")]
        public IActionResult CreateCustomer(ViewModel vm)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(vm.OneCustomer);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                vm.CurrentUser = UserSession;
                vm.AllUsers = Query.AllUsers(dbContext);
                vm.AllCustomers = Query.AllCustomers(dbContext);
                vm.AllInsurances = Query.AllInsurances(dbContext);
                vm.AllServices = Query.AllServices(dbContext);
                vm.AllTimeslots = Query.AllTimeslots(dbContext);
                return PartialView("NewCustomer", vm);
            }
        }
        [HttpGet("customers/{id}/delete")]
        public RedirectToActionResult DeleteCustomer(int id) {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            dbContext.Remove(Query.OneCustomer(id, dbContext));
            dbContext.SaveChanges();
            return RedirectToAction("customers");
        }
        [HttpGet("insurances/{id}/delete")]
        public RedirectToActionResult DeleteInsurance(int id) {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            dbContext.Remove(Query.OneInsurance(id, dbContext));
            dbContext.SaveChanges();
            return RedirectToAction("insurances");
        }

        [HttpGet("PractitionerShow/{id}")]
        public IActionResult PractitionerShow(int userId)
        {
            // Checks User's role and login
            return RedirectToAction("Dashboard");;
        }
        // MY PROFILE
        [HttpGet]
        public IActionResult MyProfile()
        {
            // Checks User's role and login
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            return View();
        }

        // FORM: edit profile
        [HttpGet]
        public IActionResult EditProfile()
        {
            // Checks User's role and login
            AccessCheck();
            return View();
        }

        [HttpPost]
        public IActionResult UpdatedProfile(User receptionist)
        {
            if (ModelState.IsValid)
            {
                dbContext.Add(receptionist);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard", "Receptionist");
            }
            else
            {
                return PartialView("EditProfile");
            }
        }

        [HttpGet]
        public IActionResult OneDayAvailability(DateTime oneDay)
        {
            // Checks User's role and login
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            ViewModel vm = new ViewModel();
            vm.AllTimeslots = Query.OneDaysTimeslots(oneDay, dbContext);
            return PartialView("DayViewTimeslots", vm);
        }
        
        [HttpGet]
        public IActionResult TodaysAvailability()
        {
            // Checks User's role and login
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            ViewModel vm = new ViewModel();
            vm.AllTimeslots = Query.TodaysTimeslots(dbContext);
            return PartialView("DayViewTimeslots", vm);
        }

        [HttpGet]
        public IActionResult ThisWeeksAvailability()
        {
            // Checks User's role and login
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            ViewModel vm = new ViewModel();
            vm.AllTimeslots = Query.ThisWeeksTimeslots(dbContext);
            return PartialView("WeekViewTimeslots", vm);
        }
        [HttpGet]
        public IActionResult ThisMonthsAvailability()
        {
              // Checks User's role and login
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            ViewModel vm = new ViewModel();
            vm.AllTimeslots = Query.ThisMonthsTimeslots(dbContext);
            return PartialView("MonthViewTimeslots", vm);
        }
        
        [HttpGet]
        public IActionResult OneWeeksAvailability(DateTime startDay)
        {
            // Checks User's role and login
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            ViewModel vm = new ViewModel();
            vm.AllTimeslots = Query.OneWeeksTimeslots(startDay, dbContext);
            return PartialView("WeekViewTimeslots", vm);
        }

        [HttpGet]
        public IActionResult OneMonthsAvailability(DateTime dayInMonth)
        {
            // Checks User's role and login
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            ViewModel vm = new ViewModel();
            vm.AllTimeslots = Query.OneMonthsTimeslots(dayInMonth, dbContext);
            return PartialView("MonthViewTimeslots", vm);
        }
    }
}
