using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplication1
{
    
    public class ImageHandler : IHttpHandler
    {
        public static int WIDTH = 300;
        public static int HEIGHT = 300;

        public void ProcessRequest(HttpContext context)
        {
            MyLog.Log("ProcessRequest for image");
            
           String auctionNo;
            if (context.Request.QueryString["id"] != null)
                auctionNo = context.Request.QueryString["id"];
            else
                throw new ArgumentException("No parameter specified");

            MyLog.Log("Showing image with id: " + auctionNo);

            context.Response.ContentType = "image/jpeg";
            Stream strm = ShowAuctionImage(auctionNo);
            byte[] buffer = new byte[4096];
            if (strm == null)
            {
                //MyLog.Log("Stream is null");
                return;
            }
            int byteSeq = strm.Read(buffer, 0, 4096);

            while (byteSeq > 0)
            {
                context.Response.OutputStream.Write(buffer, 0, byteSeq);
                byteSeq = strm.Read(buffer, 0, 4096);
            }
            //context.Response.BinaryWrite(buffer);

            MyLog.Log("Sent whole image");
        }

        public Stream ShowAuctionImage(string auctionNo)
        {
            
            try
            {
                Model1 db = new Model1();
                object img = db.Auctions.Find(auctionNo).Image;
                if (null == img)
                    return null;
                return new MemoryStream(((byte[])img).CreateThumbnail(WIDTH, HEIGHT));
            }
            catch (Exception e)
            {
                MyLog.Log("ShowAuctionImage exception: \n" + e.ToString());
                return null;
            }
            finally
            {
                // connection.Close();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}