using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using massage.Models;


namespace massage.Controllers
{
    [Route("prac")]
    public class PractitionerController : Controller
    {
        public ProjectContext dbContext;
        public PractitionerController(ProjectContext context)
        {
            dbContext = context;
        }
        // Logged in user
        // e.g. return redirect($"users/{UserSession.UserId}")
        private User UserSession {
            get {return dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));}
            set {HttpContext.Session.SetInt32("UserId", value.UserId);}
        }
        // Redirects all non-practitioner users to their respective dashboards
        // Call at the beginning of all get methods with access restricted to practitioners
        // e.g. public IActionResult Dashboard() {AccessCheck(); ...rest of action logic}
        private string[] AccessCheck() {
            User ActiveUser = UserSession;
            if(ActiveUser == null) return new string[]{"Login", "Login"};
            else if (ActiveUser.Role == 0) return new string[]{"Dashboard", "Home"};
            else if (ActiveUser.Role == 2) return new string[]{"Dashboard", "Receptionist"};
            return null;
        }

//////////////////////////////// GET ////////////////////////////////
        
        // Practitioner dashboard
        [HttpGet("dashboard")]
        public IActionResult Dashboard () {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            Generate.CheckTimeslots(dbContext); // check if timeslots need to be generated
            ViewModel vm = new ViewModel();
            User currUser = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            vm.AllReservations = Query.OnePTodaysReservations(currUser.UserId, dbContext);
            vm.CurrentUser = currUser;
            return PartialView("PDashboard", vm);
        }

        // Practitioner Schedule View w/ current
        [HttpGet("schedule")]
        public IActionResult PracSched(){
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            // stuff
            return PartialView();
        }

        // Practitioner Template View FORM
        [HttpGet("template")]
        public IActionResult PracTemplate(){
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            // stuff
            return PartialView();
        }

        // Practitioner Profile View
        [HttpGet("profile")]
        public IActionResult PracProfile(){
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            // stuff
            return PartialView();
        }

        // Practitioner Update Profile FORM
        [HttpGet("update_profile")]
        public IActionResult UpdatePracProf(){
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            // stuff
            return PartialView();
        }
        [HttpGet]
        [Route("{PracId}")]
        public IActionResult PractitionerProfile(int PracId)
        {
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            ViewModel vm = new ViewModel();
            vm.CurrentUser = UserSession;
            vm.OneUser = Query.OnePractitioner(PracId, dbContext);
            vm.PSDict = QConvert.ScheduleFromQuery(Query.OnePsSchedules(UserSession.UserId, dbContext));
            // vm.AllPSchedules = Query.OnePsSchedules(UserSession.UserId, dbContext);
            return PartialView(vm);
        }

//////////////////////////////// POST ////////////////////////////////

        // Practitioner: template FORM ON-SUBMIT
        [HttpPost]
        public IActionResult PracTemplateSubmit(){
            if(ModelState.IsValid){
                // stuff
                return RedirectToAction("Dash");
            }
            else {
                return View("UserProfile");
            }
        }

        [HttpPost("togglepracsched/{id}")]
        public IActionResult togglePracSched(ViewModel result, int id){
            string[] check = AccessCheck();
            if(check != null) return RedirectToAction(check[0], check[1]);
            Generate.CheckTimeslots(dbContext); // check if timeslots need to be generated
            System.Console.WriteLine($"####################################################{ result.PSDict.Keys}");
            List<PSchedule> Schedules = QConvert.ScheduleToQuery(result.PSDict, id);
            Query.UpdateAllOfOnePsSchedules(id, Schedules, dbContext);
            return RedirectToAction("PractitionerProfile", new {PracId = id});
        }

        // Practitioner: profile FORM ON-SUBMIT
        [HttpPost]
        public IActionResult PracProfileSubmit(){
            if(ModelState.IsValid){
                // stuff
                return RedirectToAction("PracProfile");
            }
            else {
                return View("UserProfile");
            }
        }

                [HttpPost]
        public IActionResult togglePracSched()
        {
            return RedirectToAction("PractitionerProfile");
        }
    }   // END CONTROLLER
}   // END ALL