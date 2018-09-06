using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    public class Flash
    {

        public static void Message(string msg)
        {
            System.Web.HttpContext.Current.Session["Message"] = msg;
        }

        public static void SuccessMessage(string msg)
        {
            System.Web.HttpContext.Current.Session["SuccessMessage"] = msg;
        }

        public static void ErrorMessage(string msg)
        {
            System.Web.HttpContext.Current.Session["ErrorMessage"] = msg;
        }


        public static bool HasMessage()
        {
            return System.Web.HttpContext.Current.Session["Message"] != null;
        }

        public static bool HasSuccessMessage()
        {
            return System.Web.HttpContext.Current.Session["SuccessMessage"] != null;
        }

        public static bool HasErrorMessage()
        {
            return System.Web.HttpContext.Current.Session["ErrorMessage"] != null;
        }


        public static string GetMessage()
        {
            string msg = (string) System.Web.HttpContext.Current.Session["Message"];
            System.Web.HttpContext.Current.Session["Message"] = null;
            return msg;
        }

        public static string GetSuccessMessage()
        {
            string msg = (string)System.Web.HttpContext.Current.Session["SuccessMessage"];
            System.Web.HttpContext.Current.Session["SuccessMessage"] = null;
            return msg;
        }

        public static string GetErrorMessage()
        {
            string msg = (string)System.Web.HttpContext.Current.Session["ErrorMessage"];
            System.Web.HttpContext.Current.Session["ErrorMessage"] = null;
            return msg;
        }

    }
}