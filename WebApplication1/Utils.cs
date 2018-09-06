using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace WebApplication1
{
    public static class Utils
    {

        public static string AppName { get { return "WebBid"; } }

        public static long ToJsMilliseconds(this DateTime dateTime)
        {
            return (long) dateTime
               .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
               .TotalMilliseconds;
        }

        public static void SendMail(string emailAddress, string subject, string body)
        {

            

        }

        public static List<T> PreparePagedView<T>(int numItemsPerPage, int pageNumber, List<T> coll, dynamic ViewBag)
        {
            int numPages = (int)Math.Ceiling(coll.Count / (double) numItemsPerPage);

            if (pageNumber > numPages)
                pageNumber = numPages;

            ViewBag.pageNumber = pageNumber;
            ViewBag.numPages = numPages;

            return coll.Skip(pageNumber * numItemsPerPage).Take(numItemsPerPage).ToList();
        }

        public static double TransformToCurrency(this decimal? price)
        {
            if (!price.HasValue)
                return 0;

            return Settings.GlobalSettings.T * (double) price.Value;
        }

        public static double TokenValue { get { return Settings.GlobalSettings.T; } }

    }
}