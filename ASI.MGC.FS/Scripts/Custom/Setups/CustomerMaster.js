$(document).ready(function () {
    $("#txtGlDate").datepicker();
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
            'Customer Code', 'Customer Name', 'PO Box', 'Address', 'Telephone', 'Fax', 'Email', 'Contact Person', 'Edit Actions'],
        colModel: [
            { key: true, name: 'ARCODE_ARM', index: 'ARCODE_ARM', width: 100, align: "center", sortable: false },
            { key: false, name: 'DESCRIPTION_ARM', index: 'DESCRIPTION_ARM', width: 150, align: "left", sortable: false },
            { key: false, name: 'POBOX_ARM', index: 'POBOX_ARM', width: 80, align: "center", sortable: false },
            { key: false, name: 'ADDRESS1_ARM', index: 'ADDRESS1_ARM', width: 150, align: "left", sortable: false },
            { key: false, name: 'TELEPHONE_ARM', index: 'TELEPHONE_ARM', width: 80, align: "center", sortable: false },
            { key: false, name: 'FAX_ARM', index: 'FAX_ARM', width: 80, align: "left", sortable: false },
            { key: false, name: 'EMAIL_ARM', index: 'EMAIL_ARM', width: 80, align: "center", sortable: false },
            { key: false, name: 'CONDACTPERSON_ARM', index: 'CONDACTPERSON_ARM', width: 150, align: "left", sortable: false },
            {
                name: "actions",
                width: 100,
                formatter: "actions",
                formatoptions: {
                    keys: true,
                    editOptions: {},
                    addOptions: {},
                    delOptions: {}
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

    var searchGrid = function (searchValue) {
        debugger;
        var postData = $("#tblCustomerDetails").jqGrid("getGridParam", "postData");
        postData["searchValue"] = searchValue;

        $("#tblCustomerDetails").setGridParam({ postData: postData });
        $("#tblCustomerDetails").trigger("reloadGrid", [{ page: 1 }]);
    };

    $("#txtCustSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtCustSearch").val().length >= 3 || $("#txtCustSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtCustSearch").val());
        }
    });

    $("#CustomerMasterModel").on('hide.bs.modal', function () {
        $(this).find('form')[0].reset();
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