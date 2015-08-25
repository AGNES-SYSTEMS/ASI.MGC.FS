﻿$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liPurchaseEntry").addClass("active");
    var arrPrdDetails = [];
    $('#txtDocDate').datepicker();
    $('#txtPurDate').datepicker();
    jQuery("#tblPrdDetails").jqGrid({
        datatype: "local",
        height: 100,
        colNames: ['Product Code', 'Product Description', 'Qty', 'Unit', 'Rate', 'Amount'],
        colModel: [
            { name: 'PrdCode', index: 'PrdCode', width: 100, align: "center", sortable: false },
            { name: 'PrdDesc', index: 'PrdDesc', width: 350, align: "left", sortable: false },
            { name: 'Qty', index: 'Qty', width: 100, align: "center", sortable: false },
            { name: 'Unit', index: 'Unit', width: 100, align: "center", sortable: false },
            { name: 'Rate', index: 'Rate', width: 100, align: "center", sortable: false },
            { name: 'Amount', index: 'Amount', width: 150, align: "center", sortable: false }
        ],
        multiselect: false,
        caption: "Product Details"
    });

    $(window).resize(function () {
        var outerwidthMRV = $('#gridProduct').width();
        $('#tblPrdDetails').setGridWidth(outerwidthMRV);
    });

    $("#CustomerSearchModel").on('show.bs.modal', function () {
        $("#tblCustomerSearch").jqGrid({
            url: '/Customer/GetAPCustomerDetailsList',
            datatype: "json",
            colNames: ['Customer Code', 'Customer Name'],
            colModel: [
            { key: true, name: 'ARCODE_ARM', index: 'ARCODE_ARM', width: 400 },
            { key: false, name: 'DESCRIPTION_ARM', index: 'DESCRIPTION_ARM', width: 400 }
            ],
            rowNum: 20,
            rowList: [20, 30, 40],
            mtype: 'GET',
            gridview: true,
            shrinkToFit: true,
            autowidth: true,
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
            colNames: ['Product Code', 'Product Details', 'Purchase Unit', 'Sales Unit'],
            colModel: [
            { key: true, name: 'PROD_CODE_PM', index: 'PROD_CODE_PM', width: 200 },
            { key: false, name: 'DESCRIPTION_PM', index: 'DESCRIPTION_PM', width: 250 },
            { key: false, name: 'PURCHSEUNIT_PM', index: 'PURCHSEUNIT_PM', width: 175 },
            { key: false, name: 'SALESUNIT_PM', index: 'SALESUNIT_PM', width: 175 }
            ],
            rowNum: 20,
            rowList: [20, 30, 40],
            mtype: 'GET',
            gridview: true,
            shrinkToFit: true,
            autowidth: true,
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
    }

    $("#btnCancel").click(function (e) {
        clearModalForm();
    });
    $("#btnSave").click(function (e) {
        if ($("#purchaseProductModelform").valid()) {
            e.preventDefault();
            var arrIndex = arrPrdDetails.length;
            arrPrdDetails[arrIndex] = {
                PrdCode: $("#txtPrCode").val(), PrdDesc: $("#txtPrDesc").val(),
                Qty: $("#txtQuantity").val(), Rate: $("#txtRate").val(), Unit: $("#txtUnit").val(),
                Amount: $("#txtAmount").val()
            };
            var su = jQuery("#tblPrdDetails").jqGrid('addRowData', arrIndex, arrPrdDetails[arrIndex]);
            if (su) {
                var mrvPrds = $('#tblPrdDetails').jqGrid('getGridParam', 'data');
                var jsonMrvPrds = JSON.stringify(mrvPrds);
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
        for (i = 0; i < arrPrdDetails.length; i++) {
            totalGridPrdAmount += parseFloat(arrPrdDetails[i]["Amount"]);
        }
        $("#txtNetAmount").val(totalGridPrdAmount);
        $("#txtTotalAmount").val(totalGridPrdAmount);
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
        $('#formPurchaseEntry').bootstrapValidator('revalidateField', 'ShipChrg');
    });

    $("#txtDiscount").on("change", function () {
        $('#formPurchaseEntry').bootstrapValidator('revalidateField', 'Discount');
    });

    $('#formPurchaseEntry').bootstrapValidator({
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
            },
            Cash: {
                validators: {
                    notEmpty: {
                        message: 'Cash Detail is required'
                    },
                    integer: {
                        message: 'Integer Only'
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
    });
});