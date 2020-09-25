/* 
 * Scripts for Index view of SessionTickets's Controller
 * Functions:
 * 1) Selecting and unselecting seats.
 * 2) Alerting when the seat is booked or bought.
 */

// Event listener on free seats to select a seat.
$('.btn-success').on("click", SelectSeatClick);

// Event listener on selected seats to unselect a seat.
$('.btn-warning').on("click", UnselectSeatClick);

// Event listener on already bought seats to show an alert.
$('.btn-danger').on("click", function () {
    alert("Sorry, this seat is busy. Choose another one.");
});

// Event listener on already booked seats to show an alert.
$('.btn-secondary').on("click", function () {
    alert("Sorry, this seat has been already booked. Choose another one.");
});

/* Mark a seat as selected. */
function SelectSeatClick() {
    $(this).removeClass('btn-success');
    $(this).addClass('btn-warning');
    $('#'+($(this).attr('value'))).attr('name', 'selectedTicketsIds')
    $(this).unbind('click', SelectSeatClick);
    $(this).on('click', UnselectSeatClick);
}

/* Unselect a seat. */
function UnselectSeatClick() {
    $(this).removeClass('btn-warning');
    $(this).addClass('btn-success');
    $('#' + ($(this).attr('value'))).removeAttr('name')
    $(this).unbind("click", UnselectSeatClick);
    $(this).on("click", SelectSeatClick);
}