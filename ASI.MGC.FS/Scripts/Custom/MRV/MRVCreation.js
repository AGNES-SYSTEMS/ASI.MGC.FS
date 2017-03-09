var arrMetarials = [];
var selectedRowId = "";
var jobSelect = function (jobId) {
    if (jobId) {
        var ret = jQuery("#tblJobSearch").jqGrid('getRowData', jobId);
        $("#txtJobID").val(ret.JOBID_JR).change();
        $("#txtJobDesc").val(ret.JOBDESCRIPTION_JR).change();
        $("#txtRate").val(ret.RATE_RJ).change();
        $('#mrvJobSearchModel').modal('toggle');
    }
};
var productSelect = function (prdId) {
    if (prdId) {
        var ret = jQuery("#tblProductSearch").jqGrid('getRowData', prdId);
        $("#txtPrCode").val(ret.PROD_CODE_PM).change();
        $("#txtPrDesc").val(ret.DESCRIPTION_PM).change();
        $('#mrvPrdSearchModel').modal('toggle');
    }
};
var calculateNetAmount = function () {
    var totalGridPrdAmount = 0.0;
    for (var i = 0; i < arrMetarials.length; i++) {
        totalGridPrdAmount += parseFloat(arrMetarials[i]["AMOUNT_MRR"]);
    }
    $("#txtNetPrdAmount").val(totalGridPrdAmount);
    $('#formMRVCreation').formValidation('revalidateField', 'netPrdAmount');
}
var delProduct = function (rowId) {
    if (rowId) {
        $('#tblMetarials').jqGrid('delRowData', rowId);
        $('#tblMetarials').trigger('reloadGrid');
        stringifyData();
        calculateNetAmount();
    }
};
var customerSelect = function (custId) {
    if (custId) {
        var ret = jQuery("#tblCustomerSearch").jqGrid('getRowData', custId);
        $("#txtCustCode").val(ret.ARCODE_ARM).change();
        $("#txtCustName").val(ret.DESCRIPTION_ARM).change();
        $('#CustomerSearchModel').modal('toggle');
    }
}
var stringifyData = function () {
    var jsonMrvPrds = JSON.stringify(arrMetarials);
    $('#mrvProds').val(jsonMrvPrds);
};
$(document).ready(function () {
    $("#quickLinks").children("li.active").removeClass("active");
    $("#liMrv").addClass("active");
    arrMetarials = [];
    $('#txtDeleDate').datepicker();
    $('#txtMRVDate').datepicker();
    jQuery("#tblMetarials").jqGrid({
        datatype: "local",
        data: arrMetarials,
        height: 150,
        shrinkToFit: true,
        autoheight: true,
        autowidth: true,
        styleUI: "Bootstrap",
        colNames: ['PrCode', 'Product Description', 'Job id', 'Job Description', 'Quantity', 'Rate', 'Amount', '', ''],
        colModel: [
            { name: 'PRODID_MRR', index: 'PRODID_MRR', width: 80, align: "center", sortable: false },
            { name: 'prdesc', index: 'prdesc', width: 300, align: "left", sortable: false },
            { name: 'JOBID_MRR', index: 'JOBID_MRR', width: 80, align: "center", sortable: false },
            { name: 'jobdesc', index: 'jobdesc', width: 300, align: "left", sortable: false },
            { name: 'QTY_MRR', index: 'QTY_MRR', width: 90, align: "right", sortable: false },
            { name: 'RATE_MRR', index: 'RATE_MRR', width: 100, align: "right", sortable: false },
            { name: 'AMOUNT_MRR', index: 'AMOUNT_MRR', width: 100, align: "right", sortable: false },
            {
                name: "action",
                align: "center",
                sortable: false,
                title: false,
                fixed: false,
                width: 50,
                search: false,
                formatter: function (cellValue, options) {

                    var markup = "<a %Href% data-toggle='modal' %Id% data-target='#mrvProductModel'> <i class='fa fa-pencil-square-o style='color:black'></i></a>";
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
        caption: "Materials Details"
    });
    $("#mrvProductModel").on('show.bs.modal', function (e) {
        if (e.relatedTarget.id) {
            selectedRowId = e.relatedTarget.id;
            var rowId = e.relatedTarget.id;
            var ret = $('#tblMetarials').jqGrid('getRowData', rowId);
            $("#txtPrCode").val(ret.PRODID_MRR);
            $("#txtPrDesc").val(ret.prdesc);
            $("#txtJobID").val(ret.JOBID_MRR);
            $("#txtJobDesc").val(ret.jobdesc);
            $("#txtQuantity").val(ret.QTY_MRR);
            $("#txtRate").val(ret.RATE_MRR);
            $("#txtAmount").val(ret.AMOUNT_MRR);
        }
    });
    $("#txtPrCode").change(function () {
        $('#mrvProductModelform').bootstrapValidator('revalidateField', 'PrCode');
    });
    $("#btnNew").on("click", function () {
        location.reload();
    });
    $("#txtPrDesc").change(function () {
        $('#mrvProductModelform').bootstrapValidator('revalidateField', 'PrDesc');
    });

    $("#txtJobID").change(function () {
        $('#mrvProductModelform').bootstrapValidator('revalidateField', 'JobID');
    });

    $("#txtJobDesc").change(function () {
        $('#mrvProductModelform').bootstrapValidator('revalidateField', 'JobDesc');
    });

    $("#txtQuantity").change(function () {
        $('#mrvProductModelform').bootstrapValidator('revalidateField', 'Quantity');
    });

    $("#txtRate").change(function () {
        $('#mrvProductModelform').bootstrapValidator('revalidateField', 'Rate');
    });

    function clearModalForm() {
        $("#txtPrCode").val("");
        $("#txtPrDesc").val("");
        $("#txtJobID").val("");
        $("#txtJobDesc").val("");
        $("#txtQuantity").val("1");
        $("#txtRate").val("");
        $("#txtAmount").val("0");
        selectedRowId = "";
    }
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
    $("#btnCancel").click(function () {
        clearModalForm();
    });
    $("#btnSave").click(function (e) {
        if ($("#mrvProductModelform").valid()) {
            e.preventDefault();
            if (selectedRowId) {
                arrMetarials[parseInt(selectedRowId) - 1] = {
                    PRODID_MRR: $("#txtPrCode").val(),
                    prdesc: $("#txtPrDesc").val(),
                    JOBID_MRR: $("#txtJobID").val(),
                    jobdesc: $("#txtJobDesc").val(),
                    QTY_MRR: $("#txtQuantity").val(),
                    RATE_MRR: $("#txtRate").val(),
                    AMOUNT_MRR: $("#txtAmount").val()
                };
            } else {
                var arrIndex = arrMetarials.length;
                arrMetarials[arrIndex] = {
                    PRODID_MRR: $("#txtPrCode").val(),
                    prdesc: $("#txtPrDesc").val(),
                    JOBID_MRR: $("#txtJobID").val(),
                    jobdesc: $("#txtJobDesc").val(),
                    QTY_MRR: $("#txtQuantity").val(),
                    RATE_MRR: $("#txtRate").val(),
                    AMOUNT_MRR: $("#txtAmount").val()
                };
            }
            clearModalForm();
        }
        else {
            $("#mrvProductModelform").bootstrapValidator('revalidateField', 'PrCode');
            $("#mrvProductModelform").bootstrapValidator('revalidateField', 'PrDesc');
            $("#mrvProductModelform").bootstrapValidator('revalidateField', 'JobID');
            $("#mrvProductModelform").bootstrapValidator('revalidateField', 'JobDesc');
            $("#mrvProductModelform").bootstrapValidator('revalidateField', 'Quantity');
            $("#mrvProductModelform").bootstrapValidator('revalidateField', 'Rate');
            $("#mrvProductModelform").bootstrapValidator('revalidateField', 'Amount');
        }
    });
    $(window).resize(function () {
        var outerwidth = $('#grid').width();
        $('#tblMetarials').setGridWidth(outerwidth);
    });

    $("#mrvProductModel").on('hide.bs.modal', function () {
        clearModalForm();
        $('#tblMetarials').trigger('reloadGrid');
        stringifyData();
        calculateNetAmount();
    });
    //$("#txtCustCode").autocomplete({
    //    source: '/Customer/GetCustomersCode',
    //    minLength: 0
    //}).bind('focus', function () {
    //    $(this).autocomplete("search");
    //});
    //$("#txtCustName").autocomplete({
    //    source: '/Customer/GetCustomersName',
    //    minLength: 0
    //}).bind('focus', function () {
    //    $(this).autocomplete("search");
    //});

    $("#txtExeCode").autocomplete({
        source: '/EmployeeMaster/getEmployeeIDs',
        minLength: 0,
        change: function () {
            $('#formMRVCreation').bootstrapValidator('revalidateField', 'EXECODE_MRV');
        }
    }).bind('focus', function () {
        $(this).autocomplete("search");
    });
    ////function getCustRecord() {
    //    var custCode = $('#txtCustCode').val();
    //    var custName = $('#txtCustName').val();
    //    var data = JSON.stringify({ custCode: custCode, custName: custName });
    //    $.ajax({
    //        url: '/Customer/getCustomerRecord',
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        data: data,
    //        type: "POST",
    //        success: function (custDetails) {
    //            $('#txtCustCode').val(custDetails.ARCODE_ARM).change();
    //            $('#txtCustName').val(custDetails.DESCRIPTION_ARM).change();
    //            $('#txtMRVAddress').val(custDetails.ADDRESS1_ARM).change();
    //            $('#txtMRVTel').val(custDetails.TELEPHONE_ARM).change();
    //            $('#txtMRVAddress2').val(custDetails.ADDRESS2_ARM);
    //        },
    //        complete: function () {
    //        },
    //        error: function () {
    //        }
    //    });
    //}
    $("#txtCustCode").blur(function () {
        $('#txtCustName').val("");
        //getCustRecord();
    });

    $("#txtCustName").blur(function () {
        if ($('#txtCustName').val() === "") {
            $('#txtCustCode').val("CASH");
            $('#txtMRVAddress').val("").change();
            $('#txtMRVTel').val("").change();
            $('#txtMRVAddress2').val("");
        }

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
        $('#formMRVCreation').bootstrapValidator('revalidateField', 'CUSTOMERCODE_MRV');
    });
    $('#txtCustName').on('change', function () {
        $('#formMRVCreation').bootstrapValidator('revalidateField', 'CUSTOMERNAME_MRV');
    });
    $('#txtMRVAddress').on('change', function () {
        $('#formMRVCreation').bootstrapValidator('revalidateField', 'ADDRESS1_MRV');
    });
    $('#txtMRVTel').on('change', function () {
        $('#formMRVCreation').bootstrapValidator('revalidateField', 'PHONE_MRV');
    });
    $("#mrvPrdSearchModel").on('show.bs.modal', function () {
        $("#tblProductSearch").jqGrid({
            url: '/Product/GetProductDetailsList',
            datatype: "json",
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Product Code', 'Product Details', ''],
            colModel: [
            { key: true, name: 'PROD_CODE_PM', index: 'PROD_CODE_PM', width: 250 },
            { key: false, name: 'DESCRIPTION_PM', index: 'DESCRIPTION_PM', width: 700 },
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
    $("#CustomerSearchModel").on('hide.bs.modal', function () {
        var custCode = $('#txtCustCode').val();
        var custName = $('#txtCustName').val();
        var data = JSON.stringify({ custCode: custCode, custName: custName });
        $.ajax({
            url: '/Customer/getCustomerRecord',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: data,
            type: "POST",
            success: function (custDetails) {
                $('#txtCustCode').val(custDetails.ARCODE_ARM).change();
                $('#txtCustName').val(custDetails.DESCRIPTION_ARM).change();
                $('#txtMRVAddress').val(custDetails.ADDRESS1_ARM).change();
                $('#txtMRVTel').val(custDetails.TELEPHONE_ARM).change();
                $('#txtMRVAddress2').val(custDetails.ADDRESS2_ARM);
            },
            complete: function () {
            },
            error: function () {
            }
        });
    });
    var searchGridCust = function (searchById, searchByName) {
        var custpostData = $("#tblCustomerSearch").jqGrid("getGridParam", "postData");
        custpostData["custId"] = searchById;
        custpostData["custName"] = searchByName;
        $("#tblCustomerSearch").setGridParam({ postData: custpostData });
        $("#tblCustomerSearch").trigger("reloadGrid", [{ page: 1 }]);
    };
    $("#txtCustIdSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtCustIdSearch").val().length >= 1 || $("#txtCustIdSearch").val().length === 0;
        if (shouldSearch) {
            searchGridCust($("#txtCustIdSearch").val(), $("#txtCustNameSearch").val(), "3");
        }
    });
    $("#txtCustNameSearch").off().on("keyup", function () {

        var shouldSearch = $("#txtCustNameSearch").val().length >= 3 || $("#txtCustNameSearch").val().length === 0;
        if (shouldSearch) {
            searchGridCust($("#txtCustIdSearch").val(), $("#txtCustNameSearch").val(), "3");
        }
    });
    $("#btnCustSelect").on("click", function (e) {
        var id = jQuery("#tblCustomerSearch").jqGrid('getGridParam', 'selrow');
        if (id) {
            var ret = jQuery("#tblCustomerSearch").jqGrid('getRowData', id);
            $("#txtCustCode").val(ret.ARCODE_ARM).change();
            $("#txtCustName").val(ret.DESCRIPTION_ARM).change();
            $('#CustomerSearchModel').modal('toggle');
        }
        e.preventDefault();
    });
    $("#btnSearchPrd").on("click", function (e) {
        e.preventDefault();
        var $postDataValues = $("tblProductSearch").jqGrid('getGridParam', 'postData');
        $('.filterProduct').each(function (index, item) {
            $postDataValues[$(item).attr("id")] = $(item).val();
        });
        $("tblProductSearch").jqGrid().setGridParam({ postData: $postDataValues, page: 1 }).trigger('reloadGrid');
    });

    $("#mrvJobSearchModel").on('show.bs.modal', function () {
        $("#tblJobSearch").jqGrid({
            url: '/Job/GetJobDetailsList',
            datatype: "json",
            autoheight: true,
            styleUI: "Bootstrap",
            colNames: ['Product Code', 'Product Details', 'Rate', ''],
            colModel: [
            { key: true, name: 'JOBID_JR', index: 'JOBID_JR', width: 250 },
            { key: false, name: 'JOBDESCRIPTION_JR', index: 'JOBDESCRIPTION_JR', width: 550 },
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
            $("#txtPrDesc").val(ret.DESCRIPTION_PM).change();
            $('#mrvPrdSearchModel').modal('toggle');
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
            $('#mrvJobSearchModel').modal('toggle');
        }
        e.preventDefault();
    });

    $('#formMRVCreation').on('init.field.fv', function (e, data) {
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
            CUSTOMERCODE_MRV: {
                required: true,
                validators: {
                    notEmpty: {
                        message: 'Customer Code is required'
                    }
                }
            },
            CUSTOMERNAME_MRV: {
                validators: {
                    notEmpty: {
                        message: 'Customer Name is required'
                    }
                }
            },
            MRVDATE_MRV: {
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
            ADDRESS1_MRV: {
                validators: {
                    notEmpty: {
                        message: 'Address is required'
                    }
                }
            },
            PHONE_MRV: {
                validators: {
                    notEmpty: {
                        message: 'Phone Number is required'
                    },
                    digit: {
                        message: 'Enter Valid Phone Number'
                    }
                }
            },
            DELE_DATE_MRV: {
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
            EXECODE_MRV: {
                validators: {
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
    $("#mrvProductModelform").formValidation();
});