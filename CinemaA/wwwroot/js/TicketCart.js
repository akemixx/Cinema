/*
 * Scripts for TicketCart view of SessionTickets's Controller
 * Functions:
 * 1) Deleting ticket from the ticket cart.
 * 2) Paying or making sale for a ticket using user's bonuses, ajax request for changing DB data.
 */

// Delete a ticket from a ticket cart. 
function DeleteTicket(id) {
    var ticketDeleteButton = document.getElementById(id);
    var divWithAllTheTickets = ticketDeleteButton.parentNode.parentNode;
    divWithAllTheTickets.removeChild(ticketDeleteButton.parentNode);
    if (document.getElementsByClassName("ticket").length == 0) {
        divWithAllTheTickets.innerHTML = `<p>You should choose at least one seat.</p>
            <a href="/SessionTickets?sessionId=${
                document.getElementsByName("sessionId")[0].getAttribute("value")
                }">Move back</a>`;  
    }
}

// Pay or make a sale for a ticket using authenticated user's bonuses.
async function PayByBonuses() {
    var totalPrice = parseFloat(document.getElementById("totalPrice").innerHTML);
    var userBonuses = parseFloat(document.getElementById("bonuses").innerHTML.replace(",", "."));
    if (totalPrice >= userBonuses) {
        totalPrice = totalPrice - userBonuses;
        userBonuses = 0;
    }
    else {
        userBonuses = userBonuses - totalPrice;
        totalPrice = 0;
    }
    document.getElementById("totalPrice").innerHTML = totalPrice;
    document.getElementsByName("totalPrice")[0].value = totalPrice;
    document.getElementById("bonuses").innerHTML = userBonuses;

    // Ajax request for renewing bonuses count of a user in the DB.
    let xsrf_token = document.getElementsByName("__RequestVerificationToken")[0].value;
    const url = '/SessionTickets/RenewBonusesCountAjax';

    try {
        const response = await fetch(url, {
            method: 'POST',
            credentials: 'include',
            body: JSON.stringify(userBonuses),
            headers: {
                "XSRF-TOKEN": xsrf_token,
                "Content-Type": "application/json"
            }
        });
        if (response.status !== 200) {
            const error = await response.error();
            console.error(error);
        }
    }
    catch (err) {
        console.error("Error: ", err);
    }
}