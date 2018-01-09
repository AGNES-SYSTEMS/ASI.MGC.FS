var mrvSelect = function (mrvNo) {
    if (mrvNo) {
        var ret = jQuery("#tblMRVSearch").jqGrid('getRowData', mrvNo);
        $("#txtMRVNo").val(ret.MRVNO_MRV);
        $("#txtCustCode").val(ret.CUSTOMERCODE_MRV);
        $("#txtCustDetail").val(ret.CUSTOMERNAME_MRV);
        getJobDetailByMrv();
        getSaleDetailByMrv();
        $('#mrvSearchModel').modal('toggle');
    }
};
function getJobDetailByMrv() {
    var mrvCode = $("#txtMRVNo").val();
    var data = JSON.stringify({ MRVID: mrvCode });
    $.ajax({
        url: '/MRV/getJobDetailByMRV',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        type: "POST",
        success: function (jobDetails) {
            var arrMrvJobDetails = [];
            jQuery('#tblMRVJobDetails').jqGrid('clearGridData');
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
};
function getSaleDetailByMrv() {
    var mrvCode = $("#txtMRVNo").val();
    var data = JSON.stringify({ MRVID: mrvCode, statusId: "N" });
    $.ajax({
        url: '/MRV/GetCreditSaleDetailByMrv',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: data,
        type: "POST",
        success: function (saleDetails) {
            var arrMrvSaleDetails = [];
            jQuery('#tblSaleDetails').jqGrid('clearGridData');
            var sales = saleDetails.lstSales;
            var netAmount = 0;
            var totalDiscount = 0;
            var totalShipChrg = 0;
            var totalVat = 0;
            for (var i = 0; i < sales.length; i++) {
                arrMrvSaleDetails[i] = {
                    SLNO_SD: sales[i].SaleNo, JOBNO_SD: sales[i].JobNo, PRCODE_SD: sales[i].PrCode,
                    JOBID_SD: sales[i].SwCode, Description: sales[i].Description, QTY_SD: sales[i].Qty,
                    UNIT_SD: sales[i].Unit, RATE_SD: sales[i].Rate, CREDITTOTAL_SD: sales[i].CashAmount,
                    DISCOUNT_SD: sales[i].Discount, SHIPPINGCHARGES_SD: sales[i].ShipChrg,
                    VAT_SD: sales[i].ValueAddedTax
                };
                netAmount = netAmount + arrMrvSaleDetails[i]["CREDITTOTAL_SD"];
                totalDiscount = totalDiscount + arrMrvSaleDetails[i]["DISCOUNT_SD"];
                totalShipChrg = totalShipChrg + arrMrvSaleDetails[i]["SHIPPINGCHARGES_SD"];
                totalVat = totalVat + arrMrvSaleDetails[i]["VAT_SD"];
                $("#txtTotalCreditAmount").val(netAmount.toFixed(2));
                $("#txtNetAmount").val(netAmount.toFixed(2));
                $("#txtTotalDiscount").val(totalDiscount.toFixed(2));
                $("#txtTotalShipCharges").val(totalShipChrg.toFixed(2));
                $("#txtTotalVAT").val(totalVat.toFixed(2));
                jQuery("#tblSaleDetails").jqGrid('addRowData', i + 1, arrMrvSaleDetails[i]);
            }
            var jsonMrvPrds = JSON.stringify(arrMrvSaleDetails);
            $('#hdnSaleDetail').val(jsonMrvPrds);
        },
        complete: function () {
            evaluateFields();
        },
        error: function () {
        }
    });
};
var evaluateFields = function () {
    $("#formInvoicePrep").formValidation('revalidateField', 'NARRATION_ART');
    $("#formInvoicePrep").formValidation('revalidateField', 'ARAPCODE_ART');
    $("#formInvoicePrep").formValidation('revalidateField', 'CustDetail');
    $("#formInvoicePrep").formValidation('revalidateField', 'DEBITAMOUNT_ART');
    $("#formInvoicePrep").formValidation('revalidateField', 'TotalCreditAmount');
    $("#formInvoicePrep").formValidation('revalidateField', 'TotalDiscount');
    $("#formInvoicePrep").formValidation('revalidateField', 'TotalShipCharges');
    $("#formInvoicePrep").formValidation('revalidateField', 'TotalVAT');
};
$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liInvoicePrep").addClass("active");
    $('#txtGLDate').datepicker();
    jQuery("#tblMRVJobDetails").jqGrid({
        datatype: "local",
        height: 100,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: ['Job No', 'Job Description', 'Job Status'],
        colModel: [
            { name: 'JobNo', index: 'JobNo', width: 150, align: "center", sortable: false },
            { name: 'PrdDesc', index: 'PrdDesc', width: 350, align: "left", sortable: false },
            { name: 'JobStatus', index: 'JobStatus', width: 150, align: "center", sortable: false }
        ],
        multiselect: false,
        caption: "MRV Details"
    });
    jQuery("#tblSaleDetails").jqGrid({
        datatype: "local",
        height: 100,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: ['Sale id', 'Job No', 'PR Code', 'S W Code', 'Description', 'Qty', 'Unit', 'Rate', 'Credit Amount', 'Discount', 'Ship. Chrg', 'VAT 5%'],
        colModel: [
            { name: 'SLNO_SD', index: 'SLNO_SD', width: 50, align: "center", sortable: false },
            { name: 'JOBNO_SD', index: 'JOBNO_SD', width: 80, align: "center", sortable: false },
            { name: 'PRCODE_SD', index: 'PRCODE_SD', width: 80, align: "center", sortable: false },
            { name: 'JOBID_SD', index: 'JOBID_SD', width: 80, align: "center", sortable: false },
            { name: 'Description', index: 'Description', width: 300, align: "left", sortable: false },
            { name: 'QTY_SD', index: 'QTY_SD', width: 80, align: "center", sortable: false },
            { name: 'UNIT_SD', index: 'UNIT_SD', width: 80, align: "center", sortable: false },
            { name: 'RATE_SD', index: 'RATE_SD', width: 80, align: "center", sortable: false },
            { name: 'CREDITTOTAL_SD', index: 'CREDITTOTAL_SD', width: 100, align: "center", sortable: false },
            { name: 'DISCOUNT_SD', index: 'DISCOUNT_SD', width: 100, align: "center", sortable: false },
            { name: 'SHIPPINGCHARGES_SD', index: 'SHIPPINGCHARGES_SD', width: 100, align: "center", sortable: false },
            { name: 'VAT_SD', index: 'VAT_SD', width: 100, align: "center", sortable: false }
        ],
        multiselect: false,
        caption: "Sale Details"
    });
    $("#btnNew").on("click", function () {
        location.reload(true);
    });
    $(window).resize(function () {
        var outerwidthMrv = $('#gridMRV').width();
        $('#tblMRVDetails').setGridWidth(outerwidthMrv);
        var outerwidthSale = $('#gridSale').width();
        $('#tblSaleDetails').setGridWidth(outerwidthSale);
    });
    /***** Start - Adding JQGRID Code For Searching Job Number and MRV Number****/
    $("#mrvSearchModel").on('show.bs.modal', function () {
        $("#tblMRVSearch").jqGrid({
            url: '/Invoice/GetInvoicePrepMrvList',
            datatype: "json",
            height: 150,
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: [
                'MRV No', 'Job No', 'Customer', 'Customer Details', ''],
            colModel: [
            { key: true, name: 'MRVNO_MRV', index: 'MRVNO_MRV', width: 150 },
            { key: false, name: 'JOBNO_SD', index: 'JOBNO_SD', width: 200 },
            { key: false, name: 'CUSTOMERCODE_MRV', index: 'CUSTOMERCODE_MRV', width: 150 },
            { key: false, name: 'CUSTOMERNAME_MRV', index: 'CUSTOMERNAME_MRV', width: 300 },
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
                        "%Href%": "href=javascript:mrvSelect(&apos;" + rowObject.MRVNO_MRV + "&apos;);"
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
    var searchGrid = function (mrvNo, jobNo, custName) {
        var postData = $("#tblMRVSearch").jqGrid("getGridParam", "postData");
        postData["mrvCode"] = mrvNo;
        postData["jobNo"] = jobNo;
        postData["custName"] = custName;
        $("#tblMRVSearch").setGridParam({ postData: postData });
        $("#tblMRVSearch").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtJobNoSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtJobNoSearch").val().length >= 1 || $("#txtJobNoSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtMrvSearch").val(), $("#txtJobNoSearch").val(), $("#txtCustNameSearch").val());
        }
    });
    $("#txtCustNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtCustNameSearch").val().length >= 3 || $("#txtCustNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtMrvSearch").val(), $("#txtJobNoSearch").val(), $("#txtCustNameSearch").val());
        }
    });
    $("#txtMrvSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtMrvSearch").val().length >= 3 || $("#txtMrvSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtMrvSearch").val(), $("#txtJobNoSearch").val(), $("#txtCustNameSearch").val());
        }
    });
    $("#btnMRVSelect").click(function (e) {
        var id = jQuery("#tblMRVSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblMRVSearch").jqGrid('getRowData', id);
            $("#txtMRVNo").val(ret.MRVNO_MRV);
            $("#txtCustCode").val(ret.CUSTOMERCODE_MRV);
            $("#txtCustDetail").val(ret.CUSTOMERNAME_MRV);
            getJobDetailByMrv();
            getSaleDetailByMrv();
            $('#mrvSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    /***** End - Adding JQGRID Code For Searching Job Number and MRV Number****/
    $('#formInvoicePrep').on('init.field.fv', function (e, data) {
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
            MRVNo: {
                validators: {
                    notEmpty: {
                        message: 'MRV Number is required'
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
    }).off('success.form.fv').on('success.form.fv', function (e) {
        debugger;
        // Prevent form submission
        e.preventDefault();
    });
});