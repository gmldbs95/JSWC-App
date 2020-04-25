using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using massage.Models;
using Microsoft.AspNetCore.Identity;

namespace massage.Controllers
{
    public class LoginController : Controller
    {
        // database setup
        public ProjectContext dbContext;
        public LoginController(ProjectContext context)
        {
            dbContext = context;
        }

        // User session to keep track who is logged in!!
        private User UserSession {
            get {return dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));}
            set {HttpContext.Session.SetInt32("UserId", value.UserId);}
        } 

        // routes
        [HttpGet("login")]
        [HttpGet("")]
        public IActionResult Login(){
            return View();
        }
        [HttpGet("register")]
        public IActionResult Register(){
            return View();
        }
        [HttpPost("submitregister")]
        public IActionResult SubmitRegister(User newUser) {
            if (ModelState.IsValid) 
            { // pass validations
                if (dbContext.Users.Any(u => u.UserName == newUser.UserName)){ //user in db already
                    ModelState.AddModelError("UserName", "Username already in use!");
                    return View("Register");
                }
                else
                {//user not in db, can register
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                    dbContext.Add(newUser);
                    dbContext.SaveChanges();
                    User thisUser = dbContext.Users.FirstOrDefault(u => u.UserName == newUser.UserName);
                    HttpContext.Session.SetInt32("UserId", thisUser.UserId);
                    if(thisUser.Role == 5) return RedirectToAction("Dashboard", "Admin");
                    else if (thisUser.Role == 1) return RedirectToAction("Dashboard", "Practitioner");
                    else if (thisUser.Role == 0) return RedirectToAction("Dashboard", "Home");
                    else if (thisUser.Role == 2) return RedirectToAction("Dashboard", "Receptionist");
                }
            } 
            //failed validations
            return View("Register");
        }
        [HttpPost("submitlogin")]
        public IActionResult SubmitLogin(LoginUser loginUser)
        {
            if (ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.UserName == loginUser.UserName);
                if (userInDb == null)
                { // Username not found in db
                    ModelState.AddModelError("Username", "Invalid Username/Password");
                    return View("Login");
                }
                PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
                var result = Hasher.VerifyHashedPassword(loginUser, userInDb.Password, loginUser.Password);
                if (result == 0)
                { // password doesn't match
                    ModelState.AddModelError("Username", "Invalid Username/Password");
                    return View("Login");
                }
                else
                { // success
                    UserSession = userInDb;
                    if(userInDb.Role == 5) return RedirectToAction("Dashboard", "Admin");
                    else if (userInDb.Role == 1) return RedirectToAction("Dashboard", "Practitioner");
                    else if (userInDb.Role == 0) return RedirectToAction("Dashboard", "Home");
                    else if (userInDb.Role == 2) return RedirectToAction("Dashboard", "Receptionist");
                }
            }
            //failed validations
            return View("Login");
        }
        [HttpGet("logout")]
        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Login");
        }
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
