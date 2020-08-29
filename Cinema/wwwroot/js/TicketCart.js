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