function DeleteTicket(id) {
    var btn = document.getElementById(id);
    btn.parentNode.parentNode.removeChild(btn.parentNode);
}