using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace WebApplication1.Hubs
{
    public class AuctionHub : Hub
    {
        
        public static void UpdateClientAuctions(string auctionID, decimal newBidAmount, string userPosted, decimal newBidderNumTokens, string lastBidder,
            decimal lastBidderNumTokens)
        {
            UpdateData updateData = new UpdateData();
            updateData.AuctionID = auctionID;
            updateData.NewBidAmount = (double) newBidAmount;
            updateData.UserPosted = userPosted;
            updateData.NewBidderNumTokens = (double) newBidderNumTokens;
            updateData.LastBidder = lastBidder;
            updateData.LastBidderNumTokens = (double) lastBidderNumTokens;
            updateData.Currency = Settings.GlobalSettings.C;
            updateData.TokenValue = Settings.GlobalSettings.T;

            //Clients.All.updateAuction(updateData);

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<AuctionHub>();
            hubContext.Clients.All.updateAuction(updateData);
            
        }

        public static void SendSignalToClientsToCloseAuction(string auctionID)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<AuctionHub>();
            hubContext.Clients.All.closeAuction(auctionID);
        }

        public void AuctionCloseRequest(string auctionID)
        {
            // TODO:
            //Controllers.AuctionsController auctionsController = new Controllers.AuctionsController();
            //auctionsController.CloseAuction(auctionID);
            //SendSignalToClientsToCloseAuction(auctionID);
        }

    }

    public class UpdateData
    {
        [JsonProperty("auctionID")]
        public string AuctionID { get; set; }

        [JsonProperty("newBidAmount")]
        public double NewBidAmount { get; set; }

        [JsonProperty("userPosted")]
        public string UserPosted { get; set; }

        [JsonProperty("newBidderNumTokens")]
        public double NewBidderNumTokens { get; set; }

        [JsonProperty("lastBidder")]
        public string LastBidder { get; set; }

        [JsonProperty("lastBidderNumTokens")]
        public double LastBidderNumTokens { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("tokenValue")]
        public double TokenValue { get; set; }

    }
    
}
