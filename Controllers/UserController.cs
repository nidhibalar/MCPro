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
    [Authorize(Roles ="User")]
    public class UserController : Controller
    {
        // GET: User
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

        //Browse Songs
        public ActionResult BrowseSongs()
        {
            return View();
        }

        //SongList
        public JsonResult GetSongs()
        {
            using (var context = new MusicDBEntities())
            {
                var data = context.Songs.ToList();

                var json = JsonConvert.SerializeObject(data);

                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        //particular song
        public ActionResult Song(int id)
        {
            SongsRepo sr = new SongsRepo();
            return View(sr.GetSong(id));
        }

        //Playlist Info
        public JsonResult GetPlaylists()
        {
            using (var context = new MusicDBEntities())
            {
                //get current user email
                var username = HttpContext.User.Identity.Name;
                //get id
                Users u = context.Users.Where(x => x.Email == username).FirstOrDefault();

                ModelFactory mf = new ModelFactory();

                //get playlists
                var p = context.Playlists.Where(x => x.Uid == u.Id).ToList().Select(x=>mf.Create(x));

                
                /*
                var json = JsonConvert.SerializeObject(pList, new JsonSerializerSettings()
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    Formatting = Formatting.None
                });
                */
                
                var json = JsonConvert.SerializeObject(p);
                
                return Json(p, JsonRequestBehavior.AllowGet);
            }
        }

        //Add To Playlist
        [HttpPost]
        public void AddPL(int pid,int sid)
        {
            using (var context = new MusicDBEntities())
            {
                var result = context.Playlists.SingleOrDefault(x=>x.Id == pid);
                    if (result != null)
                    {
                        result.Sid_arr += " "+sid;

                        //remove white spaces from both side
                        result.Sid_arr = result.Sid_arr.Trim();

                        //result.Sid_arr = "Some new value";
                        context.SaveChanges();
                    }
            }
        }

        //Remove From Playlist
        [HttpPost]
        public void RemovePL(int pid, int sid)
        {
            using (var context = new MusicDBEntities())
            {
                var result = context.Playlists.SingleOrDefault(x => x.Id == pid);
                if (result != null)
                {
                    string or_arr = result.Sid_arr;
                    var t = or_arr.Replace(sid.ToString(), string.Empty);

                    result.Sid_arr = t.Trim();
                    //result.Sid_arr = "Some new value";
                    context.SaveChanges();
                }
            }
        }

        //Update Password
        [HttpPost]
        public void UpdatePassword(string id, string pwd)
        {
            using (var context = new MusicDBEntities())
            {
                var u = context.Users.Where(x => x.Email == id).FirstOrDefault();
                u.Pwd = pwd;
                context.SaveChanges();
            }
        }

        //Delete from Playlist
        public ActionResult DeletePL(int id)
        {
            using (var context = new MusicDBEntities())
            {
                var result = context.Playlists.SingleOrDefault(x => x.Id == id);
                if (result != null)
                {
                    context.Playlists.Remove(result);
                    context.SaveChanges();
                }
                return RedirectToAction("MyPlaylists", "User");
            }
        }

        //My PlayLists
        public ActionResult MyPlaylists()
        {
            return View();
        }

        //particular playlist
        [AllowAnonymous]
        public ActionResult PlaylistInfo(int id)
        {
            PlaylistsRepo pr = new PlaylistsRepo();
            return View(pr.GetPlayList(id));
        }

        //Create new playlist
        [HttpPost]
        public void CreateNewPL(string p_name)
        {
            using (var context = new MusicDBEntities())
            {
                //get current user email
                var username = HttpContext.User.Identity.Name;
                //get id
                Users u = context.Users.Where(x => x.Email == username).FirstOrDefault();

                //Current date and time
                DateTime dt = DateTime.Now;
                //string sqlFormattedDate = dt.ToString("yyyy-MM-dd");

                Playlists pl = new Playlists()
                {
                    Uid = u.Id,
                    P_name=p_name,
                    Date= dt,
                    Sid_arr= ""
                };

                context.Playlists.Add(pl);
                
                context.SaveChanges();
            }
        }

        //Profile
        public ActionResult MyProfile()
        {
            UsersRepo ur = new UsersRepo();
            return View(ur.GetInfo(HttpContext.User.Identity.Name));
        }
    }
}