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

var customerSelect = function(customerId)
{
    if (customerId) {
        var ret = jQuery("#tblCustomerSearch").jqGrid('getRowData', customerId);
        $("#txtCustCode").val(ret.ARCODE_ARM).change();
        $("#txtCustName").val(ret.DESCRIPTION_ARM).change();
        $('#CustomerSearchModel').modal('toggle');
    }
}

$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liQuotation").addClass("active");
    var quotProducts = [];
    $('#txtQuotDate').datepicker();
    jQuery("#tblQuotDetails").jqGrid({
        datatype: "local",
        height: 150,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: ['PrCode', 'Product Description', 'Job ID', 'Job Description', 'Quantity', 'Rate', 'Amount'],
        colModel: [
            { name: 'PrCode', index: 'PrCode', width: 80, align: "center", sortable: false },
            { name: 'PrDesc', index: 'PrDesc', width: 300, align: "left", sortable: false },
            { name: 'JobId', index: 'JobId', width: 80, align: "center", sortable: false },
            { name: 'JobDesc', index: 'JobDesc', width: 300, align: "left", sortable: false },
            { name: 'Qty', index: 'Qty', width: 90, align: "right", sortable: false },
            { name: 'Rate', index: 'Rate', width: 100, align: "right", sortable: false },
            { name: 'Amount', index: 'Amount', width: 100, align: "right", sortable: false }
        ],
        multiselect: false,
        caption: "Product Details"
    });
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblQuotDetails').setGridWidth(outerwidth);
    });
    $("#txtPrCode").change(function () {
        $('#QuotProductModelform').bootstrapValidator('revalidateField', 'PrCode');
    });
    $("#txtPrDesc").change(function () {
        $('#QuotProductModelform').bootstrapValidator('revalidateField', 'PrDesc');
    });
    $("#txtJobID").change(function () {
        $('#QuotProductModelform').bootstrapValidator('revalidateField', 'JobID');
    });
    $("#txtJobDesc").change(function () {
        $('#QuotProductModelform').bootstrapValidator('revalidateField', 'JobDesc');
    });
    $("#txtQuantity").change(function () {
        $('#QuotProductModelform').bootstrapValidator('revalidateField', 'Quantity');
    });
    $("#txtRate").change(function () {
        $('#QuotProductModelform').bootstrapValidator('revalidateField', 'Rate');
    });
    function clearModalForm() {
        $("#txtPrCode").val("");
        $("#txtPrDesc").val("");
        $("#txtJobID").val("");
        $("#txtJobDesc").val("");
        $("#txtQuantity").val("");
        $("#txtRate").val("");
        $("#txtAmount").val("0");
    }
    $("#btnCancel").click(function () {
        clearModalForm();
    });
    $("#btnSave").click(function (e) {
        if ($("#QuotProductModelform").valid()) {
            e.preventDefault();
            var arrIndex = quotProducts.length;
            quotProducts[arrIndex] = {
                PrCode: $("#txtPrCode").val(), PrDesc: $("#txtPrDesc").val(), JobId: $("#txtJobID").val(),
                JobDesc: $("#txtJobDesc").val(), Qty: $("#txtQuantity").val(), Rate: $("#txtRate").val(),
                Amount: $("#txtAmount").val()
            };
            var su = jQuery("#tblQuotDetails").jqGrid('addRowData', arrIndex, quotProducts[arrIndex]);
            if (su) {
                var mrvPrds = $('#tblQuotDetails').jqGrid('getGridParam', 'data');
                var jsonQuotPrds = JSON.stringify(mrvPrds);
                $('#hdnQuotProds').val(jsonQuotPrds);
                clearModalForm();
            }
        }
        else {
            $("#QuotProductModelform").bootstrapValidator('revalidateField', 'PrCode');
            $("#QuotProductModelform").bootstrapValidator('revalidateField', 'PrDesc');
            $("#QuotProductModelform").bootstrapValidator('revalidateField', 'JobID');
            $("#QuotProductModelform").bootstrapValidator('revalidateField', 'JobDesc');
            $("#QuotProductModelform").bootstrapValidator('revalidateField', 'Quantity');
            $("#QuotProductModelform").bootstrapValidator('revalidateField', 'Rate');
            $("#QuotProductModelform").bootstrapValidator('revalidateField', 'Amount');
        }
    });
    $("#QuotProductModel").on('hide.bs.modal', function () {
        clearModalForm();
        var totalGridPrdAmount = 0.0;
        for (var i = 0; i < quotProducts.length; i++) {
            totalGridPrdAmount += parseFloat(quotProducts[i]["Amount"]);
        }
        $("#txtNetPrdAmount").val(totalGridPrdAmount);
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
        $('#formMRVCreation').bootstrapValidator('revalidateField', 'CUSTNAME_QM');
    });
    $('#txtCustCode').on('blur', function () {
        if ($('#txtCustCode').val() === "") {
            $('#txtCustCode').val("CASH").change();
        }
    });
    $('#txtCustName').on('change', function () {
        $('#formMRVCreation').bootstrapValidator('revalidateField', 'CustName');
    });
    $('#txtQuotationNo').on('change', function () {
        $('#formMRVCreation').bootstrapValidator('revalidateField', 'QUOTNO_QM');
    });
    $('#txtAddress1').on('change', function () {
        $('#formMRVCreation').bootstrapValidator('revalidateField', 'ADDRESS1_QM');
    });
    $("#CustomerSearchModel").on('show.bs.modal', function () {
        var $CustType = "AR";
        jQuery("#tblCustomerSearch").jqGrid({
            url: '/Customer/GetCustList?custType=' + $CustType,
            datatype: "json",
            height: 150,
            shrinkToFit: true,
            autoheight: true,
            autowidth: true,
            styleUI: "Bootstrap",
            colNames: ['Customer Code', 'Customer Name', ''],
            colModel: [
                {key:true, name: 'ARCODE_ARM', index: 'ARCODE_ARM', width: 400 },
                {key:false, name: 'DESCRIPTION_ARM', index: 'DESCRIPTION_ARM', width: 400 },
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
    $('#formQuotationEntry').bootstrapValidator({
        container: '#messages',
        feedbackIcons: {
            required: 'fa fa-asterisk',
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
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
    });
    $("#formQuotationEntry").formValidation();
});