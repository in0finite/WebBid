using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1;

namespace WebApplication1.Controllers
{
    public class UsersController : Controller
    {
        private Model1 db = new Model1();

        // GET: Users
        //public ActionResult Index()
        //{
        //    return View(db.Users.ToList());
        //}

        //// GET: Users/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    User user = db.Users.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        // GET: Users/Create
        public ActionResult Create()
        {
            if (Auth.Check())
                return RedirectToAction("Index", "Auctions");

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Surname,Email,Password")] User user)
        {
            if (Auth.Check())
                return RedirectToAction("Index", "Auctions");


            if (ModelState.IsValid)
            {
                // hash password
                user.Password = hashPass(user.Password);

                user.NumTokens = 0;
                // check if email already exists - done
                
                db.Users.Add(user);
                db.SaveChanges();

                Flash.SuccessMessage("Successfully registered");
                return RedirectToAction("Login");
            }

            return View(user);
        }

        public ActionResult Login()
        {
            if (Auth.Check())
                return RedirectToAction("Index", "Auctions");


            return View();
        }

        // POST: Users/Login
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Email,Password")] User user)
        {
            if (Auth.Check())
                return RedirectToAction("Index", "Auctions");


            // hash password
            user.Password = hashPass(user.Password);

            var existingUser = db.Users.Where(u => u.Email == user.Email && u.Password == user.Password).FirstOrDefault();
            if(existingUser != null)
            {
                // success

                MakeLoggedIn(existingUser);
                Flash.SuccessMessage("Successfully logged in");

                return RedirectToAction("Index", "Auctions");
            }
            else
            {
                // failed to log in

                ViewBag.Message = "Email or password incorrect";

                // reset password
                user.Password = "";

                return View(user);
            }
            

            //ViewBag.Message = "Email or password incorrect";
            //return View(user);
        }

        public static string hashPass(string pass)
        {
            byte[] data = new byte[pass.Length];
            for (int i = 0; i < pass.Length; i++)
            {
                data[i] = (byte) pass[i];
            }

            return new string( System.Security.Cryptography.SHA256Managed.Create().ComputeHash(data).Select(b => (char) b).ToArray() );

        }

        public ActionResult Logout()
        {
            MakeLoggedOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Checks if current user is logged in.
        /// </summary>
        /// <returns></returns>
        public static bool AuthCheck()
        {
            return System.Web.HttpContext.Current.Session["LoggedIn"] != null;
        }

        public static void MakeLoggedIn(User user)
        {
            System.Web.HttpContext.Current.Session["LoggedIn"] = true;
            System.Web.HttpContext.Current.Session["user"] = user;
        }

        public static void MakeLoggedOut()
        {
            System.Web.HttpContext.Current.Session["LoggedIn"] = null;
            System.Web.HttpContext.Current.Session["user"] = null;
        }

        private ActionResult RedirectToLogin(string msg = "You must be logged in to access this page")
        {
            Flash.ErrorMessage(msg);
            return RedirectToAction("Login");
        }


        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if(!Auth.Check())
                return RedirectToLogin();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (Auth.Id != id)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            // empty the password
            user.Password = "";

            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Surname,Email,Password")] User user)
        {
            if (!Auth.Check())
                return RedirectToLogin();

            if (Auth.Id != user.Id)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (ModelState.IsValid)
            {
                // hash password
                user.Password = hashPass(user.Password);

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    User user = db.Users.Find(id);
        //    if (user == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(user);
        //}

        //// POST: Users/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    User user = db.Users.Find(id);
        //    db.Users.Remove(user);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
