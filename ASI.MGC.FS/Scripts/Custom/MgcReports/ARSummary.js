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
                url = "/Reports/ARSummary.aspx?startDate=" + startDate + "&endDate=" + endDate + "&isExportMode=" + isExportMode;
            } else {
                url = "/Reports/ARSummary.aspx?startDate=" + startDate + "&endDate=" + endDate;
            }
            $('#iframe').prop('src', url);
        } else {
            toastr.error("Start Date/ End Date cannot be empty.");
        }
    });

    $('#iframe').on('load', function () {
        $('#loader').hide();
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