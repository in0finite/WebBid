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
    public class AuctionsController : Controller
    {
        private Model1 db = new Model1();

        // GET: Auctions
        public ActionResult Index(string productName, int? status, decimal? minPrice, decimal? maxPrice, int? count, string showOnlyWonAuctions = "", 
            int pageNumber = 0)
        {
            RemoveExpiredAuctions();

            bool bShowOnlyWon = "on" == showOnlyWonAuctions;
            //MyLog.Log("showOnlyWonAuctions: " + showOnlyWonAuctions);

            IEnumerable<Auction> auctions = db.Auctions.ToList();

            if (!string.IsNullOrEmpty(productName))
                auctions = auctions.Where(a => a.Name.Contains(productName));

            if (maxPrice.HasValue)
                auctions = auctions.Where(a => a.CurrentPrice != null && a.CurrentPrice.Value <= maxPrice.Value);

            if (minPrice.HasValue)
                auctions = auctions.Where(a => a.CurrentPrice != null && a.CurrentPrice.Value >= minPrice.Value);

            if (status.HasValue && status != -1 && !bShowOnlyWon)
                auctions = auctions.Where(a => a.Status == status);

            if (bShowOnlyWon && Auth.Check())
                auctions = auctions.Where(a => a.IsClosed() && GetLastBidder(a) != null && GetLastBidder(a).Id == Auth.Id);

            if (!count.HasValue)
                count = WebApplication1.Settings.GlobalSettings.N;

            auctions = auctions.OrderBy(a => !a.IsOpened()).ThenBy(a => a.DateOpened);  //.Take(count.Value);

            return View( Utils.PreparePagedView( count.Value, pageNumber, auctions.ToList(), ViewBag ) );
        }


        private void RemoveExpiredAuctions()
        {

            var auctionsToEnd = this.db.Auctions.ToList().Where(a => a.Status == 1 && a.DateOpened.Value.AddSeconds(a.Duration.Value) <= DateTime.UtcNow).ToList();

            foreach(var auction in auctionsToEnd)
            {
                auction.Status = 2;
                auction.DateClosed = DateTime.UtcNow;

                db.Entry(auction).State = EntityState.Modified;

                // TODO: notify all clients
                Hubs.AuctionHub.SendSignalToClientsToCloseAuction(auction.GUID);

                MyLog.Log("Expired auction: " + auction.Name);
            }

            if(auctionsToEnd.Count > 0)
                db.SaveChanges();

        }

        private void RemoveAuctionIfExpired(string GUID)
        {

            var auction = this.db.Auctions.Find(GUID);

            if(auction != null && auction.Status == 1 && auction.DateOpened.Value.AddSeconds(auction.Duration.Value) <= DateTime.UtcNow)
            {
                // should be closed
                auction.Status = 2;
                auction.DateClosed = DateTime.UtcNow;

                db.Entry(auction).State = EntityState.Modified;
                db.SaveChanges();

                // TODO: notify all clients
                Hubs.AuctionHub.SendSignalToClientsToCloseAuction(auction.GUID);

                MyLog.Log("Expired auction: " + auction.Name);
            }

        }


        // GET: Auctions/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            RemoveAuctionIfExpired(id);

            Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }

            return View(auction);
        }

        // GET: Auctions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Duration,StartingPrice")] Auction auction, HttpPostedFileBase image)
        {
            if (!Auth.Check())
                return RedirectToLogin();


            ViewBag.ImageMsg = "";


            if (ModelState.IsValid)
            {
                // read image
                if(null == image)
                {
                    ViewBag.ImageMsg = "Image is required";
                    return View(auction);
                }

                if (!image.IsImage() || !image.IsImageSupported())
                {
                    ViewBag.ImageMsg = "Only jpeg image types are supported";
                    return View(auction);
                }

                if (image != null && image.IsImage())
                {
                    auction.Image = new byte[image.ContentLength];
                    image.InputStream.Read(auction.Image, 0, image.ContentLength);
                    //auction.Image = image.CreateThumbnail(150, 150);
                }
                
                // generate GUID
                auction.GUID = Guid.NewGuid().ToString();

                // creator id
                auction.CreatorId = Auth.Id;

                // creation date
                auction.DateCreated = DateTime.UtcNow;

                auction.Status = 0;

                // duration
                if (!auction.Duration.HasValue)
                    auction.Duration = WebApplication1.Settings.GlobalSettings.D;


                db.Auctions.Add(auction);
                db.SaveChanges();

                Flash.SuccessMessage("Auction created");
                return RedirectToAction("Index");
            }


            return View(auction);
        }
        

        private ActionResult RedirectToLogin(string msg = "You must be logged in to access this page")
        {
            Flash.ErrorMessage(msg);
            return RedirectToAction("Login", "Users");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bid(string GUID, decimal tokensOffered, string OnDetailsPage = null)
        {
            if (!Auth.Check())
                return RedirectToLogin();

            string error = "Failed to place bid";


            // first refresh Auth user
            //Auth.RefreshUser();

            // modify tokens offered based on currency
            tokensOffered = ( tokensOffered / (decimal) Utils.TokenValue );


            using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    
                    var existingAuction = this.db.Auctions.Find(GUID);

                    if(existingAuction != null && existingAuction.IsOpened() && existingAuction.CurrentPrice.Value < tokensOffered )
                    {
                        decimal numRemainingTokens = Auth.User.NumTokens.Value - tokensOffered;

                        var lastBidder = GetLastBidder(existingAuction);

                        if (lastBidder != null && Auth.Id == lastBidder.Id)
                            numRemainingTokens += existingAuction.CurrentPrice.Value;

                        if (numRemainingTokens >= 0)
                        {
                    
                            // insert new bid
                            Bid bid = new Bid();
                            bid.AuctionID = existingAuction.GUID;
                            bid.UserID = Auth.Id;
                            bid.UserName = Auth.User.DisplayName;
                            bid.DateOfBidding = DateTime.UtcNow;
                            bid.TokensOffered = tokensOffered;

                            db.Bids.Add(bid);
                    
                            // update num tokens for last bidder
                            if (lastBidder != null)
                            {
                                lastBidder.NumTokens += existingAuction.CurrentPrice;
                                db.Entry(lastBidder).State = EntityState.Modified;
                            }

                            // we must modify the same instance
                            var user = Auth.User;
                            if (lastBidder != null && lastBidder.Id == user.Id)
                                user = lastBidder;

                            // update amount of tokens for new bidder
                            user.NumTokens = numRemainingTokens;
                            db.Entry(user).State = EntityState.Modified;

                            // update current price of auction
                            existingAuction.CurrentPrice = tokensOffered;
                            db.Entry(existingAuction).State = EntityState.Modified;


                            db.SaveChanges();

                            transaction.Commit();


                            // update all clients
                            Hubs.AuctionHub.UpdateClientAuctions(existingAuction.GUID, tokensOffered, user.DisplayName, user.NumTokens.Value, lastBidder != null ? lastBidder.DisplayName : "",
                                lastBidder != null ? lastBidder.NumTokens.Value : 0);


                            Flash.SuccessMessage("Bid placed successfully");
                            return RedirectAfterBid(OnDetailsPage, GUID);
                        }
                        else
                        {
                            error = "You don't have enough tokens";
                        }
                    }
            

                    Flash.ErrorMessage(error);
                    return RedirectAfterBid(OnDetailsPage, GUID);


                }
                catch (Exception)
                {
                    transaction.Rollback();

                    Flash.ErrorMessage("Transaction error");
                    return RedirectAfterBid(OnDetailsPage, GUID);
                }

            }


        }

        private ActionResult RedirectAfterBid(string OnDetailsPage, string auctionID)
        {
            if (OnDetailsPage == null)
                return RedirectToAction("Index");
            else
                return RedirectToAction("Details", new { id = auctionID });
        }

        public static User GetLastBidder(Auction auction)
        {
            var db = new Model1();

            var bid = db.Bids.Where(b => b.AuctionID == auction.GUID).OrderByDescending(b => b.DateOfBidding).FirstOrDefault();
            if (bid != null)
                return db.Users.Find(bid.UserID);
            return null;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Open(string GUID)
        {
            if (!Auth.Check() || !Auth.User.IsAdmin())
                return RedirectToLogin();

            // admin wants to open auction

            var existingAuction = this.db.Auctions.Find(GUID);
            if(null == existingAuction)
                return HttpNotFound();

            string error = "Invalid input";

            if(existingAuction.StatusText != "READY")
            {
                error = "You can only open ready auctions";
            }
            else
            {
                
                existingAuction.Status = 1;
                existingAuction.CurrentPrice = existingAuction.StartingPrice;
                existingAuction.DateOpened = DateTime.UtcNow;
                existingAuction.DateClosed = DateTime.UtcNow.AddSeconds((double) existingAuction.Duration);
                    
                db.Entry(existingAuction).State = EntityState.Modified;
                db.SaveChanges();

                // TODO: notify all clients


                Flash.SuccessMessage("Auction opened successfully");
                return RedirectToAction("Index");
                
            }

            Flash.ErrorMessage(error);
            return RedirectToAction("Index");
        }


        public ActionResult Settings()
        {
            if (!Auth.IsAdmin)
                return RedirectToLogin("admin only");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeSettings(int numberOfAuctions, int duration, int silver, int gold, int platinum, string currency, double tokenValue)
        {
            if (!Auth.IsAdmin)
                return RedirectToLogin("admin only");


            WebApplication1.Settings.GlobalSettings.N = numberOfAuctions;
            WebApplication1.Settings.GlobalSettings.D = duration;
            WebApplication1.Settings.GlobalSettings.S = silver;
            WebApplication1.Settings.GlobalSettings.G = gold;
            WebApplication1.Settings.GlobalSettings.P = platinum;
            WebApplication1.Settings.GlobalSettings.T = tokenValue;
            WebApplication1.Settings.GlobalSettings.C = currency;

            WebApplication1.Settings.GlobalSettings.SaveToFile();

            Flash.SuccessMessage("Settings are saved");
            return RedirectToAction("Settings");
        }


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
