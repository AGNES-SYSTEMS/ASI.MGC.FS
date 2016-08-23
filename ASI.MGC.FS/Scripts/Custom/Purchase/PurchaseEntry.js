var arrPrdDetails = [];
var selectedRowId = "";
var customerSelect = function (custId) {
    if (custId) {
        var ret = jQuery("#tblCustomerSearch").jqGrid('getRowData', custId);
        $("#txtAPCode").val(ret.ARCODE_ARM).change();
        $("#txtAPDetail").val(ret.DESCRIPTION_ARM).change();
        $('#CustomerSearchModel').modal('toggle');
    }
};
var productSelect = function (prdId) {
    if (prdId) {
        var ret = jQuery("#tblProductSearch").jqGrid('getRowData', prdId);
        $("#txtPrCode").val(ret.PROD_CODE_PM).change();
        $("#txtPrDesc").val(ret.DESCRIPTION_PM).change();
        $("#txtUnit").val(ret.PURCHSEUNIT_PM).change();
        $('#PrdSearchModel').modal('toggle');
    }
};
var delProduct = function (rowId) {
    if (rowId) {
        $('#tblPrdDetails').jqGrid('delRowData', rowId);
        $('#tblPrdDetails').trigger('reloadGrid');
        stringifyData();
        calculateNetAmount();
    }
};
var calculateNetAmount = function () {
    var totalGridPrdAmount = 0.0;
    for (var i = 0; i < arrPrdDetails.length; i++) {
        totalGridPrdAmount += parseFloat(arrPrdDetails[i]["Amount"]);
    }
    var netAmount = parseFloat(totalGridPrdAmount) + parseFloat($("#txtShipChrg").val()) - parseFloat($("#txtDiscount").val());
    $("#txtNetAmount").val(netAmount);
    $("#txtTotalAmount").val(totalGridPrdAmount);
    $("#formPurchaseEntry").formValidation('revalidateField', 'NetAmount');
    $("#formPurchaseEntry").formValidation('revalidateField', 'TotalAmount');
}
var stringifyData = function () {
    var prdDetails = $('#tblPrdDetails').jqGrid('getGridParam', 'data');
    var jsonPrdDetails = JSON.stringify(prdDetails);
    $('#prdDetails').val(jsonPrdDetails);
};
$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liPurchaseEntry").addClass("active");
    $('#txtDocDate').datepicker();
    $('#txtPurDate').datepicker();
    jQuery("#tblPrdDetails").jqGrid({
        datatype: "local",
        data: arrPrdDetails,
        height: 100,
        autoheight: true,
        styleUI: "Bootstrap",
        gridview: true,
        shrinkToFit: true,
        viewrecords: true,
        colNames: ['Product Code', 'Product Description', 'Qty', 'Unit', 'Rate', 'Amount', '', ''],
        colModel: [
            { name: 'PRODID_SL', index: 'PRODID_SL', width: 100, align: "center", sortable: false },
            { name: 'PrdDesc', index: 'PrdDesc', width: 350, align: "left", sortable: false },
            { name: 'RECEPT_QTY_SL', index: 'RECEPT_QTY_SL', width: 100, align: "center", sortable: false },
            { name: 'UNIT_SL', index: 'UNIT_SL', width: 100, align: "center", sortable: false },
            { name: 'RECEPT_RATE_SL', index: 'RECEPT_RATE_SL', width: 100, align: "center", sortable: false },
            { name: 'Amount', index: 'Amount', width: 150, align: "center", sortable: false },
            {
                name: "action",
                align: "center",
                sortable: false,
                title: false,
                fixed: false,
                width: 50,
                search: false,
                formatter: function (cellValue, options) {

                    var markup = "<a %Href% data-toggle='modal' %Id% data-target='#purchaseProductModel'> <i class='fa fa-pencil-square-o style='color:black'></i></a>";
                    var replacements = {
                        "%Href%": "href=javascript:editProduct(&apos;" + options.rowId + "&apos;);",
                        "%Id%": "id='" + options.rowId + "'"
                    };
                    markup = markup.replace(/%\w+%/g, function (all) {
                        return replacements[all];
                    });
                    return markup;
                }
            },
            {
                name: "action",
                align: "center",
                sortable: false,
                title: false,
                fixed: false,
                width: 50,
                search: false,
                formatter: function (cellValue, options) {

                    var markup = "<a %Href%><i class='fa fa-trash-o style='color:black'></i></a>";
                    var replacements = {
                        "%Href%": "href=javascript:delProduct(&apos;" + options.rowId + "&apos;);"
                    };
                    markup = markup.replace(/%\w+%/g, function (all) {
                        return replacements[all];
                    });
                    return markup;
                }
            }
        ],
        multiselect: false,
        caption: "Product Details"
    });
    $("#btnNew").on("click", function () {
        location.reload();
    });
    var searchGridCust = function (searchById, searchByName) {
        var custpostData = $("#tblCustomerSearch").jqGrid("getGridParam", "postData");
        custpostData["custId"] = searchById;
        custpostData["custName"] = searchByName;
        $("#tblCustomerSearch").setGridParam({ postData: custpostData });
        $("#tblCustomerSearch").trigger("reloadGrid", [{ page: 1 }]);
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
    $(window).resize(function () {
        var outerwidthMrv = $('#gridProduct').width();
        $('#tblPrdDetails').setGridWidth(outerwidthMrv);
    });
    $("#btnNew").on("click", function () {
        location.reload();
    });
    $("#CustomerSearchModel").on('show.bs.modal', function () {
        $("#tblCustomerSearch").jqGrid({
            url: '/Customer/GetAPCustomerDetailsList',
            datatype: "json",
            height: 150,
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Customer Code', 'Customer Name', ''],
            colModel: [
            { key: true, name: 'ARCODE_ARM', index: 'ARCODE_ARM', width: 400 },
            { key: false, name: 'DESCRIPTION_ARM', index: 'DESCRIPTION_ARM', width: 400 },
            {
                name: "action",
                align: "center",
                sortable: false,
                title: false,
                fixed: false,
                width: 50,
                search: false,
                formatter: function (cellValue, options, rowObject) {

                    var markup = "<a %Href%> <i class='fa fa-check-square-o style='color:black'></i></a>";
                    var replacements = {
                        "%Href%": "href=javascript:customerSelect(&apos;" + rowObject.ARCODE_ARM + "&apos;);"
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
            shrinkToFit: true,
            viewrecords: true,
            sortorder: "asc",
            pager: jQuery('#custPager'),
            caption: "Customers List",
            emptyrecords: "No Data to Display",
            jsonReader: {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false
            },
            multiselect: false
        });
    });
    $(window).resize(function () {
        var outerwidth = $('#custGrid').width();
        $('#tblCustomerSearch').setGridWidth(outerwidth);
    });
    $("#btnCustSelect").on("click", function (e) {
        var id = jQuery("#tblCustomerSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblCustomerSearch").jqGrid('getRowData', id);
            $("#txtAPCode").val(ret.ARCODE_ARM).change();
            $("#txtAPDetail").val(ret.DESCRIPTION_ARM).change();
            $('#CustomerSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    $("#PrdSearchModel").on('show.bs.modal', function () {
        $("#tblProductSearch").jqGrid({
            url: '/Product/GetIPProductDetailsList',
            datatype: "json",
            height: 150,
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Product Code', 'Product Details', 'Purchase Unit', 'Sales Unit', ''],
            colModel: [
            { key: true, name: 'PROD_CODE_PM', index: 'PROD_CODE_PM', width: 200 },
            { key: false, name: 'DESCRIPTION_PM', index: 'DESCRIPTION_PM', width: 250 },
            { key: false, name: 'PURCHSEUNIT_PM', index: 'PURCHSEUNIT_PM', width: 175 },
            { key: false, name: 'SALESUNIT_PM', index: 'SALESUNIT_PM', width: 175 },
            {
                name: "action",
                align: "center",
                sortable: false,
                title: false,
                fixed: false,
                width: 50,
                search: false,
                formatter: function (cellValue, options, rowObject) {

                    var markup = "<a %Href%> <i class='fa fa-check-square-o style='color:black'></i></a>";
                    var replacements = {
                        "%Href%": "href=javascript:productSelect(&apos;" + rowObject.PROD_CODE_PM + "&apos;);"
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
            shrinkToFit: true,
            viewrecords: true,
            sortorder: "asc",
            pager: jQuery('#prdPager'),
            caption: "Product Details List",
            emptyrecords: "No Data to Display",
            jsonReader: {
                root: "rows",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false
            },
            multiselect: false
        });
    });
    $(window).resize(function () {
        var outerwidth = $('#prdGrid').width();
        $('#tblProductSearch').setGridWidth(outerwidth);
    });
    $("#btnProductSelect").on("click", function (e) {
        var id = jQuery("#tblProductSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblProductSearch").jqGrid('getRowData', id);
            $("#txtPrCode").val(ret.PROD_CODE_PM).change();
            $("#txtPrDesc").val(ret.DESCRIPTION_PM).change();
            $("#txtUnit").val(ret.PURCHSEUNIT_PM).change();
            $('#PrdSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    $("#txtRate").on("blur", function () {
        var $amount = $("#txtRate").val() * $("#txtQuantity").val();
        $("#txtAmount").val($amount);
    });
    $("#txtQuantity").on("blur", function () {
        var $amount = $("#txtRate").val() * $("#txtQuantity").val();
        $("#txtAmount").val($amount);
    });
    function clearModalForm() {
        $("#txtPrCode").val("");
        $("#txtPrDesc").val("");
        $("#txtUnit").val("");
        $("#txtQuantity").val("1");
        $("#txtRate").val("");
        $("#txtAmount").val("0");
        selectedRowId = "";
    }
    $("#btnCancel").click(function () {
        clearModalForm();
    });
    $("#btnSave").click(function (e) {
        if ($("#purchaseProductModelform").valid()) {
            e.preventDefault();
            if (selectedRowId) {
                arrPrdDetails[selectedRowId - 1] = {
                    PRODID_SL: $("#txtPrCode").val(),
                    PrdDesc: $("#txtPrDesc").val(),
                    RECEPT_QTY_SL: $("#txtQuantity").val(),
                    RECEPT_RATE_SL: $("#txtRate").val(),
                    UNIT_SL: $("#txtUnit").val(),
                    Amount: $("#txtAmount").val()
                }
            }
            else {
                var arrIndex = arrPrdDetails.length;
                arrPrdDetails[arrIndex] = {
                    PRODID_SL: $("#txtPrCode").val(),
                    PrdDesc: $("#txtPrDesc").val(),
                    RECEPT_QTY_SL: $("#txtQuantity").val(),
                    RECEPT_RATE_SL: $("#txtRate").val(),
                    UNIT_SL: $("#txtUnit").val(),
                    Amount: $("#txtAmount").val()
                }
            };
            clearModalForm();
        }
        else {
            $("#purchaseProductModelform").bootstrapValidator('revalidateField', 'PrCode');
            $("#purchaseProductModelform").bootstrapValidator('revalidateField', 'PrDesc');
            $("#purchaseProductModelform").bootstrapValidator('revalidateField', 'Quantity');
            $("#purchaseProductModelform").bootstrapValidator('revalidateField', 'Unit');
            $("#purchaseProductModelform").bootstrapValidator('revalidateField', 'Rate');
            $("#purchaseProductModelform").bootstrapValidator('revalidateField', 'Amount');
        }
    });
    $("#purchaseProductModel").on('show.bs.modal', function (e) {
        if (e.relatedTarget.id) {
            selectedRowId = e.relatedTarget.id;
            var rowId = e.relatedTarget.id;
            var ret = $('#tblPrdDetails').jqGrid('getRowData', rowId);
            $("#txtPrCode").val(ret.PRODID_SL);
            $("#txtPrDesc").val(ret.PrdDesc);
            $("#txtQuantity").val(ret.RECEPT_QTY_SL);
            $("#txtRate").val(ret.RECEPT_RATE_SL);
            $("#txtUnit").val(ret.UNIT_SL);
            $("#txtAmount").val(ret.Amount);
        }
    });
    $("#purchaseProductModel").on('hide.bs.modal', function () {
        clearModalForm();
        $('#tblPrdDetails').trigger('reloadGrid');
        stringifyData();
        calculateNetAmount();
    });
    $("#txtAPCode").on("change", function () {
        $('#formPurchaseEntry').bootstrapValidator('revalidateField', 'APCode');
    });
    $("#txtAPDetail").on("change", function () {
        $('#formPurchaseEntry').bootstrapValidator('revalidateField', 'APDetail');
    });
    $("#txtCash").on("change", function () {
        $('#formPurchaseEntry').bootstrapValidator('revalidateField', 'Cash');
    });
    $("#txtShipChrg").on("change", function () {
        calculateNetAmount();
        $('#formPurchaseEntry').bootstrapValidator('revalidateField', 'ShipChrg');
    });
    $("#txtDiscount").on("change", function () {
        calculateNetAmount();
        $('#formPurchaseEntry').bootstrapValidator('revalidateField', 'Discount');
    });
    $('#formPurchaseEntry').on('init.field.fv', function (e, data) {
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
            NetAmount: {
                validators: {
                    notEmpty: {
                        message: 'Net Amount is required'
                    }
                }
            },
            APDetail: {
                validators: {
                    notEmpty: {
                        message: 'AP Code Detail is required'
                    }
                }
            },
            DocDate: {
                validators: {
                    notEmpty: {
                        message: 'Document Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            },
            PurDate: {
                validators: {
                    notEmpty: {
                        message: 'Purchase Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            },
            Invoice: {
                validators: {
                    notEmpty: {
                        message: 'Invoice is required'
                    }
                }
            },
            TotalAmount: {
                validators: {
                    notEmpty: {
                        message: 'Total Amount is required'
                    }
                }
            },
            ShipChrg: {
                validators: {
                    notEmpty: {
                        message: 'Ship Charges Detail is required'
                    },
                    integer: {
                        message: 'Integer Only'
                    }
                }
            },
            Discount: {
                validators: {
                    notEmpty: {
                        message: 'Discount Detail is required'
                    },
                    integer: {
                        message: 'Integer Only'
                    }
                }
            }
        }
    })
     .on('status.field.fv', function (e, data) {
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