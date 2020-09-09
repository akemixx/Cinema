// function to delete ticket from ticket cart
function DeleteTicket(id) {
    var btn = document.getElementById(id);
    var btnParentParentNode = btn.parentNode.parentNode;
    btnParentParentNode.removeChild(btn.parentNode);
    if (document.getElementsByClassName("ticket").length == 0) {
        btnParentParentNode.innerHTML = `<p>You should choose at least one seat.</p>
            <a href="/SessionTickets?IdSession=${
                document.getElementsByName("IdSession")[0].getAttribute("value")
                }">Move back</a>`;  
    }
}

// pay or make sale by personal bonuses 
async function PayByBonuses() {
    var totalPrice = parseFloat(document.getElementById("totalPrice").innerHTML);
    var bonuses = parseFloat(document.getElementById("bonuses").innerHTML.replace(",", "."));
    if (totalPrice >= bonuses) {
        totalPrice = totalPrice - bonuses;
        bonuses = 0;
    }
    else {
        bonuses = bonuses - totalPrice;
        totalPrice = 0;
    }
    document.getElementById("totalPrice").innerHTML = totalPrice;
    document.getElementsByName("totalPrice")[0].value = totalPrice;
    document.getElementById("bonuses").innerHTML = bonuses;

    // ajax request for renewing bonuses count
    let xsrf_token = document.getElementsByName("__RequestVerificationToken")[0].value;
    const url = '/SessionTickets/RenewBonusesCount';

    try {
        const response = await fetch(url, {
            method: 'POST',
            credentials: 'include',
            body: JSON.stringify(bonuses),
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