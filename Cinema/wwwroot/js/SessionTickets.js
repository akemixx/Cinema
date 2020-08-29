// button click event listeners
$('.btn-success').on("click", SelectSeatClick);
$('.btn-warning').on("click", UnselectSeatClick);
$('.btn-danger').on("click", function () {
    alert("Sorry, this seat is busy. Choose another one.");
});
$('.btn-secondary').on("click", function () {
    alert("Sorry, this seat has been already booked. Choose another one.");
});

// select seat button click
function SelectSeatClick() {
    $(this).removeClass('btn-success');
    $(this).addClass('btn-warning');
    $(this).attr('name', "SelectedSeats");
    $(this).unbind("click", SelectSeatClick);
    $(this).on("click", UnselectSeatClick);
}

// select seat button click
function UnselectSeatClick() {
    $(this).removeClass('btn-warning');
    $(this).addClass('btn-success');
    $(this).removeAttr('name');
    $(this).unbind("click", UnselectSeatClick);
    $(this).on("click", SelectSeatClick);
}