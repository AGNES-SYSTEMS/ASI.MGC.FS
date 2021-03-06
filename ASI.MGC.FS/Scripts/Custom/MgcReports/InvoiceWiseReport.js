﻿$(document).ready(function () {
    $("#txtStartDate").datepicker({ changeMonth: true, changeYear: true });
    $("#txtEndDate").datepicker({ changeMonth: true, changeYear: true });
    var startDate = "";
    var endDate = "";
    var validateArguments = function () {
        startDate = $("#txtStartDate").val();
        endDate = $("#txtEndDate").val();
        if (startDate === "" || endDate === "") {
            return false;
        }
        return true;
    }

    $("#btnReportSubmit").on("click", function () {
        $("#btnReportSubmit").prop('disabled', false);
        $("#btnReportSubmit").removeAttr('disabled');
        $("#btnReportSubmit").removeClass('disabled');
        var isValid = validateArguments();
        if (isValid) {
            $('#frameWrap').show();
            var url = "";
            var isExportMode = getQueryStringByName("isExportMode", document.location.href);
            if (isExportMode !== null && isExportMode !== "") {
                url = "/Reports/InvoiceWiseReport.aspx?startDate=" + startDate + "&endDate=" + endDate + "&isExportMode=" + isExportMode;
            } else {
                url = "/Reports/InvoiceWiseReport.aspx?startDate=" + startDate + "&endDate=" + endDate;
            }
            $('#iframe').prop('src', url);
        } else {
            toastr.error("Start Date/ End Date cannot be empty.");
        }
    });

    $('#iframe').on('load', function () {
        $('#loader').hide();
    });

    $("#txtStartDate").change(function () {
        $("#formInvoiceWiseReport").formValidation('revalidateField', 'startDate');
    });
    $("#txtEndDate").change(function () {
        $("#formInvoiceWiseReport").formValidation('revalidateField', 'endDate');
    });
    $('#formInvoiceWiseReport').on('init.field.fv', function (e, data) {
        var $icon = data.element.data('fv.icon'),
            options = data.fv.getOptions(),
            validators = data.fv.getOptions(data.field).validators;

        if (validators.notEmpty && options.icon && options.icon.required) {
            $icon.addClass(options.icon.required).show();
        }
    }).formValidation({
        container: '#messages',
        icon: {
            required: 'fa fa-asterisk',
            valid: 'fa fa-check',
            invalid: 'fa fa-times',
            validating: 'fa fa-refresh'
        },
        fields: {
            startDate: {
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
            endDate: {
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
    }).on('status.field.fv', function (e, data) {
        // Remove the required icon when the field updates its status
        var $icon = data.element.data('fv.icon'),
            options = data.fv.getOptions(),                      // Entire options
            validators = data.fv.getOptions(data.field).validators; // The field validators

        if (validators.notEmpty && options.icon && options.icon.required) {
            $icon.removeClass(options.icon.required).addClass('fa');
        }
    }).on('success.field.fv', function (e, data) {
        if (data.fv.getInvalidFields().length > 0) {    // There is invalid field
            data.fv.disableSubmitButtons(true);
        }
    }).on('success.form.fv', function (e) {
        debugger;
        // Prevent form submission
        e.preventDefault();
    });
});