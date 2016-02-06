var jobMRVSelect = function (jobId) {
    if (jobId) {
        var ret = jQuery("#tblJobSearch").jqGrid('getRowData', jobId);
        $("#txtJobID").val(ret.JOBNO_JM);
        $("#txtMRVNo").val(ret.MRVNO_JM);
        $('#mrvJobSearchModel').modal('toggle');
    }
};

var employeeSelect = function (empId) {
    if (empId) {
        var ret = jQuery("#tblEmployeeSearch").jqGrid('getRowData', empId);
        $("#txtEmpCode").val(ret.EMPCODE_EM).change();
        $("#txtEmpName").val(ret.EMPFNAME_EM).change();
        $('#EmployeeSearchModel').modal('toggle');
    }
}

var productSelect = function (prdId) {
    if (prdId) {
        var ret = jQuery("#tblProductSearch").jqGrid('getRowData', prdId);
        $("#txtPrCode").val(ret.PROD_CODE_PM).change();
        $("#txtPrDetail").val(ret.DESCRIPTION_PM).change();
        $('#PrdSearchModel').modal('toggle');
    }
};

var jobSelect = function(jobId) {
    if (jobId) {
        var ret = jQuery("#tblJobModalSearch").jqGrid('getRowData', jobId);
        $("#txtSowID").val(ret.JOBID_JR).change();
        $("#txtSowDetail").val(ret.JOBDESCRIPTION_JR).change();
        $("#txtPrRate").val(ret.RATE_RJ).change();
        $('#JobSearchModel').modal('toggle');
    }
}

var customerSelect = function(custId) {
    if (custId) {
        var ret = jQuery("#tblCustomerSearch").jqGrid('getRowData', custId);
        $("#txtCreditCustCode").val(ret.ARCODE_ARM).change();
        $("#txtCreditCustName").val(ret.DESCRIPTION_ARM).change();
        $('#CustomerSearchModel').modal('toggle');
    }
}

$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liSalesEntry").addClass("active");
    $("#txtSaleDate").datepicker();
    $('#ddlSaleType').on("change", function () {
        if ($('#ddlSaleType').val() === "Product") {
            $("#divSowCode").hide();
            $("#divSowDetail").hide();
            $("#txtSowID").val("");
            $("#txtSowDetail").val("");
            $("#divPrCode").show();
            $("#divPrDetail").show();
        }
        else if ($('#ddlSaleType').val() === "SOW") {
            $("#divPrCode").hide();
            $("#divPrDetail").hide();
            $("#txtPrCode").val("");
            $("#txtPrDetail").val("");
            $("#divSowCode").show();
            $("#divSowDetail").show();
        }
    });
    $('#ddlPayMode').on("change", function () {
        var totalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount;
        $("#txtPrAmount").val(totalAmount);
        if ($('#ddlPayMode').val() === "Cash") {
            $finalAmount = $("#txtShipCharge").val() * 1 + totalAmount - $("#txtDiscount").val() * 1;
            $("#txtCreditAmount").val("0");
            $("#txtCreditCustCode").val("");
            $("#txtCreditCustName").val("");
            $("#divCreditAmount").hide();
            $("#divCreditCustDetails").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() === "Credit") {
            $finalAmount = $("#txtShipCharge").val() * 1 + totalAmount - $("#txtDiscount").val() * 1;
            $("#txtCashAmount").val("0");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
            $("#divCreditCustDetails").show();
        }
    });
    $("#txtQty").on("blur", function () {
        if ($("#txtQty").val() === "") {
            $("#txtQty").val("1");
        }
        var totalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount;
        $("#txtPrAmount").val(totalAmount);
        if ($('#ddlPayMode').val() === "Cash") {
            $finalAmount = $("#txtShipCharge").val() - $("#txtDiscount").val() + totalAmount;
            $("#txtCreditAmount").val("0");
            $("#divCreditAmount").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() === "Credit") {
            $finalAmount = $("#txtShipCharge").val() - $("#txtDiscount").val() + totalAmount;
            $("#txtCashAmount").val("0");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
        }
    });
    $("#txtPrRate").on("blur", function () {
        if ($("#txtPrRate").val() === "") {
            $("#txtPrRate").val(0);
        }
        var totalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount;
        $("#txtPrAmount").val(totalAmount);
        if ($('#ddlPayMode').val() === "Cash") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + totalAmount;
            $("#txtCreditAmount").val("0");
            $("#divCreditAmount").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() === "Credit") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + totalAmount;
            $("#txtCashAmount").val("0");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
        }
    });
    $("#txtDiscount").on("blur", function () {
        if ($("#txtDiscount").val() === "") {
            $("#txtDiscount").val("0");
        }
        var totalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount;
        $("#txtPrAmount").val(totalAmount);
        if ($('#ddlPayMode').val() === "Cash") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + totalAmount;
            $("#divCreditAmount").val("0");
            $("#divCreditAmount").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() === "Credit") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + totalAmount;
            $("#divCashAmount").val("0");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
        }
    });
    $("#txtShipCharge").on("blur", function () {
        if ($("#txtShipCharge").val() === "") {
            $("#txtShipCharge").val("0");
        }
        var totalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount;
        $("#txtPrAmount").val(totalAmount);
        if ($('#ddlPayMode').val() === "Cash") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + totalAmount;
            $("#divCreditAmount").val("0");
            $("#divCreditAmount").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() === "Credit") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + totalAmount;
            $("#divCashAmount").val("0");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
        }
    });
    var $mrvJobStatus = "N";
    /***** Start - Adding JQGRID Code For Searching Job Number and MRV Number****/
    $("#mrvJobSearchModel").on('show.bs.modal', function () {
        $("#tblJobSearch").jqGrid({
            url: '/Job/GetJobMrvList?jobStatus=' + $mrvJobStatus,
            datatype: "json",
            height: 150,
            shrinkToFit: true,
            autoheight: true,
            autowidth: true,
            styleUI: "Bootstrap",
            colNames: ['Job No', 'MRV No',''],
            colModel: [
            { key: true, name: 'JOBNO_JM', index: 'JOBNO_JM', width: 400 },
            { key: false, name: 'MRVNO_JM', index: 'MRVNO_JM', width: 400 },
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
                        "%Href%": "href=javascript:jobMRVSelect(&apos;" + rowObject.JOBNO_JM + "&apos;);"
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
            caption: "Job No & MRV No List",
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
        }
        e.preventDefault();
    });

    $("#mrvJobSearchModel").on('hide.bs.modal', function () {
        var jobCode = $("#txtJobID").val();
        var mrvNo = $("#txtMRVNo").val();
        if (jobCode !== "" && mrvNo !== "") {
            var data = JSON.stringify({ jobId: jobCode, mrvNo: mrvNo });
            $.ajax({
                url: '/Job/GetJobMrvData',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: data,
                type: "POST",
                success: function (jobMrvDetails) {
                    $("#txtCustCode").val(jobMrvDetails.custCode);
                    $("#txtCustName").val(jobMrvDetails.custName);
                    $("#txtMRVProdCode").val(jobMrvDetails.prdCode);
                    $("#txtMRVProdDetail").val(jobMrvDetails.prdDetail);
                },
                complete: function () {
                },
                error: function () {
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
            height: 150,
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Product Code', 'Product Details',''],
            colModel: [
            { key: true, name: 'PROD_CODE_PM', index: 'PROD_CODE_PM', width: 400 },
            { key: false, name: 'DESCRIPTION_PM', index: 'DESCRIPTION_PM', width: 400 },
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
            sortorder: "desc",
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
            height: 150,
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Product Code', 'Product Details', 'Rate',''],
            colModel: [
            { key: true, name: 'JOBID_JR', index: 'JOBID_JR', width: 250 },
            { key: false, name: 'JOBDESCRIPTION_JR', index: 'JOBDESCRIPTION_JR', width: 400 },
            { key: false, name: 'RATE_RJ', index: 'RATE_RJ', width: 150 },
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
                        "%Href%": "href=javascript:jobSelect(&apos;" + rowObject.JOBID_JR + "&apos;);"
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
            height: 150,
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Customer Code', 'Customer Name',''],
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

    $("#EmployeeSearchModel").on('show.bs.modal', function () {
        $("#tblEmployeeSearch").jqGrid({
            url: '/EmployeeMaster/GetEmployeeDetailsList',
            datatype: "json",
            height: 150,
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Employee Code', 'Employee Name',''],
            colModel: [
            { key: true, name: 'EMPCODE_EM', index: 'EMPCODE_EM', width: 400 },
            { key: false, name: 'EMPFNAME_EM', index: 'EMPFNAME_EM', width: 400 },
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
                        "%Href%": "href=javascript:employeeSelect(&apos;" + rowObject.EMPCODE_EM + "&apos;);"
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

    $('#formJobEntry').formValidation({
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
    }).on('success.form.fv', function (e) {
        debugger;
        // Prevent form submission
        e.preventDefault();
    });
});