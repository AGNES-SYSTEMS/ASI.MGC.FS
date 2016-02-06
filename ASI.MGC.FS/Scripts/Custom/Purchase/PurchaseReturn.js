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

$(document).ready(function () {
    var arrPrdDetails = [];
    $('#txtDocDate').datepicker();
    $('#txtPurDate').datepicker();
    jQuery("#tblPrdDetails").jqGrid({
        datatype: "local",
        height: 100,
        autoheight: true,
        styleUI: "Bootstrap",
        gridview: true,
        shrinkToFit: true,
        viewrecords: true,
        colNames: ['Product Code', 'Product Description', 'Qty', 'Unit', 'Rate', 'Amount'],
        colModel: [
            { name: 'PRODID_SL', index: 'PRODID_SL', width: 100, align: "center", sortable: false },
            { name: 'PrdDesc', index: 'PrdDesc', width: 350, align: "left", sortable: false },
            { name: 'ISSUE_QTY_SL', index: 'ISSUE_QTY_SL', width: 100, align: "center", sortable: false },
            { name: 'UNIT_SL', index: 'UNIT_SL', width: 100, align: "center", sortable: false },
            { name: 'ISSUE_RATE_SL', index: 'ISSUE_RATE_SL', width: 100, align: "center", sortable: false },
            { name: 'Amount', index: 'Amount', width: 150, align: "center", sortable: false }
        ],
        multiselect: false,
        caption: "Product Details"
    });

    $(window).resize(function () {
        var outerwidthMrv = $('#gridProduct').width();
        $('#tblPrdDetails').setGridWidth(outerwidthMrv);
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
            { key: true, name: 'ARCODE_ARM', index: 'ARCODE_ARM', width: 300 },
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

        var reloadGrid = function (filterValue) {
            var postData = $("#tblCustomerSearch").jqGrid("getGridParam", "postData");
            postData["custName"] = filterValue;

            $("#tblCustomerSearch").setGridParam({ postData: postData });
            $("#tblCustomerSearch").trigger("reloadGrid", [{ page: 1 }]);
            $("#tblCustomerSearch").trigger("reloadGrid");
        };

        $("#custSearch").off().on("keyup", function () {
            var shouldSearch = $("#custSearch").val().length >= 3 || $("#custSearch").val().length === 0;
            if (shouldSearch) {
                reloadGrid($("#custSearch").val());
            }
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

        var reloadGrid = function (filterValue) {
            var postData = $("#tblProductSearch").jqGrid("getGridParam", "postData");
            postData["prdName"] = filterValue;

            $("#tblProductSearch").setGridParam({ postData: postData });
            $("#tblProductSearch").trigger("reloadGrid", [{ page: 1 }]);
            //$("#tblProductSearch").trigger("reloadGrid");
        };

        $("#prdSearch").off().on("keyup", function () {
            var shouldSearch = $("#prdSearch").val().length >= 3 || $("#prdSearch").val().length === 0;
            if (shouldSearch) {
                reloadGrid($("#prdSearch").val());
            }
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
    }
    $("#btnCancel").click(function () {
        clearModalForm();
    });
    $("#btnSave").click(function (e) {
        if ($("#purchaseProductModelform").valid()) {
            e.preventDefault();
            var arrIndex = arrPrdDetails.length;
            arrPrdDetails[arrIndex] = {
                PRODID_SL: $("#txtPrCode").val(), PrdDesc: $("#txtPrDesc").val(),
                ISSUE_QTY_SL: $("#txtQuantity").val(), ISSUE_RATE_SL: $("#txtRate").val(), UNIT_SL: $("#txtUnit").val(),
                Amount: $("#txtAmount").val()
            };
            var su = jQuery("#tblPrdDetails").jqGrid('addRowData', arrIndex, arrPrdDetails[arrIndex]);
            if (su) {
                var prdDetails = $('#tblPrdDetails').jqGrid('getGridParam', 'data');
                var jsonPrdDetails = JSON.stringify(prdDetails);
                $('#prdDetails').val(jsonPrdDetails);
                clearModalForm();
            }
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
    $("#purchaseProductModel").on('hide.bs.modal', function () {
        clearModalForm();
        var totalGridPrdAmount = 0.0;
        for (var i = 0; i < arrPrdDetails.length; i++) {
            totalGridPrdAmount += parseFloat(arrPrdDetails[i]["Amount"]);
        }
        $("#txtNetAmount").val(totalGridPrdAmount);
        $("#txtTotalAmount").val(totalGridPrdAmount);
    });

    $("#txtAPCode").on("change", function () {
        $('#formPurchaseReturn').formValidation('revalidateField', 'APCode');
    });

    $("#txtAPDetail").on("change", function () {
        $('#formPurchaseReturn').formValidation('revalidateField', 'APDetail');
    });

    $("#txtCash").on("change", function () {
        $('#formPurchaseReturn').formValidation('revalidateField', 'Cash');
    });

    $("#txtShipChrg").on("change", function () {
        $('#formPurchaseReturn').formValidation('revalidateField', 'ShipChrg');
    });

    $("#txtDiscount").on("change", function () {
        $('#formPurchaseReturn').formValidation('revalidateField', 'Discount');
    });

    $('#formPurchaseReturn').formValidation({
        container: '#messages',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            APCode: {
                validators: {
                    notEmpty: {
                        message: 'AP Code is required'
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
            }
        }
    }).on('success.form.fv', function (e) {
        debugger;
        // Prevent form submission
        e.preventDefault();
    });
});