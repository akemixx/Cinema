$('.btn-success').on("click", BookClick);
$('.btn-warning').on("click", UnbookClick);
$('.btn-danger').on("click", function () {
    alert("Sorry, this seat is busy. Choose another one.");
});
$('.btn-secondary').on("click", function () {
    alert("Sorry, this seat has been already booked. Choose another one.");
});

function BookClick() {
    $(this).removeClass('btn-success');
    $(this).addClass('btn-warning');
    $(this).attr('name', "BookedSeats");
    $(this).unbind("click", BookClick);
    $(this).on("click", UnbookClick);
}

function UnbookClick() {
    $(this).removeClass('btn-warning');
    $(this).addClass('btn-success');
    $(this).removeAttr('name');
    $(this).unbind("click", UnbookClick);
    $(this).on("click", BookClick);
}