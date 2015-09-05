﻿$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liCashMemo").addClass("active");
    $("#txtNetAmount").val("");
    $("#txtBankCode").val("");
    $("#txtBankNote").val("");
    $('#txtDate').datepicker();
    $('#txtGLDate').datepicker();
    jQuery("#tblMRVJobDetails").jqGrid({
        datatype: "local",
        height: 100,
        colNames: ['Job No', 'Product Description', 'Job Status'],
        colModel: [
            { name: 'JobNo', index: 'JobNo', width: 150, align: "center", sortable: false },
            { name: 'PrdDesc', index: 'PrdDesc', width: 350, align: "left", sortable: false },
            { name: 'JobStatus', index: 'JobStatus', width: 150, align: "center", sortable: false }
        ],
        multiselect: false,
        caption: "MRV Details"
    });
});
jQuery("#tblSaleDetails").jqGrid({
    datatype: "local",
    height: 100,
    colNames: ['Sale ID', 'Job No', 'PR Code', 'S W Code', 'Description', 'Qty', 'Unit', 'Rate', 'Cash Amount', 'Discount', 'Ship. Chrg'],
    colModel: [
        { name: 'SLNO_SD', index: 'SLNO_SD', width: 50, align: "center", sortable: false },
        { name: 'JOBNO_SD', index: 'JOBNO_SD', width: 80, align: "center", sortable: false },
        { name: 'PRCODE_SD', index: 'PRCODE_SD', width: 80, align: "center", sortable: false },
        { name: 'JOBID_SD', index: 'JOBID_SD', width: 80, align: "center", sortable: false },
        { name: 'Description', index: 'Description', width: 200, align: "left", sortable: false },
        { name: 'QTY_SD', index: 'QTY_SD', width: 80, align: "center", sortable: false },
        { name: 'UNIT_SD', index: 'UNIT_SD', width: 80, align: "center", sortable: false },
        { name: 'RATE_SD', index: 'RATE_SD', width: 80, align: "center", sortable: false },
        { name: 'CASHTOTAL_SD', index: 'CASHTOTAL_SD', width: 100, align: "center", sortable: false },
        { name: 'DISCOUNT_SD', index: 'DISCOUNT_SD', width: 100, align: "center", sortable: false },
        { name: 'SHIPPINGCHARGES_SD', index: 'SHIPPINGCHARGES_SD', width: 100, align: "center", sortable: false }
    ],
    multiselect: false,
    caption: "Sale Details"
});
$(window).resize(function () {
    var outerwidthMrv = $('#gridMRV').width();
    $('#tblMRVJobDetails').setGridWidth(outerwidthMrv);
    var outerwidthSale = $('#gridSale').width();
    $('#tblSaleDetails').setGridWidth(outerwidthSale);
});
/***** Start - Adding JQGRID Code For Searching Job Number and MRV Number****/
$("#mrvSearchModel").on('show.bs.modal', function () {
    $("#tblMRVSearch").jqGrid({
        url: '/MRV/getMRVList',
        datatype: "json",
        colNames: ['MRV No', 'Customer', 'Customer Details'],
        colModel: [
        { key: true, name: 'MRVNO_MRV', index: 'MRVNO_MRV', width: 200 },
        { key: false, name: 'CUSTOMERCODE_MRV', index: 'CUSTOMERCODE_MRV', width: 200 },
        { key: false, name: 'CUSTOMERNAME_MRV', index: 'CUSTOMERNAME_MRV', width: 400 }
        ],
        rowNum: 20,
        rowList: [20, 30, 40],
        mtype: 'GET',
        gridview: true,
        shrinkToFit: true,
        autowidth: true,
        viewrecords: true,
        sortorder: "desc",
        pager: jQuery('#Pager'),
        caption: "MRV No List",
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
    $(window).resize(function () {
        var outerwidth = $('#MrvGrid').width();
        $('#tblMRVSearch').setGridWidth(outerwidth);
    });
});
var arrMrvJobDetails = [];
var arrMrvSaleDetails = [];
function getJobDetailByMRV() {
    var mrvCode = $("#txtMRVNo").val();
    var data = JSON.stringify({ MRVID: mrvCode });
    $.ajax({
        url: '/MRV/getJobDetailByMRV',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        type: "POST",
        success: function (jobDetails) {
            arrMrvJobDetails = [];
            for (var i = 0; i < jobDetails.JobData.length; i++) {
                arrMrvJobDetails[i] = { JobNo: jobDetails.JobData[i].JobNo, PrdDesc: jobDetails.JobData[i].PrdCode, JobStatus: jobDetails.JobData[i].JobStatus };
                jQuery("#tblMRVJobDetails").jqGrid('addRowData', i + 1, arrMrvJobDetails[i]);
            }
        },
        complete: function () {
        },
        error: function () {
        }
    });
}

function getSaleDetailByMRV() {
    var mrvCode = $("#txtMRVNo").val();
    var data = JSON.stringify({ MRVID: mrvCode });
    $.ajax({
        url: '/MRV/getSaleDetailByMRV',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        type: "POST",
        success: function (saleDetails) {
            arrMrvSaleDetails = [];
            var sales = saleDetails.lstSales;
            var netAmount = 0;
            var totalDiscount = 0;
            var totalShipChrg = 0;
            for (var i = 0; i < sales.length; i++) {
                arrMrvSaleDetails[i] = {
                    SLNO_SD: sales[i].SaleNo, JOBNO_SD: sales[i].JobNo, PRCODE_SD: sales[i].PrCode, JOBID_SD: sales[i].SwCode,
                    Description: sales[i].Description, QTY_SD: sales[i].Qty,
                    UNIT_SD: sales[i].Unit, RATE_SD: sales[i].Rate, CASHTOTAL_SD: sales[i].CashAmount,
                    DISCOUNT_SD: sales[i].Discount, SHIPPINGCHARGES_SD: sales[i].ShipChrg
                };
                netAmount = netAmount + arrMrvSaleDetails[i]["CASHTOTAL_SD"];
                totalDiscount = totalDiscount + arrMrvSaleDetails[i]["DISCOUNT_SD"];
                totalShipChrg = totalShipChrg + arrMrvSaleDetails[i]["SHIPPINGCHARGES_SD"];
                $("#txtTotalCashAmount").val(netAmount);
                $("#txtNetAmount").val(netAmount);
                $("#txtBankAmount").val(netAmount);
                $("#txtTotalDiscount").val(totalDiscount);
                $("#txtTotalShipCharges").val(totalShipChrg);
                jQuery("#tblSaleDetails").jqGrid('addRowData', i + 1, arrMrvSaleDetails[i]);
            }
            var jsonMrvPrds = JSON.stringify(arrMrvSaleDetails);
            $('#hdnSaleDetails').val(jsonMrvPrds);
        },
        complete: function () {
        },
        error: function () {
        }
    });
}
$("#BankSearchModel").on('show.bs.modal', function () {
    $("#tblBankSearch").jqGrid({
        url: '/Bank/GetBankDetailsList',
        datatype: "json",
        colNames: ['Bank Code', 'Bank Name'],
        colModel: [
        { key: true, name: 'BANKCODE_BM', index: 'BANKCODE_BM', width: 400 },
        { key: false, name: 'BANKNAME_BM', index: 'BANKNAME_BM', width: 400 }],
        rowNum: 20,
        rowList: [20, 30, 40],
        mtype: 'GET',
        gridview: true,
        shrinkToFit: true,
        autowidth: true,
        viewrecords: true,
        sortorder: "asc",
        pager: jQuery('#bankPager'),
        caption: "Bank Details List",
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
    var outerwidth = $('#bankGrid').width();
    $('#tblBankSearch').setGridWidth(outerwidth);
});

$("#btnBankSelect").on("click", function (e) {
    var id = jQuery("#tblBankSearch").jqGrid('getGridParam', 'selrow');
    if (id) {
        var ret = jQuery("#tblBankSearch").jqGrid('getRowData', id);
        $("#txtBankCode").val(ret.BANKCODE_BM).change();
        $("#txtBankDetails").val(ret.BANKNAME_BM).change();
        $('#BankSearchModel').modal('toggle');
    }
    e.preventDefault();
});
$("#btnMRVSelect").click(function (e) {
    var id = jQuery("#tblMRVSearch").jqGrid('getGridParam', 'selrow');
    if (id) {
        var ret = jQuery("#tblMRVSearch").jqGrid('getRowData', id);
        $("#txtMRVNo").val(ret.MRVNO_MRV);
        $("#txtCustCode").val(ret.CUSTOMERCODE_MRV);
        $("#txtCustDetail").val(ret.CUSTOMERNAME_MRV);
        getJobDetailByMRV();
        getSaleDetailByMRV();
        $('#mrvSearchModel').modal('toggle');
    }
    e.preventDefault();
});
/***** End - Adding JQGRID Code For Searching Job Number and MRV Number****/
$('#formCashMemo').bootstrapValidator({
    container: '#messages',
    feedbackIcons: {
        valid: 'glyphicon glyphicon-ok',
        invalid: 'glyphicon glyphicon-remove',
        validating: 'glyphicon glyphicon-refresh'
    },
    fields: {
        MRVNo: {
            validators: {
                notEmpty: {
                    message: 'MRV Number is required'
                }
            }
        }
    }
});