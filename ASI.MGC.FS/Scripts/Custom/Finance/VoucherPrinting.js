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
    $('#formVoucherPrinting').formValidation({
        container: '#messages',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
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
    }).on('success.form.fv', function (e) {
        debugger;
        // Prevent form submission
        e.preventDefault();
    });
});