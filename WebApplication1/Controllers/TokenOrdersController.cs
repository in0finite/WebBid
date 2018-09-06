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
    public class TokenOrdersController : Controller
    {
        private Model1 db = new Model1();

        // GET: TokenOrders
        public ActionResult Index(int pageNumber = 0)
        {
            if (!Auth.Check())
                return RedirectToLogin();
            
            var myOrders = db.TokenOrders.Where(o => o.UserId == Auth.Id).ToList();

            //int numPages = (int) Math.Ceiling( myOrders.Count / 10.0 );

            //if (pageNumber > numPages)
            //    pageNumber = numPages;

            //ViewBag.pageNumber = pageNumber;
            //ViewBag.numPages = numPages;

            //return View(myOrders.Skip(pageNumber * 10).ToList());

            return View(Utils.PreparePagedView(WebApplication1.Settings.GlobalSettings.N, pageNumber, myOrders, ViewBag));
        }

        // GET: TokenOrders/Details/5
        //public ActionResult Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TokenOrder tokenOrder = db.TokenOrders.Find(id);
        //    if (tokenOrder == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tokenOrder);
        //}

        // GET: TokenOrders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TokenOrders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NumTokens")] TokenOrder tokenOrder)
        {

            if (!Auth.Check())
                return RedirectToLogin();


            if (ModelState.IsValid)
            {
                tokenOrder.GUID = Guid.NewGuid().ToString();
                tokenOrder.UserId = Auth.Id;
                tokenOrder.Status = "SUBMITTED";
                tokenOrder.PackagePrice = (decimal) (tokenOrder.NumTokens * GetTokenPrice());
                tokenOrder.DateSubmitted = DateTime.UtcNow;
                
                db.TokenOrders.Add(tokenOrder);
                db.SaveChanges();

                Flash.SuccessMessage("Order submitted");
                return RedirectToAction("Index");
            }

            return View(tokenOrder);
        }

        public static double GetTokenPrice()
        {
            return Settings.GlobalSettings.T;
        }


        private ActionResult RedirectToLogin(string msg = "You must be logged in to access this page")
        {
            Flash.ErrorMessage(msg);
            return RedirectToAction("Login", "Users");
        }


        // GET: TokenOrders/Edit/5
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TokenOrder tokenOrder = db.TokenOrders.Find(id);
        //    if (tokenOrder == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tokenOrder);
        //}

        // POST: TokenOrders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "GUID,UserId,NumTokens,PackagePrice,Status")] TokenOrder tokenOrder)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(tokenOrder).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(tokenOrder);
        //}

        // GET: TokenOrders/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TokenOrder tokenOrder = db.TokenOrders.Find(id);
        //    if (tokenOrder == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tokenOrder);
        //}

        //// POST: TokenOrders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    TokenOrder tokenOrder = db.TokenOrders.Find(id);
        //    db.TokenOrders.Remove(tokenOrder);
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
