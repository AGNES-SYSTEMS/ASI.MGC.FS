var quotProducts = [];
var selectedRowId = "";
var jobSelect = function (jobId) {
    if (jobId) {
        var ret = jQuery("#tblJobSearch").jqGrid('getRowData', jobId);
        $("#txtJobID").val(ret.JOBID_JR).change();
        $("#txtJobDesc").val(ret.JOBDESCRIPTION_JR).change();
        $("#txtRate").val(ret.RATE_RJ).change();
        $('#QuotJobSearchModel').modal('toggle');
    }
};
var productSelect = function (prdId) {
    if (prdId) {
        var ret = jQuery("#tblProductSearch").jqGrid('getRowData', prdId);
        $("#txtPrCode").val(ret.PROD_CODE_PM).change();
        $("#txtPrDesc").val(ret.DESCRIPTION_PM).change();
        $('#QuotPrdSearchModel').modal('toggle');
    }
};
var customerSelect = function (customerId) {
    if (customerId) {
        var ret = jQuery("#tblCustomerSearch").jqGrid('getRowData', customerId);
        $("#txtCustCode").val(ret.ARCODE_ARM).change();
        $("#txtCustName").val(ret.DESCRIPTION_ARM).change();
        $('#CustomerSearchModel').modal('toggle');
    }
};
var delProduct = function (rowId) {
    if (rowId) {
        $('#tblQuotDetails').jqGrid('delRowData', rowId);
        $('#tblQuotDetails').trigger('reloadGrid');
        stringifyData();
        calculateNetAmount();
    }
};
var calculateNetAmount = function () {
    var totalGridPrdAmount = 0.0;
    for (var i = 0; i < quotProducts.length; i++) {
        totalGridPrdAmount += parseFloat(quotProducts[i]["Amount"]);
    }
    $("#txtNetPrdAmount").val(totalGridPrdAmount);
    $("#formQuotationEntry").formValidation('revalidateField', 'NetPrdAmount');
};
var stringifyData = function () {
    var quotPrds = $('#tblQuotDetails').jqGrid('getGridParam', 'data');
    var jsonQuotPrds = JSON.stringify(quotPrds);
    $('#hdnQuotProds').val(jsonQuotPrds);
};
$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liQuotation").addClass("active");
    quotProducts = [];
    $('#txtQuotDate').datepicker();
    jQuery("#tblQuotDetails").jqGrid({
        datatype: "local",
        data: quotProducts,
        height: 150,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: ['PrCode', 'Product Description', 'Job id', 'Job Description', 'Quantity', 'Rate', 'Amount', '', ''],
        colModel: [
            { name: 'PrCode', index: 'PrCode', width: 80, align: "center", sortable: false },
            { name: 'PrDesc', index: 'PrDesc', width: 300, align: "left", sortable: false },
            { name: 'JobId', index: 'JobId', width: 80, align: "center", sortable: false },
            { name: 'JobDesc', index: 'JobDesc', width: 300, align: "left", sortable: false },
            { name: 'Qty', index: 'Qty', width: 90, align: "right", sortable: false },
            { name: 'Rate', index: 'Rate', width: 100, align: "right", sortable: false },
            { name: 'Amount', index: 'Amount', width: 100, align: "right", sortable: false },
            {
                name: "action",
                align: "center",
                sortable: false,
                title: false,
                fixed: false,
                width: 50,
                search: false,
                formatter: function (cellValue, options) {

                    var markup = "<a %Href% data-toggle='modal' %Id% data-target='#QuotProductModel'> <i class='fa fa-pencil-square-o style='color:black'></i></a>";
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
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblQuotDetails').setGridWidth(outerwidth);
    });
    $("#txtPrCode").change(function () {
        $('#QuotProductModelform').formValidation('revalidateField', 'PrCode');
    });
    $("#txtPrDesc").change(function () {
        $('#QuotProductModelform').formValidation('revalidateField', 'PrDesc');
    });
    $("#txtJobID").change(function () {
        $('#QuotProductModelform').formValidation('revalidateField', 'JobID');
    });
    $("#txtJobDesc").change(function () {
        $('#QuotProductModelform').formValidation('revalidateField', 'JobDesc');
    });
    $("#txtQuantity").change(function () {
        $('#QuotProductModelform').formValidation('revalidateField', 'Quantity');
    });
    $("#txtRate").change(function () {
        $('#QuotProductModelform').formValidation('revalidateField', 'Rate');
    });
    function clearModalForm() {
        $("#txtPrCode").val("");
        $("#txtPrDesc").val("");
        $("#txtJobID").val("");
        $("#txtJobDesc").val("");
        $("#txtQuantity").val("");
        $("#txtRate").val("");
        $("#txtAmount").val("0");
        selectedRowId = "";
    }
    $("#btnCancel").click(function () {
        clearModalForm();
    });
    $("#btnSave").click(function (e) {
        if ($("#QuotProductModelform").valid()) {
            e.preventDefault();
            if (selectedRowId) {
                quotProducts[selectedRowId - 1] = {
                    PrCode: $("#txtPrCode").val(),
                    PrDesc: $("#txtPrDesc").val(),
                    JobId: $("#txtJobID").val(),
                    JobDesc: $("#txtJobDesc").val(),
                    Qty: $("#txtQuantity").val(),
                    Rate: $("#txtRate").val(),
                    Amount: $("#txtAmount").val()
                }
            }
            else {
                var arrIndex = quotProducts.length;
                quotProducts[arrIndex] = {
                    PrCode: $("#txtPrCode").val(),
                    PrDesc: $("#txtPrDesc").val(),
                    JobId: $("#txtJobID").val(),
                    JobDesc: $("#txtJobDesc").val(),
                    Qty: $("#txtQuantity").val(),
                    Rate: $("#txtRate").val(),
                    Amount: $("#txtAmount").val()
                }
            };
            clearModalForm();
        }
        else {
            //$("#QuotProductModelform").formValidation('revalidateField', 'PrCode');
            $("#QuotProductModelform").formValidation('revalidateField', 'PrDesc');
            //$("#QuotProductModelform").formValidation('revalidateField', 'JobID');
            $("#QuotProductModelform").formValidation('revalidateField', 'JobDesc');
            $("#QuotProductModelform").formValidation('revalidateField', 'Quantity');
            $("#QuotProductModelform").formValidation('revalidateField', 'Rate');
            $("#QuotProductModelform").formValidation('revalidateField', 'Amount');
        }
    });
    $("#QuotProductModel").on('show.bs.modal', function (e) {
        if (e.relatedTarget.id) {
            selectedRowId = e.relatedTarget.id;
            var rowId = e.relatedTarget.id;
            var ret = $('#tblQuotDetails').jqGrid('getRowData', rowId);
            $("#txtPrCode").val(ret.PrCode);
            $("#txtPrDesc").val(ret.PrDesc);
            $("#txtJobID").val(ret.JobId),
            $("#txtJobDesc").val(ret.JobDesc),
            $("#txtQuantity").val(ret.Qty);
            $("#txtRate").val(ret.Rate);
            $("#txtAmount").val(ret.Amount);
        }
    });
    $("#QuotProductModel").on('hide.bs.modal', function () {
        clearModalForm();
        $('#tblQuotDetails').trigger('reloadGrid');
        stringifyData();
        calculateNetAmount();
    });
    $("#txtQuantity").change(function () {
        var totalAmount = $("#txtQuantity").val() * $("#txtRate").val();
        $("#txtAmount").val(totalAmount);
    });
    $("#txtRate").change(function () {
        var totalAmount = $("#txtQuantity").val() * $("#txtRate").val();
        $("#txtAmount").val(totalAmount);
    });
    $('#txtCustCode').on('change', function () {
        $('#formQuotationEntry').formValidation('revalidateField', 'CUSTNAME_QM');
    });
    $('#txtCustCode').on('blur', function () {
        if ($('#txtCustCode').val() === "") {
            $('#txtCustCode').val("CASH").change();
        }
    });
    $('#txtCustName').on('change', function () {
        $('#formQuotationEntry').formValidation('revalidateField', 'CustName');
    });
    $('#txtQuotationNo').on('change', function () {
        $('#formQuotationEntry').formValidation('revalidateField', 'QUOTNO_QM');
    });
    $('#txtAddress1').on('change', function () {
        $('#formQuotationEntry').formValidation('revalidateField', 'ADDRESS1_QM');
    });
    $("#CustomerSearchModel").on('show.bs.modal', function () {
        var $CustType = "AR";
        jQuery("#tblCustomerSearch").jqGrid({
            url: '/Customer/GetAllCustomers?custType=' + $CustType,
            datatype: "json",
            height: 150,
            shrinkToFit: true,
            autoheight: true,
            autowidth: true,
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
            viewrecords: true,
            sortorder: "desc",
            pager: jQuery('#custPager'),
            caption: "Customer Details",
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
            var outerwidth = $('#custgrid').width();
            $('#tblCustomerSearch').setGridWidth(outerwidth);
        });
    });
    $("#CustomerSearchModel").on('hide.bs.modal', function () {
    });
    $("#btnCustomerSelect").on("click", function (e) {
        var id = jQuery("#tblCustomerSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblCustomerSearch").jqGrid('getRowData', id);
            $("#txtCustCode").val(ret.ARCODE_ARM).change();
            $("#txtCustName").val(ret.DESCRIPTION_ARM).change();
            $('#CustomerSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    $("#QuotPrdSearchModel").on('show.bs.modal', function () {
        $("#tblProductSearch").jqGrid({
            url: '/Product/GetProductDetailsList',
            datatype: "json",
            height: 150,
            shrinkToFit: true,
            autoheight: true,
            autowidth: true,
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
        $(window).resize(function () {
            var outerwidth = $('#prdGrid').width();
            $('#tblProductSearch').setGridWidth(outerwidth);
        });
    });
    $("#QuotJobSearchModel").on('show.bs.modal', function () {
        $("#tblJobSearch").jqGrid({
            url: '/Job/GetJobDetailsList',
            datatype: "json",
            height: 150,
            shrinkToFit: true,
            autoheight: true,
            autowidth: true,
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
            viewrecords: true,
            sortorder: "desc",
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
        $(window).resize(function () {
            var outerwidth = $('#jobGrid').width();
            $('#tblJobSearch').setGridWidth(outerwidth);
        });
    });
    $("#btnProductSelect").on("click", function (e) {
        var id = jQuery("#tblProductSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblProductSearch").jqGrid('getRowData', id);
            $("#txtPrCode").val(ret.PROD_CODE_PM).change();
            $("#txtPrDesc").val(ret.DESCRIPTION_PM).change();
            $('#QuotPrdSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    $("#btnJobSelect").on("click", function (e) {
        var id = jQuery("#tblJobSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblJobSearch").jqGrid('getRowData', id);
            $("#txtJobID").val(ret.JOBID_JR).change();
            $("#txtJobDesc").val(ret.JOBDESCRIPTION_JR).change();
            $("#txtRate").val(ret.RATE_RJ).change();
            $('#QuotJobSearchModel').modal('toggle');
        }
        e.preventDefault();
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
            postData = $("#tblJobSearch").jqGrid("getGridParam", "postData");
            postData["jobId"] = searchById;
            postData["jobName"] = searchByName;
            $("#tblJobSearch").setGridParam({ postData: postData });
            $("#tblJobSearch").trigger("reloadGrid", [{ page: 1 }]);
        }
        else if (gridType === "3") {
            postData = $("#tblCustomerSearch").jqGrid("getGridParam", "postData");
            postData["custId"] = searchById;
            postData["custName"] = searchByName;
            $("#tblCustomerSearch").setGridParam({ postData: postData });
            $("#tblCustomerSearch").trigger("reloadGrid", [{ page: 1 }]);
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
    $("#txtJobIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtJobIdSearch").val().length >= 1 || $("#txtJobIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtJobIdSearch").val(), $("#txtJobNameSearch").val(), "2");
        }
    });
    $("#txtJobNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtJobNameSearch").val().length >= 3 || $("#txtJobNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGrid($("#txtJobIdSearch").val(), $("#txtJobNameSearch").val(), "2");
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
    $('#formQuotationEntry').on('init.field.fv', function (e, data) {
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
            CUSTNAME_QM: {
                required: true,
                validators: {
                    notEmpty: {
                        message: 'Customer Code is required'
                    }
                }
            },
            CustName: {
                validators: {
                    notEmpty: {
                        message: 'Customer Name is required'
                    }
                }
            },
            DATE_QM: {
                validators: {
                    notEmpty: {
                        message: 'Date is required'
                    },
                    date: {
                        format: 'MM/DD/YYYY',
                        message: 'Enter Valid Date'
                    }
                }
            },
            ADDRESS1_QM: {
                validators: {
                    notEmpty: {
                        message: 'Address is required'
                    }
                }
            },
            QUOTNO_QM: {
                validators: {
                    notEmpty: {
                        message: 'Phone Number is required'
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
    $("#formQuotationEntry").formValidation();
});