// List of options at: http://bootstrapvalidator.com/validators/
//  http://www.jqueryscript.net/form/Powerful-Form-Validation-Plugin-For-jQuery-Bootstrap-3.html
$(document).ready(function () {
    $('#frm')
        .formValidation({
            framework: 'bootstrap',
            icon: {
                valid: 'glyphicon glyphicon-ok',
                invalid: 'glyphicon glyphicon-remove',
                validating: 'glyphicon glyphicon-refresh'
            },
            live: 'submitted',
            message: 'This is a required input and cannot be blank',
            fields: {
                ctl00$ContentPlaceHolder1$txtClientApprovalComments: { validators: { notEmpty: {} } }
            }
        })
        .on('err.field.fv', function (e, data) {
            if (data.fv.getSubmitButton()) {
                data.fv.disableSubmitButtons(false);
            }
        })
        .on('success.field.fv', function (e, data) {
            if (data.fv.getSubmitButton()) {
                data.fv.disableSubmitButtons(false);
            }
        });
});

// disable comments box validation on "approve" button
$("#ContentPlaceHolder1_cmdClientAccept").click(function () {
    $('#frm').formValidation('enableFieldValidators', 'ctl00$ContentPlaceHolder1$txtClientApprovalComments', false);
});