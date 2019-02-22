    /*
        new bootstrap date picker
            https://bootstrap-datepicker.readthedocs.io/en/latest/index.html
    */
    $(function () {
        // date pickers don't auto-fire a "change" event - make it so...
        $('.input-group.date').on('changeDate', function (e) {
            // get the associated questionId & new date from this control
            var questionId = $(this).attr('questionId');
            var formattedDate = e.format('dd M yyyy');

            // save the changes..
            saveClientTargetDate(questionId, formattedDate);
        });
        });
            
    function saveClientResponse(questionID, controlRef) {
            // get the current text from the box
            var oControl = document.getElementById(controlRef);
            var answerText = oControl.value;

            ajaxCall(questionID, answerText, 'null', 'null');
    }

    function saveClientTargetDate(questionID, targetDate) {
        ajaxCall(questionID, 'null', targetDate, 'null');
    }

    function closeClientResponse(questionID) {

        bootbox.confirm({
            scrollTop: 0,
            title: "Confirm item close",
            message: "Are you sure you want to close this item?",
            buttons: {
                cancel: {
                    label: '<i class="fa fa-times"></i> No, cancel'
                },
                confirm: {
                    label: '<i class="fa fa-check"></i> Yes, close the item',
                    className: 'btn-warning'
                }
            },
            callback: function (result) {
                if (result == true) {
                    ajaxCall(questionID, 'null', 'null', '1');      // close it!

                    // let's not go back to the server..just hide that row now!!
                    $("#row_q" + questionID).hide();                 
                    $("#row_a" + questionID).hide(); 
                            
                    // now, see how many questions are left..
                    //  if nothing left, then refresh the page..
                    //  Note that there are TWO rowheaders per line but that's ok, we just need to see zero
                    var numOpenItems = $('.rowheader:visible').length;
                    if (numOpenItems == 0)
                        location.reload();
                }
            }
        });
    }
        