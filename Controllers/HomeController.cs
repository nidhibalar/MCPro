using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCPro.Models;
using MCPro.DB.DbOperations;
using System.Web.Security;

namespace MCPro.Controllers
{
    public class HomeController : Controller
    {
        UsersRepo urep = null;
        public HomeController()
        {
            urep = new UsersRepo();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //register
        public ActionResult Register()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                int id = urep.GetRegister(model);
                if (id > 0)
                {
                    ModelState.Clear();
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        //login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                int chk = urep.Login(model);

                if (chk == 1) // if normal user
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("Index", "User");
                }
                else if (chk == 2) // if admin user
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Username/Password!");
                }
            }
            return View();
        }
    }
}