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
    [Route("admin")]
    public class AdminController : Controller
    {
        // database setup
        public ProjectContext dbContext;
        public AdminController(ProjectContext context)
        {
            dbContext = context;

        }
        // User session to keep track who is logged in!!
        private User UserSession {
            get {return dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));}
            set {HttpContext.Session.SetInt32("UserId", value.UserId);}
        }
        private string[] AccessCheck() {
            User ActiveUser = UserSession;
            if(ActiveUser == null) return new string[]{"Login", "Login"};
            else if (ActiveUser.Role == 0) return new string[]{"Dashboard", "Home"};
            else if (ActiveUser.Role == 2) return new string[]{"Dashboard", "Receptionist"};
            else if (ActiveUser.Role == 1) return new string[]{"Dashboard", "Practitioner"};
            return null;
        }

//////////////////////////////// GET ////////////////////////////////
        // Testing dashboard route changed to allow creation of admin dashboard
        [HttpGet("testingdashboard")]
        public IActionResult TestingDashboard() {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            ViewModel vm = new ViewModel();
            vm.CurrentUser = UserSession;
            vm.AllUsers = dbContext.Users.ToList();
            vm.AllCustomers = dbContext.Customers.ToList();
            vm.AllInsurances = dbContext.Insurances.ToList();
            vm.AllServices = dbContext.Services.ToList();
            vm.AllTimeslots = dbContext.Timeslots.ToList();
            vm.AllPractitioners = Query.AllPractitioners(dbContext);
            return PartialView("Dashboard", vm);
        }
        [HttpGet("dashboard")]
        public IActionResult Dashboard() {
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
            return PartialView("ADashboard", vm);
        }

        [HttpGet("AddTestUsers")]
        public IActionResult AddTestUsers()
        {
            Testing.CreateUser(dbContext);
            return RedirectToAction("Dashboard");
        }


        [HttpGet]
        public IActionResult AddTestServices()
        {
            Testing.CreateServices(dbContext);
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public IActionResult AddTestCustomers()
        {
            Testing.CreateCustomers(dbContext);
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public IActionResult AddTestPSchedule()
        {
            Testing.CreatePSchedule(dbContext, 2);
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public IActionResult AddTestTimeslots()
        {
            Testing.CreateTimeslots(dbContext);
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public IActionResult AddTestInsurances()
        {
            Testing.CreateInsurances(dbContext);
            return RedirectToAction("Dashboard");
        }



        // SERVICE
        // Admin: New Service FORM
        [HttpGet("new/service")]
        public IActionResult NewService()
        {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            return PartialView();
        }

        [HttpPost("CreateService")]
        public IActionResult CreateService(Service newService)
        {
            User currentUser = dbContext.Users.Where(u => u.UserId == HttpContext.Session.GetInt32("UserId")).SingleOrDefault();
            if (ModelState.IsValid)
            {
                dbContext.Add(newService);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return PartialView("newService");
            }
        }

        // INSURANCE
        // Admin: Add new insurance FORM
        [HttpGet("newinsurance")]
        public IActionResult NewInsurance()
        {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            return PartialView();
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


        // CUSTOMER
        // Admin: New Customer FORM
        [HttpGet("newcustomer")]
        public IActionResult NewCustomer()
        {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            ViewModel vm = new ViewModel();
            vm.AllInsurances = dbContext.Insurances.ToList();
            vm.CurrentUser = UserSession;
            vm.AllUsers = dbContext.Users.ToList();
            return PartialView(vm);
        }

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



        // ADMIN: Add Roles
        [HttpGet("all_newusers")]
        public IActionResult AllNewUsers(){
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            ViewModel vm = new ViewModel();
            vm.AllUsers = dbContext.Users.Where(n => n.Role == 0).ToList();
            vm.CurrentUser = UserSession;
            return PartialView(vm);
        }
        [HttpGet("setrole_prac/{id}")]
        public IActionResult SetRoleP(int id){
            User thisUser = dbContext.Users.FirstOrDefault(u => u.UserId == id);
            thisUser.Role = 1;
            dbContext.SaveChanges();
            return RedirectToAction("AllNewUsers");
        }
        [HttpGet("setrole_rec/{id}")]
        public IActionResult SetRoleR(int id){
            User thisUser = dbContext.Users.FirstOrDefault(u => u.UserId == id);
            thisUser.Role = 2;
            dbContext.SaveChanges();
            return RedirectToAction("AllNewUsers");
        }

//////////////////////////////// POST ////////////////////////////////
        
        // Admin: Employee Profile FORM-SUBMIT
        public IActionResult UserProfileSubmit(){
            if(ModelState.IsValid){
                // stuff
                return RedirectToAction("UserProfile");
            }
            else {
                return PartialView("UserProfile");

            }
        }

        // Admin: Employee template FORM-SUBMIT
        public IActionResult UserTemplateSubmit(){
            if(ModelState.IsValid){
                // stuff
                return RedirectToAction("UserProfile");
            }
            else {
                return PartialView("UserProfile");
            }
        }

        // Admin: Add new insurance FORM-SUBMIT
        public IActionResult CreateInsuranceSubmit(){
            if(ModelState.IsValid){
                // stuff
                return RedirectToAction("AllInsurance");
            }
            else {
                return PartialView("UserProfile");
            }
        }

        // Admin: Updating insurance FORM-SUBMIT
        public IActionResult UpdateInsuranceSubmit(){
            if(ModelState.IsValid){
                // stuff
                return RedirectToAction("AllInsurance");
            }
            else {
                return PartialView("UserProfile");
            }
        }

    }   // END CONTROLLER
}   // END ALL