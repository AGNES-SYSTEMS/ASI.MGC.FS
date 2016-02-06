$(document).ready(function () {
    $("#txtStartDate").datepicker();
    $("#txtEndDate").datepicker();

    var validateArguments = function () {
        if ($("#txtStartDate").val() === "" || $("#txtStartDate").val() === null) {
            return false;
        }

        if ($("#txtEndDate").val() === "" || $("#txtEndDate").val() === null) {
            return false;
        }

        return true;
    }

    $("#btnApply").on("click", function () {
            toastr.error("Start Date/ End Date cannot be empty.");
    });

    $('#formBalanceSheet').formValidation({
        framework: 'bootstrap',
        icon: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            StartDate: {
                validators: {
                    notEmpty: {
                        message: 'The date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'The date is not a valid'
                    }
                }
            },
            EndDate: {
                validators: {
                    notEmpty: {
                        message: 'The date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'The date is not a valid'
                    }
                }
            }
        }
    }).on('success.form.fv', function (e) {
        debugger;
        // Prevent form submission
        e.preventDefault();
    });
});