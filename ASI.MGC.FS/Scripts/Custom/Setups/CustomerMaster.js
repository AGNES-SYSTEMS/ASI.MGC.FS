var fetchCustomerDetails = function (custCode) {
    var data = JSON.stringify({ custCode: custCode, custName: null });
    $.ajax({
        url: '/Customer/getCustomerRecord',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        type: "POST",
        success: function (custDetails) {
            $('#txtCustCode').val(custDetails.ARCODE_ARM).change();
            $('#txtCustName').val(custDetails.DESCRIPTION_ARM).change();
            $('#txtPoBox').val(custDetails.POBOX_ARM).change();
            $('#txtVATNo').val(custDetails.VATNO_ARM).change();
            $('#txtAddress1').val(custDetails.ADDRESS1_ARM);
            $('#txtAddress2').val(custDetails.ADDRESS2_ARM).change();
            $('#txtTelephone').val(custDetails.TELEPHONE_ARM).change();
            $('#txtFax').val(custDetails.FAX_ARM).change();
            $('#txtEmail').val(custDetails.EMAIL_ARM).change();
            $('#txtContactPerson').val(custDetails.CONDACTPERSON_ARM);
            $('#txtSaleExe').val(custDetails.SALESEXE_ARM);
            $('#txtStatus').val(custDetails.STATUS_ARM).change();
            $('#txtReceiveable').val(custDetails.RECEIVABLETYPE_ARM).change();
            $('#txtLimitAmount').val(custDetails.LIMITAMOUNT_ARM).change();
            $('#txtCreditDays').val(custDetails.CREDITDAYS_ARM);
            $('#txtNotes').val(custDetails.Notes_ARM).change();
            $('#txtCustCode').prop("readonly", true);
        },
        complete: function () {
        },
        error: function () {
        }
    });
}
$(document).ready(function () {
    $("#txtGlDate").datepicker({
        changeMonth: true,
        changeYear: true
    });
    var $custType = "AR";
    jQuery("#tblCustomerDetails").jqGrid({
        url: '/Customer/GetAllCustomers?custType=' + $custType,
        datatype: "json",
        height: 450,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: [
            'Customer Code', 'Customer Name', 'PO Box', 'Address', 'Telephone', 'Fax', 'Email', 'Contact Person', 'Action'],
        colModel: [
            { key: true, name: 'ARCODE_ARM', index: 'ARCODE_ARM', width: 100, align: "center", sortable: true },
            { key: false, name: 'DESCRIPTION_ARM', index: 'DESCRIPTION_ARM', width: 150, align: "left", sortable: false },
            { key: false, name: 'POBOX_ARM', index: 'POBOX_ARM', width: 80, align: "center", sortable: false },
            { key: false, name: 'ADDRESS1_ARM', index: 'ADDRESS1_ARM', width: 150, align: "left", sortable: false },
            { key: false, name: 'TELEPHONE_ARM', index: 'TELEPHONE_ARM', width: 80, align: "center", sortable: false },
            { key: false, name: 'FAX_ARM', index: 'FAX_ARM', width: 80, align: "left", sortable: false },
            { key: false, name: 'EMAIL_ARM', index: 'EMAIL_ARM', width: 80, align: "center", sortable: false },
            { key: false, name: 'CONDACTPERSON_ARM', index: 'CONDACTPERSON_ARM', width: 150, align: "left", sortable: false },
            {
                name: "action",
                align: "center",
                sortable: false,
                title: false,
                fixed: false,
                width: 50,
                search: false,
                formatter: function (cellValue, options) {

                    var markup = "<a %Href% data-toggle='modal' %Id% data-target='#CustomerMasterModel'> <i class='fa fa-pencil-square-o style='color:black'></i></a>";
                    var replacements = {
                        "%Href%": "href=javascript:editCustomerDetails(&apos;" + options.rowId + "&apos;)",
                        "%Id%": "id='" + options.rowId + "'"
                    };
                    markup = markup.replace(/%\w+%/g, function (all) {
                        return replacements[all];
                    });
                    return markup;
                }
            }
        ],
        rowNum: 40,
        rowList: [40, 100, 500, 1000],
        mtype: 'GET',
        gridview: true,
        viewrecords: true,
        sortorder: "desc",
        pager: jQuery('#Pager'),
        caption: "Customer Details",
        emptyrecords: "No Data to Display",
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false
        },
        multiselect: false
    }).navGrid("#Pager", { edit: false, add: false, del: false });
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblCustomerDetails').setGridWidth(outerwidth);
    });

    var searchGridCust = function (searchById, searchByName) {
        var custpostData = $("#tblCustomerDetails").jqGrid("getGridParam", "postData");
        custpostData["custId"] = searchById;
        custpostData["custName"] = searchByName;
        $("#tblCustomerDetails").setGridParam({ postData: custpostData });
        $("#tblCustomerDetails").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtCustIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtCustIdSearch").val().length >= 1 || $("#txtCustIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGridCust($("#txtCustIdSearch").val(), $("#txtCustNameSearch").val(), "3");
        }
    });
    $("#txtCustNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtCustNameSearch").val().length >= 3 || $("#txtCustNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGridCust($("#txtCustIdSearch").val(), $("#txtCustNameSearch").val(), "3");
        }
    });
    $("#CustomerMasterModel").on('hide.bs.modal', function () {
        $('#txtCustCode').prop("readonly", false);
        $(this).find('form')[0].reset();
    });

    $("#CustomerMasterModel").on('show.bs.modal', function (e) {
        if (e.relatedTarget.id) {
            fetchCustomerDetails(e.relatedTarget.id);
        }
    });
    $('#formCustomerMaster').on('init.field.fv', function (e, data) {
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
            ARCODE_ARM: {
                validators: {
                    notEmpty: {
                        message: 'AR Code is required'
                    }
                }
            },
            DESCRIPTION_ARM: {
                validators: {
                    notEmpty: {
                        message: 'Description is required'
                    }
                }
            },
            GLDate: {
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
            POBOX_ARM: {
                validators: {
                    notEmpty: {
                        message: 'P.O. Box is required'
                    }
                }
            },
            ADDRESS1_ARM: {
                validators: {
                    notEmpty: {
                        message: 'Address 1 is required'
                    }
                }
            },
            TELEPHONE_ARM: {
                validators: {
                    notEmpty: {
                        message: 'Telephone is required'
                    }
                }
            },
            CONDACTPERSON_ARM: {
                validators: {
                    notEmpty: {
                        message: 'Contact Person is required'
                    }
                }
            },
            LIMITAMOUNT_ARM: {
                validators: {
                    notEmpty: {
                        message: 'Limit Amount is required'
                    }
                }
            },
            CREDITDAYS_ARM: {
                validators: {
                    notEmpty: {
                        message: 'Credit Days is required'
                    }
                }
            },
            OpeningBalance: {
                validators: {
                    notEmpty: {
                        message: 'Opening Balance is required'
                    }
                }
            },
            VATNO_ARM: {
                validators: {
                    notEmpty: {
                        message: 'VAT No is required'
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
});