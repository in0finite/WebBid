using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1
{
    public class Auth
    {

        //private static string lastDate = "";
        private static HttpRequest lastRequest = null;


        public static bool Check()
        {
            return WebApplication1.Controllers.UsersController.AuthCheck();
        }

        public static User User
        {
            get
            {
                var currentRequest = HttpContext.Current.Request;
                if (currentRequest != lastRequest)
                {
                    lastRequest = currentRequest;
                    // update user variable
                    HttpContext.Current.Session["user"] = new Model1().Users.Find(Auth.Id);
                }
                return (User)HttpContext.Current.Session["user"];
            }
        }

        public static int Id { get { if (!Check()) return -1; else return User.Id; } }
        
        public static bool IsAdmin { get { return Auth.Check() && Auth.User.IsAdmin(); } }

        private static string GetCurrentRequestDate()
        {
            return HttpContext.Current.Request.Headers["Date"];
        }

        public static void RefreshUser()
        {
            if (!Auth.Check())
                return;
            HttpContext.Current.Session["user"] = new Model1().Users.Find(Auth.Id);
        }

    }
}