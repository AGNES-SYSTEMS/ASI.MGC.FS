$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liSalesEntry").addClass("active");
    $("#txtSaleDate").datepicker();
    $('#ddlSaleType').on("change", function (e) {
        if ($('#ddlSaleType').val() == "Product") {
            $("#divSowCode").hide();
            $("#divSowDetail").hide();
            $("#divPrCode").show();
            $("#divPrDetail").show();
        }
        else if ($('#ddlSaleType').val() == "SOW") {
            $("#divPrCode").hide();
            $("#divPrDetail").hide();
            $("#divSowCode").show();
            $("#divSowDetail").show();
        }
    });
    $('#ddlPayMode').on("change", function (e) {
        var TotalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount = null;
        $("#txtPrAmount").val(TotalAmount);
        if ($('#ddlPayMode').val() == "Cash") {
            $finalAmount = $("#txtShipCharge").val() * 1 + TotalAmount - $("#txtDiscount").val() * 1;
            $("#txtCreditAmount").val("0");
            $("#txtCreditCustCode").val("");
            $("#txtCreditCustName").val("");
            $("#divCreditAmount").hide();
            $("#divCreditCustDetails").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() == "Credit") {
            $finalAmount = $("#txtShipCharge").val() * 1 + TotalAmount - $("#txtDiscount").val() * 1;
            $("#txtCashAmount").val("0");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
            $("#divCreditCustDetails").show();
        }
    });
    $("#txtQty").on("blur", function () {
        if ($("#txtQty").val() == "") {
            $("#txtQty").val("1");
        }
        var TotalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount = null;
        $("#txtPrAmount").val(TotalAmount);
        if ($('#ddlPayMode').val() == "Cash") {
            $finalAmount = $("#txtShipCharge").val() - $("#txtDiscount").val() + TotalAmount;
            $("#txtCreditAmount").val("0");
            $("#divCreditAmount").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() == "Credit") {
            $finalAmount = $("#txtShipCharge").val() - $("#txtDiscount").val() + TotalAmount;
            $("#txtCashAmount").val("0");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
        }
    });
    $("#txtPrRate").on("blur", function () {
        if ($("#txtPrRate").val() == "") {
            $("#txtPrRate").val(0);
        }
        var TotalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount = null;
        $("#txtPrAmount").val(TotalAmount);
        if ($('#ddlPayMode').val() == "Cash") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + TotalAmount;
            $("#txtCreditAmount").val("0");
            $("#divCreditAmount").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() == "Credit") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + TotalAmount;
            $("#txtCashAmount").val("0");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
        }
    });
    $("#txtDiscount").on("blur", function () {
        if ($("#txtDiscount").val() == "") {
            $("#txtDiscount").val("0");
        }
        var TotalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount = null;
        $("#txtPrAmount").val(TotalAmount);
        if ($('#ddlPayMode').val() == "Cash") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + TotalAmount;
            $("#divCreditAmount").val("0");
            $("#divCreditAmount").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() == "Credit") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + TotalAmount;
            $("#divCashAmount").val("0");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
        }
    });
    $("#txtShipCharge").on("blur", function () {
        if ($("#txtShipCharge").val() == "") {
            $("#txtShipCharge").val("0");
        }
        var TotalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount = null;
        $("#txtPrAmount").val(TotalAmount);
        if ($('#ddlPayMode').val() == "Cash") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + TotalAmount;
            $("#divCreditAmount").val("0");
            $("#divCreditAmount").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() == "Credit") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + TotalAmount;
            $("#divCashAmount").val("0");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
        }
    });

    /***** Start - Adding JQGRID Code For Searching Job Number and MRV Number****/
    $("#mrvJobSearchModel").on('show.bs.modal', function () {
        $("#tblJobSearch").jqGrid({
            url: '/Job/getJobMRVList',
            datatype: "json",
            colNames: ['Job No', 'MRV No'],
            colModel: [
            { key: true, name: 'JOBNO_JM', index: 'JOBNO_JM', width: 400 },
            { key: false, name: 'MRVNO_JM', index: 'MRVNO_JM', width: 400 }
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
            caption: "Job No & MRV No List",
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
        var outerwidth = $('#grid').width();
        $('#tblJobSearch').setGridWidth(outerwidth);
    });

    $("#btnJobMRVSearch").on("click", function (e) {
        var id = jQuery("#tblJobSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblJobSearch").jqGrid('getRowData', id);
            $("#txtJobID").val(ret.JOBNO_JM);
            $("#txtMRVNo").val(ret.MRVNO_JM);
            $('#mrvJobSearchModel').modal('toggle');
        } else {

        }
        e.preventDefault();
    });

    $("#mrvJobSearchModel").on('hide.bs.modal', function () {
        var _jobCode = $("#txtJobID").val();
        var _MRVNo = $("#txtMRVNo").val();
        if (_jobCode != "" && _MRVNo != "") {
            var data = JSON.stringify({ JobID: _jobCode, MRVNo: _MRVNo });
            $.ajax({
                url: '/Job/GetJobMRVData',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: data,
                type: "POST",
                success: function (jobMRVDetails) {
                    $("#txtCustCode").val(jobMRVDetails.custCode);
                    $("#txtCustName").val(jobMRVDetails.custName);
                    $("#txtMRVProdCode").val(jobMRVDetails.prdCode);
                    $("#txtMRVProdDetail").val(jobMRVDetails.prdDetail);
                },
                complete: function () {
                },
                error: function (jobMRVDetails) {
                }
            });
        }
    });
    
    $("#txtUnit").autocomplete({
        source: '/Product/getUnitlist',
        minLength: 0,
        change: function () {
            $('#formJobEntry').bootstrapValidator('revalidateField', 'Unit');
        }
    }).bind('focus', function () {
        $(this).autocomplete("search");
    });

    $('#txtEmpCode').on('change', function () {
        $('#formJobEntry').bootstrapValidator('revalidateField', 'EmpCode');
    });

    $('#txtEmpName').on('change', function () {
        $('#formJobEntry').bootstrapValidator('revalidateField', 'EmpName');
    });
    
    $("#PrdSearchModel").on('show.bs.modal', function () {
        $("#tblProductSearch").jqGrid({
            url: '/Product/GetProductDetailsList',
            datatype: "json",
            colNames: ['Product Code', 'Product Details'],
            colModel: [
            { key: true, name: 'PROD_CODE_PM', index: 'PROD_CODE_PM', width: 400 },
            { key: false, name: 'DESCRIPTION_PM', index: 'DESCRIPTION_PM', width: 400 }
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

    $("#JobSearchModel").on('show.bs.modal', function () {
        $("#tblJobModalSearch").jqGrid({
            url: '/Job/GetJobDetailsList',
            datatype: "json",
            colNames: ['Product Code', 'Product Details', 'Rate'],
            colModel: [
            { key: true, name: 'JOBID_JR', index: 'JOBID_JR', width: 250 },
            { key: false, name: 'JOBDESCRIPTION_JR', index: 'JOBDESCRIPTION_JR', width: 400 },
            { key: false, name: 'RATE_RJ', index: 'RATE_RJ', width: 150 }
            ],
            rowNum: 20,
            rowList: [20, 30, 40],
            mtype: 'GET',
            gridview: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortorder: "asc",
            pager: jQuery('#jobPager'),
            caption: "Job Details List",
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
        var outerwidth = $('#jobGrid').width();
        $('#tblJobSearch').setGridWidth(outerwidth);
    });

    $("#btnProductSelect").on("click", function (e) {
        var id = jQuery("#tblProductSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblProductSearch").jqGrid('getRowData', id);
            $("#txtPrCode").val(ret.PROD_CODE_PM).change();
            $("#txtPrDetail").val(ret.DESCRIPTION_PM).change();
            $('#PrdSearchModel').modal('toggle');
        }
        e.preventDefault();
    });

    $("#btnJobSelect").on("click", function (e) {
        var id = jQuery("#tblJobModalSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblJobModalSearch").jqGrid('getRowData', id);
            $("#txtSowID").val(ret.JOBID_JR).change();
            $("#txtSowDetail").val(ret.JOBDESCRIPTION_JR).change();
            $("#txtPrRate").val(ret.RATE_RJ).change();
            $('#JobSearchModel').modal('toggle');
        }
        e.preventDefault();
    });

    $("#CustomerSearchModel").on('show.bs.modal', function () {
        $("#tblCustomerSearch").jqGrid({
            url: '/Customer/GetCustomerDetailsList',
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

    $("#EmployeeSearchModel").on('show.bs.modal', function () {
        $("#tblEmployeeSearch").jqGrid({
            url: '/EmployeeMaster/GetEmployeeDetailsList',
            datatype: "json",
            colNames: ['Employee Code', 'Employee Name'],
            colModel: [
            { key: true, name: 'EMPCODE_EM', index: 'EMPCODE_EM', width: 400 },
            { key: false, name: 'EMPFNAME_EM', index: 'EMPFNAME_EM', width: 400 }],
            rowNum: 20,
            rowList: [20, 30, 40],
            mtype: 'GET',
            gridview: true,
            shrinkToFit: true,
            autowidth: true,
            viewrecords: true,
            sortorder: "asc",
            pager: jQuery('#empPager'),
            caption: "Employee Details List",
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
        var outerwidth = $('#empGrid').width();
        $('#tblEmployeeSearch').setGridWidth(outerwidth);
    });

    $("#btnCustSelect").on("click", function (e) {
        var id = jQuery("#tblCustomerSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblCustomerSearch").jqGrid('getRowData', id);
            $("#txtCreditCustCode").val(ret.ARCODE_ARM).change();
            $("#txtCreditCustName").val(ret.DESCRIPTION_ARM).change();
            $('#CustomerSearchModel').modal('toggle');
        }
        e.preventDefault();
    });

    $("#btnEmpSelect").on("click", function (e) {
        var id = jQuery("#tblEmployeeSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblEmployeeSearch").jqGrid('getRowData', id);
            $("#txtEmpCode").val(ret.EMPCODE_EM).change();
            $("#txtEmpName").val(ret.EMPFNAME_EM).change();
            $('#EmployeeSearchModel').modal('toggle');
        }
        e.preventDefault();
    });

    $('#formJobEntry').bootstrapValidator({
        container: '#messages',
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        fields: {
            MRVNO_SD: {
                validators: {
                    notEmpty: {
                        message: 'MRV Number is required'
                    }
                }
            },
            EmpCode: {
                validators: {
                    notEmpty: {
                        message: 'Employee Code is required'
                    }
                }
            },
            EmpName: {
                validators: {
                    notEmpty: {
                        message: 'Employee Name is required'
                    }
                }
            },
            SALEDATE_SD: {
                validators: {
                    notEmpty: {
                        message: 'Sale Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            },
            QTY_SD: {
                validators: {
                    notEmpty: {
                        message: 'Quantity is required'
                    },
                    integer: {
                        message: 'Integer Only'
                    }
                }
            },
            Unit: {
                validators: {
                    notEmpty: {
                        message: 'Unit is required'
                    }
                }
            },
            RATE_SD: {
                validators: {
                    notEmpty: {
                        message: 'Rate is required'
                    },
                    integer: {
                        message: 'Integer Only'
                    }
                }
            },
            DISCOUNT_SD: {
                validators: {
                    notEmpty: {
                        message: 'Discount is required'
                    },
                    integer: {
                        message: 'Integer Only'
                    }
                }
            },
            SHIPPINGCHARGES_SD: {
                validators: {
                    notEmpty: {
                        message: 'Ship Charges is required'
                    },
                    integer: {
                        message: 'Integer Only'
                    }
                }
            }
        }
    });
});