using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCProj.Models;
using MCPro.DB.DbOperations;
using System.Web.Security;
using MCPro.DB;
using Newtonsoft.Json;

namespace MCPro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        SongsRepo srep = null;
        UsersRepo urep = null;
        public AdminController()
        {
            srep = new SongsRepo();
            urep = new UsersRepo();
        }
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        //Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //Add New Song
        public ActionResult AddSong()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddSong(SongsModel model)
        {
            if(ModelState.IsValid)
            {
                int id = srep.AddSong(model);
                if(id>0)
                {
                    ModelState.Clear();
                    ViewBag.IsSuccess = "Song Added.";
                }
            }
            return View();
        }

        //Manage Songs
        public ActionResult ManageSongs()
        {
            return View();
        }

        //List of songs JSON
        public JsonResult GetSongs()
        {
            using (var context = new MusicDBEntities())
            {
                var data = context.Songs.ToList();

                var json = JsonConvert.SerializeObject(data);

                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        //Edit Songs Details
        public ActionResult EditSong(int id, SongsModel model)
        {
            if (ModelState.IsValid)
            {
                int idd = srep.EditSong(model);
                if (idd > 0)
                {
                    ViewBag.IsSuccess = "Song details updated!";
                }
            }
            SongsRepo sr = new SongsRepo();
            return View(sr.GetSong(id));
        }

        //Delete Song
        public ActionResult DeleteSong(int id)
        {
            srep.DeleteSong(id);
            return RedirectToAction("ManageSongs");
        }

        //Manage Users
        public ActionResult ManageUsers()
        {
            return View();
        }

        //List of Users
        public JsonResult GetUsers()
        {
            using (var context = new MusicDBEntities())
            {
                ModelFactory mf = new ModelFactory();

                //get users
                var p = context.Users.ToList().Select(x => mf.UList(x));

                var json = JsonConvert.SerializeObject(p);

                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        //Edit User Details
        public ActionResult EditUser(int id, UsersModel model)
        {
            if (ModelState.IsValid)
            {
                int idd = urep.EditUser(model);
                if (idd > 0)
                {
                    ViewBag.IsSuccess = "User deatails updated!";
                }
            }
            
            UsersRepo ur = new UsersRepo();
            return View(ur.GetUserInfo(id));
        }

        //Delete User

    }
}