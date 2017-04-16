function ToJavaScriptDate(value) {
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));
    return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
}
$(document).ready(function () {
    debugger;
    function getInvDetails() {
        var invCode = $("#txtInvNumber").val();
        if (invCode !== "") {
            var data = JSON.stringify({ invCode: invCode });
            $.ajax({
                url: '/Finance/GetInvDetails',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: data,
                type: "POST",
                success: function (invDetails) {
                    debugger;
                    if (invDetails !== undefined && invDetails !== null) {
                        $("#txtDocDate").val(ToJavaScriptDate(invDetails.DOCDATE_BT)).change();
                        $("#txtOtherRef").val(invDetails.OTHERREF_BT).change();
                        $("#txtAccCode").val(invDetails.BANKCODE_BT).change();
                        if (invDetails.CREDITAMOUT_BT !== "" && parseInt(invDetails.CREDITAMOUT_BT) !== 0) {
                            $("#txtAmount").val(invDetails.CREDITAMOUT_BT).change();

                        } else {
                            $("#txtAmount").val(invDetails.DEBITAMOUT_BT).change();
                        }
                        $("#txtStatus").val(invDetails.STATUS_BT).change();

                        if (invDetails.STATUS_BT === "R") {
                            toastr.info("Document already reversed.");
                            $("#btnSave").prop("disabled", true);
                        } else {
                            $("#btnSave").prop("disabled", false);
                        }
                        $("#formDocumentReversal").formValidation('revalidateField', 'AccCode');
                        $("#formDocumentReversal").formValidation('revalidateField', 'DocDate');
                        $("#formDocumentReversal").formValidation('revalidateField', 'OtherRef');
                        $("#formDocumentReversal").formValidation('revalidateField', 'Status');
                        $("#formDocumentReversal").formValidation('revalidateField', 'Amount');
                    } else {
                        toastr.error("Invalid Document Number.");

                    }
                },
                complete: function () {
                    if ( $("#txtStatus").val() === "R") {
                        $("#btnSave").hide();
                    }
                },
                error: function (err) {
                    toastr.error("Something went wrong. Please try again");
                }
            });
        }
    }
    $("#btnNew").on("click", function () {
        location.reload(true);
    });
    $("#txtInvNumber").on("blur", function () {
        var ddlVal = $("#ddlDocType").val();
        if (ddlVal !== "8" && ddlVal !== "9" && ddlVal !== "15" && ddlVal !== "16") {
            getInvDetails();
        } else {
            if ($("#txtInvNumber") !== "") {
                toastr.error("Invalid Document Number.");
            }
        }
    });
    $('#formDocumentReversal').on('init.field.fv', function (e, data) {
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
            InvNumber: {
                validators: {
                    notEmpty: {
                        message: 'INV Code is required'
                    }
                }
            },
            DocDate: {
                validators: {
                    notEmpty: {
                        message: 'Document Date is required'
                    }
                }
            },
            OtherRef: {
                validators: {
                    notEmpty: {
                        message: 'Other Ref is required'
                    }
                }
            },
            AccCode: {
                validators: {
                    notEmpty: {
                        message: 'Account Code is required'
                    }
                }
            }, Amount: {
                validators: {
                    notEmpty: {
                        message: 'Amount is required'
                    }
                }
            },
            Status: {
                validators: {
                    notEmpty: {
                        message: 'Status is required'
                    }
                }
            }
        }
    }).on('status.field.fv', function (e, data) {
        // Remove the required icon when the field updates its status
        var $icon = data.element.data('fv.icon'),
            options = data.fv.getOptions(),
            validators = data.fv.getOptions(data.field).validators;

        if (validators.notEmpty && options.icon && options.icon.required) {
            $icon.removeClass(options.icon.required).addClass('fa');
        }
    }).on('success.field.fv', function (e, data) {
        if (data.fv.getInvalidFields().length > 0) {
            data.fv.disableSubmitButtons(true);
        }
    }).on('success.form.fv', function (e) {
        debugger;
        // Prevent form submission
        e.preventDefault();
    });
});