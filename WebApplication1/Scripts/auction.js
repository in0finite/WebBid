




$.connection.hub.start();


$.connection.auctionHub.client.updateAuction = function (updateData) {
    updateAuction(updateData);
};

$.connection.auctionHub.client.closeAuction = function (auctionID) {
    onAuctionClosed(auctionID);
}


function updateAuction(updateData) {
    
    document.getElementById("user " + updateData.auctionID).innerHTML = updateData.userPosted;
    document.getElementById("price " + updateData.auctionID).innerHTML = (updateData.newBidAmount * updateData.tokenValue) + " " + updateData.currency;
    document.getElementById("price " + updateData.auctionID).className += " bg-danger text-white";

    //var offerInput = $("offerInput " + updateData.auctionID);
    var newMinValue = Number(updateData.newBidAmount) + 1;
    var transformedMinValue = newMinValue * updateData.tokenValue;

    var offerInput = document.getElementById("form_" + updateData.auctionID).elements["offerInput " + updateData.auctionID];
    offerInput.value = transformedMinValue;
    offerInput.min = transformedMinValue;
    offerInput.step = updateData.tokenValue;

    // update num tokens

    var navDisplayName = document.getElementById("NavDisplayName");
    var navNumTokens = document.getElementById("NavNumTokens");

    if (navDisplayName != null && navNumTokens != null) {
        if (navDisplayName.innerHTML == updateData.userPosted) {
            setNumTokensHtml(navNumTokens, updateData.newBidderNumTokens);
        } else if (navDisplayName.innerHTML == updateData.lastBidder) {
            setNumTokensHtml(navNumTokens, updateData.lastBidderNumTokens);
        }
    }
    
}

function setNumTokensHtml(elem, numTokens) {
    elem.innerHTML = "Tokens [" + numTokens + "]";
}


function setIntervalForAuction(id, countDownDate) {

    // Update the count down every 1 second
    var x = setInterval(function () {

        if (document.getElementById("timer " + id).innerHTML == "CLOSED") {
            clearInterval(x);
            return;
        }

        // Get local date and time
        var localTime = new Date();

        // convert it to UTC date and time
        //var now = new Date(localTime.getUTCFullYear(), localTime.getUTCMonth(), localTime.getUTCDate(),
        //    localTime.getUTCHours(), localTime.getUTCMinutes(), localTime.getUTCSeconds(), localTime.getUTCMilliseconds());
        var now = localTime;

        // Find the distance between now an the count down date
        var distance = countDownDate.getTime() - now.getTime();

        // Time calculations for days, hours, minutes and seconds
        var days = Math.floor(distance / (1000 * 60 * 60 * 24));
        var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        var seconds = Math.floor((distance % (1000 * 60)) / 1000);
        
        // Display the result in the element
        var timerElem = document.getElementById("timer " + id);
        //timerElem.innerHTML = days + "d " + hours + "h " + minutes + "m " + seconds + "s ";
        timerElem.innerHTML = ("" + (distance / 1000)).toHHMMSS();

        if (distance < 0) {
            // countdown finished

            //debugger;

            clearInterval(x);

            onAuctionClosed(id);
            
            //$.connection.auctionHub.server.auctionCloseRequest(id);
        }

    }, 1000);

}

function onAuctionClosed(auctionID) {

    //document.getElementById("state " + auctionID).innerHTML = "CLOSED";
    document.getElementById("timer " + auctionID).innerHTML = "CLOSED";
    document.getElementById("bidButton " + auctionID).className += " disabled";
    document.getElementById("bidButton " + auctionID).disabled = true;

}


String.prototype.toHHMMSS = function () {
    var sec_num = parseInt(this, 10); // don't forget the second param
    var hours = Math.floor(sec_num / 3600);
    var minutes = Math.floor((sec_num - (hours * 3600)) / 60);
    var seconds = sec_num - (hours * 3600) - (minutes * 60);

    if (hours < 10) { hours = "0" + hours; }
    if (minutes < 10) { minutes = "0" + minutes; }
    if (seconds < 10) { seconds = "0" + seconds; }
    return hours + ':' + minutes + ':' + seconds;
}

