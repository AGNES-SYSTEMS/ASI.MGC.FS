$(document).ready(function () {
    $("#txtDayTo").datepicker();
    $("#btnNew").on("click", function () {
        location.reload();
    });
    $("#btnStart").on("click", function () {
        var startDate = new Date($('#txtDayFrom').val());
        var endDate = new Date($('#txtDayTo').val());
        if (startDate !== "" && endDate !== null) {
            var data = JSON.stringify({ startDate: startDate, endDate: endDate });
            $.ajax({
                url: '/Finance/GetDayEndProcessData',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: data,
                type: "POST",
                success: function (status) {
                    if (status.unPostedSales > 0) {
                        toastr.info("Sale Cannot be Closed!! There are " + status.unPostedSales + " transanctions not posted ");
                    } else {
                        $('#txtJobTotal').val(status.jobTotal);
                        $('#txtSalesTotal').val(status.salesTotal);
                        $('#txtShippingTotal').val(status.discountTotal);
                        $('#txtDiscountTotal').val(status.shippingTotal);
                    }
                },
                complete: function () {
                },
                error: function () {
                    toastr.error("Sorry! Something went wrong, please try again.");
                }
            });
        }
    });
    $('#formDayEndProcess').on('init.field.fv', function (e, data) {
        var $icon = data.element.data('fv.icon'),
            options = data.fv.getOptions(),
            validators = data.fv.getOptions(data.field).validators;

        if (validators.notEmpty && options.icon && options.icon.required) {
            $icon.addClass(options.icon.required).show();
        }
    }).formValidation({
        container: '#messages',
        framework: 'bootstrap',
        icon: {
            required: 'fa fa-asterisk',
            valid: 'fa fa-check',
            invalid: 'fa fa-times',
            validating: 'fa fa-refresh'
        },
        fields: {
            DayFrom: {
                validators: {
                    notEmpty: {
                        message: 'GL Code is required'
                    }
                }
            },
            Date: {
                validators: {
                    notEmpty: {
                        message: 'GL Description is required'
                    }
                }
            },
            DayTo: {
                validators: {
                    notEmpty: {
                        message: 'Sub GL Code is required'
                    }
                }
            },
            LastUpdateDate: {
                validators: {
                    notEmpty: {
                        message: 'GL Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            },
            DocumentNo: {
                validators: {
                    notEmpty: {
                        message: 'Document No is required'
                    }
                }
            },
            LastDocumentNo: {
                validators: {
                }
            },
            JobTotal: {
                validators: {
                    notEmpty: {
                        message: 'Job Total is required'
                    }
                }
            },
            SalesTotal: {
                validators: {
                    notEmpty: {
                        message: 'Sales Total is required'
                    }
                }
            },
            ShippingTotal: {
                validators: {
                    notEmpty: {
                        message: 'Shipping Total is required'
                    }
                }
            },
            DiscountTotal: {
                validators: {
                    notEmpty: {
                        message: 'Discount Total is required'
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
    $("#formArUnMatching").formValidation('revalidateField', 'DOCCNUMBER_ARM');
});