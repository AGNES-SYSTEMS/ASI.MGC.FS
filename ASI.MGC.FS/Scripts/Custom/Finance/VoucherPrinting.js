$(document).ready(function () {

    var docNumber = "";
    var validateArguments = function () {
        $('#formVoucherPrinting').formValidation();
        docNumber = $("#txtDocNumber").val();
        if (docNumber === "") {
            return false;
        }
        return true;
    }

    $("#btnSubmit").on("click", function () {
        $("#btnSubmit").prop('disabled', false);
        $("#btnSubmit").removeAttr('disabled');
        $("#btnSubmit").removeClass('disabled');
        var isValid = validateArguments();
        if (isValid) {
            var voucherType = $("#ddlVoucherType").val();
            $('#frameWrap').show();
            var url = "";
            if (voucherType === "1") {
                url = "/Reports/CashPaymentVP.aspx?docNo=" + docNumber;
            }
            else if (voucherType === "2") {
                url = "/Reports/CashReceiptVP.aspx?docNo=" + docNumber;
            }
            else if (voucherType === "3") {
                url = "/Reports/BankPaymentVP.aspx?docNo=" + docNumber;
            }
            else if (voucherType === "4") {
                url = "/Reports/BankReceiptVP.aspx?docNo=" + docNumber;
            }
            $('#iframe').prop('src', url);
        } else {
            toastr.error("Doc Number cannot be empty.");
        }
    });

    $('#iframe').on('load', function () {
        $('#loader').hide();
    });
    $('#formVoucherPrinting').on('init.field.fv', function (e, data) {
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
            DocNumber: {
                validators: {
                    notEmpty: {
                        message: 'Doc Number is required'
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