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
                    ctl00$ContentPlaceHolder1$cboSurveyTemplate: { validators: { greaterThan: { value: 1, message: 'Please select a valid template from the list of options' } } },
                    ctl00$ContentPlaceHolder1$cboAuditor: { validators: { greaterThan: { value: 1, message: 'Please select a consultant' } } },
                    ctl00$ContentPlaceHolder1$txtDateOfAudit: {
                        validators: {
                            callback: {
                                message: 'The date of audit is not valid',
                                callback: function (value, validator, $field) {
                                    if (value == '') {
                                        return false;
                                    }

                                    console.log("Checking Date [" + value + "]");

                                    // Check if the value has any of our supported formats..
                                    //  http://formvalidation.io/examples/supporting-multiple-date-formats/
                                    return moment(value, 'DD MMM YYYY', true).isValid()
                                        || moment(value, 'DD MM YYYY', true).isValid();
                                }
                            }
                        }
                    },
                    ctl00$ContentPlaceHolder1$txtClientContact: { validators: { notEmpty: {} } },
                    ctl00$ContentPlaceHolder1$txtSiteDesc: { validators: { notEmpty: {} } },
                    ctl00$ContentPlaceHolder1$txtScopeOfWork: { validators: { notEmpty: {} } },
                    ctl00$ContentPlaceHolder1$txtWeatherConditions: { validators: { notEmpty: {} } }
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

        