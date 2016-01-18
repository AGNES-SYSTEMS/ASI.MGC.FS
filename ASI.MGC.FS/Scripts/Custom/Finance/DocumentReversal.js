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
                    } else {
                        toastr.error("Invalid Document Number.");
                    }
                },
                complete: function () {
                },
                error: function () {
                    toastr.error("Something went wrong. Please try again");
                }
            });
        }
    }
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
    $('#formDocumentReversal').formValidation({
        container: '#messages',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            InvNumber: {
                validators: {
                    notEmpty: {
                        message: 'INV Code is required'
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