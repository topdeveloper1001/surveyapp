/*
    new bootstrap date picker
        https://bootstrap-datepicker.readthedocs.io/en/latest/index.html
    No longer have to refer to each textbox by name - this will work on all datepickers on-screen
    Also no longer reliant on older versions of jQuery
*/
$(function () {
    $.fn.datepicker.defaults.format = "dd M yyyy"; // set standard format for all datepickers eg: 01 Oct 2017

    $('.input-group.date').datepicker({
        autoclose: true,
        enableOnReadonly: false,
        todayBtn: true,
        todayHighlight: true
    });
});