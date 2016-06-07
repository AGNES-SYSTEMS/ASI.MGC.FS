$(document).ready(function () {
    var $status = "IP";
    jQuery("#tblPrdDetails").jqGrid({
        url: '/Product/GetProdsList?status=' + $status,
        datatype: "json",
        height: 450,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: [
            'Product Code', 'Product Name', 'Current Qty', 'Rate', 'Selling Price',
        'Purchase Unit', 'Sale Unit', 'Unit', 'Product Status', 'Edit Actions'],
        colModel: [
            { key: true, name: 'PROD_CODE_PM', index: 'PROD_CODE_PM', width: 80, align: "center", sortable: false },
            { key: false, name: 'DESCRIPTION_PM', index: 'DESCRIPTION_PM', width: 150, align: "left", sortable: false },
            { key: false, name: 'CUR_QTY_PM', index: 'CUR_QTY_PM', width: 80, align: "left", sortable: false },
            { key: false, name: 'RATE_PM', index: 'RATE_PM', width: 80, align: "left", sortable: false },
            { key: false, name: 'SELLINGPRICE_RM', index: 'SELLINGPRICE_RM', width: 80, align: "left", sortable: false },
            { key: false, name: 'PURCHSEUNIT_PM', index: 'PURCHSEUNIT_PM', width: 80, align: "left", sortable: false },
            { key: false, name: 'SALESUNIT_PM', index: 'SALESUNIT_PM', width: 80, align: "left", sortable: false },
            { key: false, name: 'UNIT_PR', index: 'UNIT_PR', width: 80, align: "left", sortable: false },
            { key: false, name: 'STATUS_PM', index: 'STATUS_PM', width: 80, align: "left", sortable: false },
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
        caption: "Product Details",
        emptyrecords: "No Data to Display",
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false
        },
        multiselect: false
    }).navGrid("#Pager", { search: false, edit: false, add: false, del: false });
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblPrdDetails').setGridWidth(outerwidth);
    });
    var searchGrid = function (searchValue) {
        debugger;
        var postData = $("#tblPrdDetails").jqGrid("getGridParam", "postData");
        postData["searchValue"] = searchValue;

        $("#tblPrdDetails").setGridParam({ postData: postData });
        $("#tblPrdDetails").trigger("reloadGrid", [{ page: 1 }]);
    };

    $("#txtPrdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtPrdSearch").val().length >= 3 || $("#txtPrdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtPrdSearch").val());
        }
    });

    $("#ProductMasterModel").on('hide.bs.modal', function () {
        $(this).find('form')[0].reset();
    });

    $('#formProductMaster').on('init.field.fv', function (e, data) {
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
            PROD_CODE_PM: {
                validators: {
                    notEmpty: {
                        message: 'Prod Code is required'
                    }
                }
            },
            DESCRIPTION_PM: {
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
            CUR_QTY_PM: {
                validators: {
                    notEmpty: {
                        message: 'Current Qty is required'
                    }
                }
            },
            RATE_PM: {
                validators: {
                    notEmpty: {
                        message: 'Rate is required'
                    }
                }
            },
            SELLINGPRICE_RM: {
                validators: {
                    notEmpty: {
                        message: 'Selling Price is required'
                    }
                }
            },
            PURCHSEUNIT_PM: {
                validators: {
                    notEmpty: {
                        message: 'Purchase Unit is required'
                    }
                }
            },
            SALESUNIT_PM: {
                validators: {
                    notEmpty: {
                        message: 'Sales Unit is required'
                    }
                }
            },
            UNIT_PR: {
                validators: {
                    notEmpty: {
                        message: 'Unit is required'
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