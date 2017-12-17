var jobMRVSelect = function (jobId) {
    if (jobId) {
        var ret = jQuery("#tblJobSearch").jqGrid('getRowData', jobId);
        $("#txtJobid").val(ret.JOBNO_JM);
        $("#txtMRVNo").val(ret.MRVNO_JM);
        $('#mrvJobSearchModel').modal('toggle');
    }
};
var $custCode = "";
var $custName = "";
var employeeSelect = function (empId) {
    if (empId) {
        var ret = jQuery("#tblEmployeeSearch").jqGrid('getRowData', empId);
        $("#txtEmpCode").val(ret.EMPCODE_EM).change();
        $("#txtEmpName").val(ret.EMPFNAME_EM).change();
        $('#EmployeeSearchModel').modal('toggle');
    }
};
var productSelect = function (prdId) {
    if (prdId) {
        var ret = jQuery("#tblProductSearch").jqGrid('getRowData', prdId);
        $("#txtPrCode").val(ret.PROD_CODE_PM).change();
        $("#txtPrDetail").val(ret.DESCRIPTION_PM).change();
        $('#PrdSearchModel').modal('toggle');
    }
};
var jobSelect = function (jobId) {
    if (jobId) {
        var ret = jQuery("#tblJobModalSearch").jqGrid('getRowData', jobId);
        $("#txtSowid").val(ret.JOBID_JR).change();
        $("#txtSowDetail").val(ret.JOBDESCRIPTION_JR).change();
        $("#txtPrRate").val(ret.RATE_RJ).change();
        $('#JobSearchModel').modal('toggle');
    }
};
var customerSelect = function (custId) {
    if (custId) {
        var ret = jQuery("#tblCustomerSearch").jqGrid('getRowData', custId);
        $("#txtCreditCustCode").val(ret.ARCODE_ARM).change();
        $("#txtCreditCustName").val(ret.DESCRIPTION_ARM).change();
        $('#CustomerSearchModel').modal('toggle');
    }
};
var DeleteSalesById = function (salesId) {
    if (salesId) {
        var data = JSON.stringify({ salesId: salesId });
        $.ajax({
            url: '/Job/DeleteSalebyId',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: data,
            type: "POST",
            success: function (status) {
                if (status) {
                    $('#tblSales').trigger('reloadGrid');
                    toastr.success("Operation Completed Succesfully");
                } else {
                    toastr.error("Sorry! Something went wrong, please try again.");
                }
            },
            complete: function () {
            },
            error: function () {
                toastr.error("Sorry! Something went wrong, please try again.");
            }
        });
    }
};
$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liSalesEntry").addClass("active");
    $("#txtSaleDate").datepicker();
    $('#ddlSaleType').on("change", function () {
        if ($('#ddlSaleType').val() === "Product") {
            $("#divSowCode").hide();
            $("#divSowDetail").hide();
            $("#txtSowid").val("");
            $("#txtSowDetail").val("");
            $("#txtPrDetail").attr("required", "true");
            $("#txtSowDetail").removeAttr("required");
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
            $("#txtSowDetail").attr("required", "true");
            $("#txtPrDetail").removeAttr("required");
        }
    });
    $('#ddlPayMode').on("change", function () {
        var totalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount;
        var $Vat = 0;
        var $taxedAmount = 0;
        $("#txtPrAmount").val(totalAmount);
        if ($('#ddlPayMode').val() === "Cash") {
            $finalAmount = $("#txtShipCharge").val() * 1 + totalAmount - $("#txtDiscount").val() * 1;
            $Vat = ((totalAmount - $("#txtDiscount").val()) * 5.0) / 100;
            $taxedAmount = $finalAmount + $Vat;
            $finalAmount = $taxedAmount;
            $("#txtCreditAmount").val("0");
            $("#txtCreditCustCode").val("");
            $("#txtCreditCustName").val("");
            $("#txtCashAmount").attr("required", "true");
            $("#txtCreditCustName").removeAttr("required");
            $("#divCreditAmount").hide();
            $("#divCreditCustDetails").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() === "Credit") {
            $finalAmount = $("#txtShipCharge").val() * 1 + totalAmount - $("#txtDiscount").val() * 1;
            $Vat = ((totalAmount - $("#txtDiscount").val()) * 5.0) / 100;
            $taxedAmount = $finalAmount + $Vat;
            $finalAmount = $taxedAmount;
            $("#txtCashAmount").val("0");
            $("#txtCreditCustName").attr("required", "true");
            $("#txtCashAmount").removeAttr("required");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
            $("#divCreditCustDetails").show();
            if ($custCode !== "CASH") {
                $("#txtCreditCustCode").val($custCode);
                $("#txtCreditCustName").val($custName);
            }
            else {
                $("#txtCreditCustCode").val("");
                $("#txtCreditCustName").val("");
            }
        }
        $('#txtVAT').val($Vat);
    });
    $("#txtQty").on("blur", function () {
        if ($("#txtQty").val() === "") {
            $("#txtQty").val("1");
        }
        var totalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount;
        var $Vat = 0;
        var $taxedAmount = 0;
        $("#txtPrAmount").val(totalAmount);
        if ($('#ddlPayMode').val() === "Cash") {
            $finalAmount = $("#txtShipCharge").val() - $("#txtDiscount").val() + totalAmount;
            $Vat = ((totalAmount - $("#txtDiscount").val()) * 5.0) / 100;
            $taxedAmount = $finalAmount + $Vat;
            $finalAmount = $taxedAmount;
            $("#txtCreditAmount").val("0");
            $("#divCreditAmount").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() === "Credit") {
            $finalAmount = $("#txtShipCharge").val() - $("#txtDiscount").val() + totalAmount;
            $Vat = ((totalAmount - $("#txtDiscount").val()) * 5.0) / 100;
            $taxedAmount = $finalAmount + $Vat;
            $finalAmount = $taxedAmount;
            $("#txtCashAmount").val("0");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
        }
        $('#txtVAT').val($Vat);
    });
    $("#txtPrRate").on("blur", function () {
        if ($("#txtPrRate").val() === "") {
            $("#txtPrRate").val(0);
        }
        var totalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount;
        var $Vat = 0;
        var $taxedAmount = 0;
        $("#txtPrAmount").val(totalAmount);
        if ($('#ddlPayMode').val() === "Cash") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + totalAmount;
            $Vat = ((totalAmount - $("#txtDiscount").val()) * 5.0) / 100;
            $taxedAmount = $finalAmount + $Vat;
            $finalAmount = $taxedAmount;
            $("#txtCreditAmount").val("0");
            $("#divCreditAmount").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() === "Credit") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + totalAmount;
            $Vat = ((totalAmount - $("#txtDiscount").val()) * 5.0) / 100;
            $taxedAmount = $finalAmount + $Vat;
            $finalAmount = $taxedAmount;
            $("#txtCashAmount").val("0");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
        }
        $('#txtVAT').val($Vat);
    });
    $("#txtDiscount").on("blur", function () {
        if ($("#txtDiscount").val() === "") {
            $("#txtDiscount").val("0");
        }
        var totalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount;
        var $Vat = 0;
        var $taxedAmount = 0;
        $("#txtPrAmount").val(totalAmount);
        if ($('#ddlPayMode').val() === "Cash") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + totalAmount;
            $Vat = ((totalAmount - $("#txtDiscount").val()) * 5.0) / 100;
            $taxedAmount = $finalAmount + $Vat;
            $finalAmount = $taxedAmount;
            $("#divCreditAmount").val("0");
            $("#divCreditAmount").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() === "Credit") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + totalAmount;
            $Vat = ((totalAmount - $("#txtDiscount").val()) * 5.0) / 100;
            $taxedAmount = $finalAmount + $Vat;
            $finalAmount = $taxedAmount;
            $("#divCashAmount").val("0");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
        }
        $('#txtVAT').val($Vat);
    });
    $("#txtShipCharge").on("blur", function () {
        if ($("#txtShipCharge").val() === "") {
            $("#txtShipCharge").val("0");
        }
        var totalAmount = $("#txtQty").val() * $("#txtPrRate").val();
        var $finalAmount;
        var $Vat = 0;
        var $taxedAmount = 0;
        $("#txtPrAmount").val(totalAmount);
        if ($('#ddlPayMode').val() === "Cash") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + totalAmount;
            $Vat = ((totalAmount - $("#txtDiscount").val()) * 5.0) / 100;
            $taxedAmount = $finalAmount + $Vat;
            $finalAmount = $taxedAmount;
            $("#divCreditAmount").val("0");
            $("#divCreditAmount").hide();
            $('#txtCashAmount').val($finalAmount);
            $("#divCashAmount").show();
        }
        else if ($('#ddlPayMode').val() === "Credit") {
            $finalAmount = $("#txtShipCharge").val() * 1 - $("#txtDiscount").val() * 1 + totalAmount;
            $Vat = ((totalAmount - $("#txtDiscount").val()) * 5.0) / 100;
            $taxedAmount = $finalAmount + $Vat;
            $finalAmount = $taxedAmount;
            $("#divCashAmount").val("0");
            $("#divCashAmount").hide();
            $('#txtCreditAmount').val($finalAmount);
            $("#divCreditAmount").show();
        }
        $('#txtVAT').val($Vat);
    });
    var $mrvJobStatus = "N";
    /***** Start - Adding JQGRID Code For Searching Job Number and MRV Number****/
    var searchJobGrid = function (mrvNo, jobNo) {
        var postData = $("#tblJobSearch").jqGrid("getGridParam", "postData");
        postData["mrvNo"] = mrvNo;
        postData["jobNo"] = jobNo;
        $("#tblJobSearch").setGridParam({ postData: postData });
        $("#tblJobSearch").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtJobSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtJobSearch").val().length >= 3 || $("#txtJobSearch").val().length === 0;
        if (shouldSearch) {
            searchJobGrid($("#txtMrvSearch").val(), $("#txtJobSearch").val());
        }
    });
    $("#txtMrvSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtMrvSearch").val().length >= 3 || $("#txtMrvSearch").val().length === 0;
        if (shouldSearch) {
            searchJobGrid($("#txtMrvSearch").val(), $("#txtJobSearch").val());
        }
    });
    var searchGrid = function (searchById, searchByName, gridType) {
        if (gridType === "1") {
            var postData = $("#tblProductSearch").jqGrid("getGridParam", "postData");
            postData["prdCode"] = searchById;
            postData["prdName"] = searchByName;
            $("#tblProductSearch").setGridParam({ postData: postData });
            $("#tblProductSearch").trigger("reloadGrid", [{ page: 1 }]);
        }
        else if (gridType === "2") {
            postData = $("#tblJobModalSearch").jqGrid("getGridParam", "postData");
            postData["jobId"] = searchById;
            postData["jobName"] = searchByName;
            $("#tblJobModalSearch").setGridParam({ postData: postData });
            $("#tblJobModalSearch").trigger("reloadGrid", [{ page: 1 }]);
        }
        else if (gridType === "3") {
            postData = $("#tblCustomerSearch").jqGrid("getGridParam", "postData");
            postData["custId"] = searchById;
            postData["custName"] = searchByName;
            $("#tblCustomerSearch").setGridParam({ postData: postData });
            $("#tblCustomerSearch").trigger("reloadGrid", [{ page: 1 }]);
        }
        else if (gridType === "4") {
            postData = $("#tblEmployeeSearch").jqGrid("getGridParam", "postData");
            postData["searchById"] = searchById;
            postData["searchByName"] = searchByName;
            $("#tblEmployeeSearch").setGridParam({ postData: postData });
            $("#tblEmployeeSearch").trigger("reloadGrid", [{ page: 1 }]);
        }
    };
    $("#txtPrdIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtPrdIdSearch").val().length >= 1 || $("#txtPrdIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtPrdIdSearch").val(), $("#txtPrdNameSearch").val(), "1");
        }
    });
    $("#txtPrdNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtPrdNameSearch").val().length >= 3 || $("#txtPrdNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtPrdIdSearch").val(), $("#txtPrdNameSearch").val(), "1");
        }
    });
    $("#txtSowIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtSowIdSearch").val().length >= 1 || $("#txtSowIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtSowIdSearch").val(), $("#txtSowNameSearch").val(), "2");
        }
    });
    $("#txtSowNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtSowNameSearch").val().length >= 3 || $("#txtSowNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtSowIdSearch").val(), $("#txtSowNameSearch").val(), "2");
        }
    });
    $("#txtCustIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtCustIdSearch").val().length >= 1 || $("#txtCustIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtCustIdSearch").val(), $("#txtCustNameSearch").val(), "3");
        }
    });
    $("#txtCustNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtCustNameSearch").val().length >= 3 || $("#txtCustNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtCustIdSearch").val(), $("#txtCustNameSearch").val(), "3");
        }
    });
    $("#txtEmpIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtEmpIdSearch").val().length >= 1 || $("#txtEmpIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtEmpIdSearch").val(), $("#txtEmpNameSearch").val(), "4");
        }
    });
    $("#txtEmpNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtEmpNameSearch").val().length >= 3 || $("#txtEmpNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtEmpIdSearch").val(), $("#txtEmpNameSearch").val(), "4");
        }
    });
    $("#mrvJobSearchModel").on('show.bs.modal', function () {
        $("#tblJobSearch").jqGrid({
            url: '/Job/GetJobMrvList?jobStatus=' + $mrvJobStatus,
            datatype: "json",
            height: 150,
            shrinkToFit: true,
            autoheight: true,
            autowidth: true,
            styleUI: "Bootstrap",
            colNames: ['Job No', 'MRV No', ''],
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
            $("#txtJobid").val(ret.JOBNO_JM);
            $("#txtMRVNo").val(ret.MRVNO_JM);
            $('#mrvJobSearchModel').modal('toggle');
        }
        e.preventDefault();
    });

    $("#mrvJobSearchModel").on('hide.bs.modal', function () {
        var jobCode = $("#txtJobid").val();
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
                    $custCode = jobMrvDetails.custCode;
                    $("#txtCustName").val(jobMrvDetails.custName);
                    $custName = jobMrvDetails.custName;
                    $("#txtMRVProdCode").val(jobMrvDetails.prdCode);
                    $("#txtMRVProdDetail").val(jobMrvDetails.prdDetail);
                    if (jobMrvDetails.custCode === "CASH") {
                        $("#ddlPayMode").val("Cash").change();
                    } else {
                        $("#ddlPayMode").val("Credit").change();
                    }
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
            $('#formJobEntry').formValidation('revalidateField', 'Unit');
        }
    }).bind('focus', function () {
        $(this).autocomplete("search");
    });

    $('#txtEmpCode').on('change', function () {
        $('#formJobEntry').formValidation('revalidateField', 'EmpCode');
    });

    $('#txtEmpName').on('change', function () {
        $('#formJobEntry').formValidation('revalidateField', 'EmpName');
    });

    $("#PrdSearchModel").on('show.bs.modal', function () {
        $("#tblProductSearch").jqGrid({
            url: '/Product/GetProductDetailsList',
            datatype: "json",
            height: 150,
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Product Code', 'Product Details', ''],
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
            colNames: ['Product Code', 'Product Details', 'Rate', ''],
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
            $("#txtSowid").val(ret.JOBID_JR).change();
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

    $("#EmployeeSearchModel").on('show.bs.modal', function () {
        $("#tblEmployeeSearch").jqGrid({
            url: '/EmployeeMaster/GetEmployeeDetailsList',
            datatype: "json",
            height: 150,
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Employee Code', 'Employee Name', ''],
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
    $("#btnNew").on("click", function () {
        location.reload(true);
    });
    $("#btnPrint").on("click", function () {
        var jobCode = $('#txtJobid').val();
        if (jobCode !== "") {
            var jobCardUrl = "/MgcReports/JobCardDetails?jobNo=" + jobCode;
            window.open(jobCardUrl);
            toastr.info("Job Card is open in new window.");
        }
    });
    $("#btnDelete").on("click", function () {
        var jobCode = $('#txtJobid').val();
        if (jobCode !== "" && jobCode !== null) {
            var data = JSON.stringify({ jobId: jobCode });
            $.ajax({
                url: '/Job/DeleteJobsbyJobId',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: data,
                type: "POST",
                success: function (status) {
                    if (status) {
                        $('#tblSales').trigger('reloadGrid');
                        toastr.success("Operation Completed Succesfully");
                    } else {
                        toastr.error("Sorry! Something went wrong, please try again.");
                    }
                },
                complete: function () {
                },
                error: function () {
                    toastr.error("Sorry! Something went wrong, please try again.");
                }
            });
        }
    });
    $("#btnRemoveSale").on("click", function () {
        var id = jQuery("#tblSales").jqGrid('getGridParam', 'selrow');
        if (id) {
            DeleteSalesById(id);
        }
        e.preventDefault();
    });
    $("#SalesByJobNoModel").on('show.bs.modal', function () {
        $.jgrid.gridUnload('#tblSales');
        var jobCode = $('#txtJobid').val();
        $("#tblSales").jqGrid({
            url: '/Job/GetSalesbyJobId?jobCode=' + jobCode,
            datatype: "json",
            height: 150,
            styleUI: "Bootstrap",
            colNames: ['', 'Job No', 'Pr Code', 'S W Code', 'Desciption', 'Qty', 'Rate', 'Amount', 'Disc.', 'Ship. Chrge','VAT', ''],
            colModel: [
            { key: true, name: 'SLNO_SD', index: 'SLNO_SD', hidden: true },
            { key: false, name: 'JOBNO_SD', index: 'JOBNO_SD', width: 100 },
            { key: false, name: 'PRCODE_SD', index: 'PRCODE_SD', width: 50 },
            { key: false, name: 'JOBID_SD', index: 'JOBID_SD', width: 100 },
            { key: false, name: 'DESCRIPTION_SD', index: 'DESCRIPTION_SD', width: 400 },
            { key: false, name: 'QTY_SD', index: 'QTY_SD', width: 50 },
            { key: false, name: 'RATE_SD', index: 'RATE_SD', width: 50 },
            { key: false, name: 'AMOUNT_SD', index: 'AMOUNT_SD', width: 50 },
            { key: false, name: 'DISCOUNT_SD', index: 'DISCOUNT_SD', width: 50 },
            { key: false, name: 'SHIPPINGCHARGES_SD', index: 'SHIPPINGCHARGES_SD', width: 50 },
            { key: false, name: 'VAT_SD', index: 'VAT_SD', width: 50 },
            {
                name: "action",
                align: "center",
                sortable: false,
                title: false,
                fixed: false,
                width: 50,
                search: false,
                formatter: function (cellValue, options, rowObject) {
                    var markup = "<a %Href%> <i class='fa fa-trash-o style='color:black'></i></a>";
                    var replacements = {
                        "%Href%": "href=javascript:DeleteSalesById(&apos;" + rowObject.SLNO_SD + "&apos;);"
                    };
                    markup = markup.replace(/%\w+%/g, function (all) {
                        return replacements[all];
                    });
                    return markup;
                }
            }
            ],
            mtype: 'GET',
            gridview: true,
            viewrecords: true,
            sortorder: "desc",
            pager: jQuery('#salesPager'),
            caption: "Job Details",
            emptyrecords: "No Data to Display",
            //jsonReader: {
            //    root: "rows",
            //    page: "page",
            //    total: "total",
            //    records: "records",
            //    repeatitems: false
            //},
            //loadonce: true,
            width: 820,
            rowNum: 20,
            multiselect: false
        });
    });
    $('#formJobEntry').on('init.field.fv', function (e, data) {
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
            MRVNO_SD: {
                validators: {
                    notEmpty: {
                        message: 'MRV Number is required'
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
                    numeric: {
                        message: 'Numeric Only'
                    }
                }
            },
            DISCOUNT_SD: {
                validators: {
                    notEmpty: {
                        message: 'Discount is required'
                    },
                    numeric: {
                        message: 'Numeric Only'
                    }
                }
            },
            SHIPPINGCHARGES_SD: {
                validators: {
                    notEmpty: {
                        message: 'Ship Charges is required'
                    },
                    numeric: {
                        message: 'Numeric Only'
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