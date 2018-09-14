$(document).ready(function () {
    $("#txtDocNo").on('blur', function () {
        var docType = $("#ddlDocType option:selected").text();
        var formatedCode;
        var invNo = $("#txtDocNo").val();
        if (invNo === "" || invNo === "undefined") {
            //toastr.warning("Doc Type mismatch or Doc match not found");
            $("#formDuplicatePrinting").formValidation('revalidateField', 'DocNo');
        }
        else if (/^\d+$/.test(invNo)) {
            formatedCode = docType + "/" + invNo + "/" + new Date().getFullYear();
            $("#txtDocNo").val(formatedCode);
            $("#formDuplicatePrinting").formValidation('revalidateField', 'DocNo');
        }
        else {

        }
    });

    if ($("#txtDocNo").val() !== "" && $("#txtDocNo").val() !== undefined) {
        //$("#formDuplicatePrinting").submit();
    }
    //var docNumber = "";
    //var validateArguments = function () {
    //    $('#formVoucherPrinting').formValidation();
    //    docNumber = $("#txtDocNumber").val();
    //    if (docNumber === "") {
    //        return false;
    //    }
    //    return true;
    //}
    //$("#btnSubmit").on("click", function () {
    //    var isValid = validateArguments();
    //    if (isValid) {
    //        var voucherType = $("#ddlVoucherType").val();
    //        $('#frameWrap').show();
    //        var url = "";
    //        if (voucherType === "0") {
    //            url = "/Reports/CashPaymentVP.aspx?isDuplicate=1&docNo=" + docNumber;
    //        }
    //        else if (voucherType === "1") {
    //            url = "/Reports/CashReceiptVP.aspx?isDuplicate=1&docNo=" + docNumber;
    //        }
    //        else if (voucherType === "2") {
    //            url = "/Reports/BankPaymentVP.aspx?isDuplicate=1&docNo=" + docNumber;
    //        }
    //        else if (voucherType === "3") {
    //            url = "/Reports/BankReceiptVP.aspx?isDuplicate=1&docNo=" + docNumber;
    //        }
    //        else if (voucherType === "4") {
    //            url = "/Reports/CashReceiptVP.aspx?isDuplicate=1&docNo=" + docNumber;
    //        }
    //        else if (voucherType === "5") {
    //            url = "/Reports/BankPaymentVP.aspx?isDuplicate=1&docNo=" + docNumber;
    //        }
    //        else if (voucherType === "6") {
    //            url = "/Reports/BankReceiptVP.aspx?isDuplicate=1&docNo=" + docNumber;
    //        }
    //        $('#iframe').prop('src', url);
    //    } else {
    //        toastr.error("Doc Number cannot be empty.");
    //    }
    //});
    $('#formDuplicatePrinting').on('init.field.fv', function (e, data) {
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
            DocNo: {
                validators: {
                    regexp: {
                        regexp: /[a-zA-Z]{3}\/[0-9]+\/[0-9]{4}/,
                        message: ''
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
        $("#divSaving").show();
    });
    $("#formDuplicatePrinting").formValidation('revalidateField', 'DocNo');
});