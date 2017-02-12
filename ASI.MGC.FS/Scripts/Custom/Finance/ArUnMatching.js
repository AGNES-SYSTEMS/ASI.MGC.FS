$(document).ready(function () {
    jQuery("#tblUnMatchingDetails").jqGrid({
        url: '/Finance/GetArUnMatchDetails',
        datatype: "json",
        height: 250,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: ['Doc No', 'Amount', 'Match Number'],
        colModel: [
            { name: 'DOCCNUMBER_ARM', index: 'DOCCNUMBER_ARM', width: 150, align: "center", sortable: false },
            { name: 'AMOUNT_ARM', index: 'AMOUNT_ARM', width: 150, align: "left", sortable: false },
            { name: 'MATCHNO_ARM', index: 'MATCHNO_ARM', width: 150, align: "left", sortable: false }
        ],
        rowNum: 1000,
        multiselect: false,
        caption: "Allocation Details",
        loadComplete: function (data) {
            if ($("#tblUnMatchingDetails").getGridParam("reccount") <= 0 && $("#txtInvoiceNumber").val() !== "")
            {
                toastr.warning("Doc Type mismatch or Doc match not found");
            }
        }
    });
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblUnMatchingDetails').setGridWidth(outerwidth);
    });
    $("#txtInvoiceNumber").on('blur', function () {
        var docType = $("#ddlDocType option:selected").text();
        var formatedCode;
        var invNo = $("#txtInvoiceNumber").val();
        if (invNo === "" || invNo === "undefined") {
            toastr.warning("Doc Type mismatch or Doc match not found");
            $("#formArUnMatching").formValidation('revalidateField', 'DOCCNUMBER_ARM');
        }
        else if (/^\d+$/.test(invNo)) {
            formatedCode = docType + "/" + invNo + "/" + new Date().getFullYear();
            $("#txtInvoiceNumber").val(formatedCode);
            $("#formArUnMatching").formValidation('revalidateField', 'DOCCNUMBER_ARM');
        }
        var partyPostData = $("#tblUnMatchingDetails").jqGrid("getGridParam", "postData");
        partyPostData["invNo"] = invNo;
        $("#tblUnMatchingDetails").setGridParam({ postData: partyPostData });
        $("#tblUnMatchingDetails").trigger("reloadGrid", [{ page: 1 }]);
    });
    $('#formArUnMatching').on('init.field.fv', function (e, data) {
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
            DOCCNUMBER_ARM: {
                validators: {
                    regexp: {
                        regexp: /[a-zA-Z]{3}\/[0-9]+\/[0-9]{4}/,
                        message: 'The full name can consist of alphabetical characters and spaces only'
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