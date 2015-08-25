$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liCashMemo").addClass("active");
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
    colNames: ['Job No', 'PR Code', 'S W Code', 'Description', 'Qty', 'Unit', 'Rate', 'Cash Amount', 'Discount', 'Ship. Chrg'],
    colModel: [
        { name: 'JobNo', index: 'JobNo', width: 80, align: "center", sortable: false },
        { name: 'PRCode', index: 'PRCode', width: 80, align: "center", sortable: false },
        { name: 'SWCode', index: 'SWCode', width: 80, align: "center", sortable: false },
        { name: 'Description', index: 'Description', width: 250, align: "left", sortable: false },
        { name: 'Qty', index: 'Qty', width: 80, align: "center", sortable: false },
        { name: 'Unit', index: 'Unit', width: 80, align: "center", sortable: false },
        { name: 'Rate', index: 'Rate', width: 80, align: "center", sortable: false },
        { name: 'CashAmount', index: 'CashAmount', width: 100, align: "center", sortable: false },
        { name: 'Discount', index: 'Discount', width: 100, align: "center", sortable: false },
        { name: 'ShipChrg', index: 'ShipChrg', width: 100, align: "center", sortable: false }

    ],
    multiselect: false,
    caption: "Sale Details"
});
$(window).resize(function () {
    var outerwidthMRV = $('#gridMRV').width();
    $('#tblMRVJobDetails').setGridWidth(outerwidthMRV);
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
        autowidth: true,
        multiselect: false
    });
});
$(window).resize(function () {
    var outerwidth = $('#MrvGrid').width();
    $('#tblMRVSearch').setGridWidth(outerwidth);
});

$("#txtBankCode").autocomplete({
    source: '/Cash/getBankCodes',
    minLength: 0
}).bind('focus', function () {
    $(this).autocomplete("search");
});

$("#txtBankDetails").autocomplete({
    source: '/Cash/getBankName',
    minLength: 0
}).bind('focus', function () {
    $(this).autocomplete("search");
});

$("#txtBankCode").on("blur", function (e) {
    $("#txtBankDetails").val("");
    getBankDetails();

});

$("#txtBankDetails").on("blur", function (e) {
    $("#txtBankCode").val("");
    getBankDetails();
});

function getBankDetails() {
    var _bankCode = $('#txtBankCode').val();
    var _bankName = $('#txtBankDetails').val();
    var data = JSON.stringify({ bankCode: _bankCode, bankName: _bankName });
    $.ajax({
        url: '/Cash/getBankDetails',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        type: "POST",
        success: function (bankDetails) {
            $('#txtBankCode').val(bankDetails.BANKCODE_BM).change();
            $('#txtBankDetails').val(bankDetails.BANKNAME_BM).change();
        },
        complete: function () {
        },
        error: function (bankDetails) {
        }
    });
}

var arrMrvJobDetails = [];
var arrMrvSaleDetails = [];
function getJobDetailByMRV() {
    var _MrvCode = $("#txtMRVNo").val();
    var data = JSON.stringify({ MRVID: _MrvCode });
    $.ajax({
        url: '/MRV/getJobDetailByMRV',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        type: "POST",
        success: function (jobDetails) {
            arrMrvJobDetails = [];
            var i = 0;
            for (i = 0; i < jobDetails.JobData.length; i++) {
                arrMrvJobDetails[i] = { JobNo: jobDetails.JobData[i].JobNo, PrdDesc: jobDetails.JobData[i].PrdCode, JobStatus: jobDetails.JobData[i].JobStatus };
                jQuery("#tblMRVJobDetails").jqGrid('addRowData', i + 1, arrMrvJobDetails[i]);
            }
        },
        complete: function () {
        },
        error: function (jobDetails) {
        }
    });
}

function getSaleDetailByMRV() {
    var _MrvCode = $("#txtMRVNo").val();
    var data = JSON.stringify({ MRVID: _MrvCode });
    $.ajax({
        url: '/MRV/getSaleDetailByMRV',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        type: "POST",
        success: function (saleDetails) {
            arrMrvSaleDetails = [];
            var sales = saleDetails.lstSales;
            var NetAmount = 0;
            var totalDiscount = 0;
            var totalShipChrg = 0;
            var i = 0;
            for (i = 0; i < sales.length; i++) {
                arrMrvSaleDetails[i] = {
                    JobNo: sales[i].JobNo, PRCode: sales[i].PRCode, SWCode: sales[i].SWCode,
                    Description: sales[i].Description, Qty: sales[i].Qty,
                    Unit: sales[i].Unit, Rate: sales[i].Rate, CashAmount: sales[i].CashAmount,
                    Discount: sales[i].Discount, ShipChrg: sales[i].ShipChrg
                };
                NetAmount = NetAmount + arrMrvSaleDetails[i]["CashAmount"];
                totalDiscount = totalDiscount + arrMrvSaleDetails[i]["Discount"];
                totalShipChrg = totalShipChrg + arrMrvSaleDetails[i]["ShipChrg"];
                $("#txtTotalCashAmount").val(NetAmount);
                $("#txtNetAmount").val(NetAmount);
                $("#txtBankAmount").val(NetAmount);
                $("#txtTotalDiscount").val(totalDiscount);
                $("#txtTotalShipCharges").val(totalShipChrg);
                jQuery("#tblSaleDetails").jqGrid('addRowData', i + 1, arrMrvSaleDetails[i]);
            }

        },
        complete: function () {
        },
        error: function (saleDetails) {
        }
    });
}
$("#BankSearchModel").on('show.bs.modal', function () {
    $("#tblBankSearch").jqGrid({
        url: '/Cash/GetBankDetailsList',
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
    } else {

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