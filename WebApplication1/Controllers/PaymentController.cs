using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1
{
    public class PaymentController : ApiController
    {
        private static string successString = "success!";
        private static string errorString = "failed";

        //private Model2 db = new Model2();
        private Model1 db = new Model1();


        // GET api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        //[Route("/payment/{clientid}")]
        public string Get(string clientid, string status)
        {
            
            using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {

                    // find order by id
                    var tokenOrder = this.db.TokenOrders.Find(clientid);
                    if (null == tokenOrder)
                        return errorString;

                    if (tokenOrder.Status != "SUBMITTED")
                        return "already processed";

                    var user = db.Users.Find(tokenOrder.UserId);

                    if (status != "success")
                    {
                        // payment failed

                        tokenOrder.Status = "CANCELED";
                        db.Entry(tokenOrder).State = EntityState.Modified;
                        
                    }
                    else
                    {
                        // payment succeeded

                        tokenOrder.Status = "COMPLETED";
                        db.Entry(tokenOrder).State = EntityState.Modified;

                        if (user != null)
                        {
                            // add tokens to user
                            user.NumTokens += tokenOrder.NumTokens;
                            db.Entry(user).State = EntityState.Modified;
                        }

                    }
                    

                    db.SaveChanges();
                    transaction.Commit();


                    if (user != null)
                    {
                        // notify him by email
                        SendMail(user, tokenOrder);
                    }

                    return successString;
                }
                catch(Exception)
                {
                    transaction.Rollback();

                    return "transaction error";
                }

            }

        }

        public static void SendMail(User user, TokenOrder tokenOrder)
        {
            Utils.SendMail(user.Email, Utils.AppName + " - payment", GetMailText(user, tokenOrder));
        }

        public static string GetMailText(User user, TokenOrder tokenOrder)
        {
            return string.Format( "Hi, {0} {1},\n\nYour payment changed status:\nID - {2}\nNum tokens - {3}\nStatus - {4}\n\n{5} team", user.Name, user.Surname, tokenOrder.GUID, tokenOrder.NumTokens, tokenOrder.Status, Utils.AppName );
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}