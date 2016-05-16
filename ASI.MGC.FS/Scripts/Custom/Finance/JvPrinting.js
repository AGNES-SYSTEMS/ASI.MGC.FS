$(document).ready(function () {

    var docNumber = "";

    var validateArguments = function () {
        $('#formJvPrinting').formValidation();
        docNumber = $("#txtDocNumber").val();
        if (docNumber === "") {
            return false;
        }
        return true;
    }

    $("#btnSubmit").on("click", function () {
        var isValid = validateArguments();
        if (isValid) {
            $('#frameWrap').show();
            var url = "/Reports/JournalVoucher.aspx?jvNo=" + docNumber;
            $('#iframe').prop('src', url);
        } else {
            toastr.error("Doc Number cannot be empty.");
        }
    });

    $('#iframe').on('load', function () {
        $('#loader').hide();
    });
    $('#formJvPrinting').formValidation({
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